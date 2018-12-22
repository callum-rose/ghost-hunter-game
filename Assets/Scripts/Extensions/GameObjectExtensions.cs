using UnityEngine;
using System;
using System.Reflection;

namespace CustomExtensions
{
    public static class GameObjectExtensions
    {

        public static T GetCopyOf<T>(this Component comp, T other) where T : Component
        {
            Type type = comp.GetType();
            if (type != other.GetType())
                return null;

            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;

            PropertyInfo[] pinfos = type.GetProperties(flags);
            foreach (var pinfo in pinfos)
            {
                if (pinfo.CanWrite)
                {
                    try
                    {
                        pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                    }
                    catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
                }
            }
            FieldInfo[] finfos = type.GetFields(flags);
            foreach (var finfo in finfos)
                finfo.SetValue(comp, finfo.GetValue(other));

            return comp as T;
        }

        public static void RemoveAllComponentsExcept(this GameObject gameObject, params Type[] exemptComponents)
        {
            MonoBehaviour[] monos = gameObject.GetComponents<MonoBehaviour>();
            foreach (var mono in monos)
            {
                for (int i = 0; i < exemptComponents.Length; i++)
                {
                    if (mono.GetType() == exemptComponents[i])
                        break;
                    if (i == exemptComponents.Length - 1)
                        GameObject.Destroy(mono);
                }
            }
        }

        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            if (component == null)
                component = gameObject.AddComponent<T>();
            return component;
        }
    }
}