using System;
using System.IO;
using UnityEngine;
using System.Linq;

namespace Utils
{
    public static class FileUtil
    {
        readonly static string persistantPath = Application.persistentDataPath + "/SaveData";
        readonly static string resourcesPath = Application.dataPath + "/Resources/SaveData";

        public static void Test()
        {
            LogUtil.Write(resourcesPath);
        }

        public static void Save(string directory, string filename, object data)
        {
            if (!Directory.Exists(directory))
            {
                try
                {
                    Directory.CreateDirectory(directory);
                }
                catch (IOException e)
                {
                    LogUtil.WriteException(e);
                    return;
                }
            }

            string filePath = directory + "/" + filename + ".txt";

            string dataString = "";
            try
            {
                dataString = JsonUtility.ToJson(data);
            }
            catch (Exception e)
            {
                LogUtil.WriteException(e);
            }

            try
            {
                File.WriteAllText(filePath, dataString);
            }
            catch (IOException e)
            {
                LogUtil.WriteException(e);
            }

            LogUtil.Write("Saved object " + data + " at " + filePath);
        }

        public static void SaveToPersistantData(string filename, object data)
        {
            Save(persistantPath, filename, data);
        }

        public static void SaveToResources(string filename, object data)
        {
            Save(resourcesPath, filename, data);
        }

        public static T Load<T>(string directory, string filename)
        {
            if (!Directory.Exists(directory))
            {
                try
                {
                    Directory.CreateDirectory(directory);
                }
                catch (IOException e)
                {
                    LogUtil.WriteException(e);
                    return default(T);
                }
            }

            string filePath = directory + "/" + filename + ".txt";

            string dataString;
            try
            {
                dataString = File.ReadAllText(filePath);
            }
            catch (IOException e)
            {
                LogUtil.WriteException(e);
                return default(T);
            }

            T dataObj = default(T);
            try
            {
                dataObj = JsonUtility.FromJson<T>(dataString);
            }
            catch (Exception e)
            {
                LogUtil.WriteException(e);
            }

            LogUtil.Write("Loaded object " + dataObj + " from " + filePath);

            return dataObj;
        }

        public static T LoadFromPersistantData<T>(string filename)
        {
            return Load<T>(persistantPath, filename);
        }

        public static T LoadFromResources<T>(string filename)
        {
            return Load<T>(resourcesPath, filename);
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