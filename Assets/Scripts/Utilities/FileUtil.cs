using System;
using System.IO;
using UnityEngine;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Utils
{
    public static class FileUtil
    {
        readonly static string persistantPath = Application.persistentDataPath + "/SaveData";
        readonly static string resourcesPath = Application.dataPath + "/Resources/SaveData";

        /// <summary>
        /// Saves object as a json string. Object must implement <see cref="IJsonable"/>.
        /// </summary>
        public static void SaveAsJson(string directory, string filename, object data)
        {
            if (!data.GetType().GetInterfaces().Contains(typeof(IJsonable)))
            {
                LogUtil.WriteError("Object " + data + "does not implement " + typeof(IJsonable));
                return;
            }

            if (!Directory.Exists(directory))
            {
                try
                {
                    Directory.CreateDirectory(directory);
                }
                catch (Exception e)
                {
                    LogUtil.WriteException(e);
                    return;
                }
            }

            filename = filename.Replace('/', '_');
            filename = filename.Replace('\\', '_');
            string filePath = directory + "/" + filename + ".json";

            string dataString = JsonUtility.ToJson(data);
            int initialByteSize = dataString.Length;

            // compress
            dataString = ZipUtil.CompressToString(dataString);
            int finalByteSize = dataString.Length;
           
            LogUtil.Write(string.Format("Compressed object {0} to {1}% of original size ({2:##.###}kb to {3:##.###}kb)", data, (float)finalByteSize / initialByteSize * 100, (float)initialByteSize / 1000, (float)finalByteSize / 1000));
            
            try
            {
                File.WriteAllText(filePath, dataString);
            }
            catch (Exception e)
            {
                LogUtil.WriteException(e);
                return;
            }

            LogUtil.Write("Saved object " + data + " at " + filePath);
        }

        /// <summary>
        /// Saves object as a json string. Object must implement <see cref="IJsonable"/>.
        /// </summary>
        public static void SaveAsJsonToPersistantData(string filename, object data)
        {
            SaveAsJson(persistantPath, filename, data);
        }

        /// <summary>
        /// Saves object as a json string. Object must implement <see cref="IJsonable"/>.
        /// </summary>
        public static void SaveAsJsonToResources(string filename, object data)
        {
            SaveAsJson(resourcesPath, filename, data);
        }

        /// <summary>
        /// Loads object from a json string. Object must implement <see cref="IJsonable"/>.
        /// </summary>
        public static T LoadJson<T>(string directory, string filename)
        {
            if (!typeof(T).GetInterfaces().Contains(typeof(IJsonable)))
            {
                LogUtil.WriteError("Type " + typeof(T) + " does not implement " + typeof(IJsonable));
                return default(T);
            }

            string filePath = directory + "/" + filename + ".json";

            if (!File.Exists(filePath))
            {
                LogUtil.WriteWarning("Path: " + filePath + " does not exist.");
                return default(T);
            }

            string dataString;
            try
            {
                dataString = File.ReadAllText(filePath);
            }
            catch (Exception e)
            {
                LogUtil.WriteException(e);
                return default(T);
            }

            dataString = ZipUtil.Decompress(dataString);
            T dataObj = JsonUtility.FromJson<T>(dataString);

            LogUtil.Write("Loaded object " + dataObj + " from " + filePath);

            return dataObj;
        }

        /// <summary>
        /// Loads object from a json string. Object must implement <see cref="IJsonable"/>.
        /// </summary>
        public static T LoadFromPersistantData<T>(string filename)
        {
            return LoadJson<T>(persistantPath, filename);
        }

        /// <summary>
        /// Loads object from a json string. Object must implement <see cref="IJsonable"/>.
        /// </summary>
        public static T LoadFromResources<T>(string filename)
        {
            return LoadJson<T>(resourcesPath, filename);
        }

        /// <summary>
        /// Finds all <see cref="MonoBehaviour"/> in the scene that implement 
        /// <see cref="ISaveAndLoadable"/> and invokes their <see cref="ISaveAndLoadable.SaveData"/>
        /// method.
        /// </summary>
        public static void SaveAll()
        {
            var iFileables = UnityEngine.Object.FindObjectsOfType<MonoBehaviour>().OfType<ISaveAndLoadable>();
            foreach (var f in iFileables)
                f.SaveData();
        }
    }
}