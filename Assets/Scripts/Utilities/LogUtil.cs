using System;
using System.Diagnostics;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Utils
{
    public static class LogUtil
    {
        public static void Write(object message, bool withStackTrace)
        {
            StackFrame stackFrame = new StackFrame(1);
            MethodBase callerMethod = stackFrame.GetMethod();
            Type callingClass = callerMethod.DeclaringType;

            string messageString = "[" + DateTime.Now + "] "
                    + callingClass + "." + callerMethod.Name + "() -> "
                    + message;

            if (withStackTrace)
                messageString += " -> Stacktrace: " + new StackTrace();

            UnityEngine.Debug.Log(messageString);
        }

        public static void Write(object message)
        {
            Write(message, false);
        }

        public static void WriteWarning(object message)
        {
            UnityEngine.Debug.LogWarning("See Below");
            Write(" WARNING -> " + message, true);
        }

        public static void WriteError(object message)
        {
            UnityEngine.Debug.LogError("See Below");
            Write(" ERROR -> " + message, true);
        }

        public static void WriteException(Exception e)
        {
            UnityEngine.Debug.LogError("See Below");
            Write(" CAUGHT EXCEPTION -> " + e.Message + " -> Stacktrace: " + e.StackTrace);
        }

        public static void WriteEnumerable<T>(IEnumerable<T> enumerable)
        {
            WriteEnumerable("", enumerable);
        }

        public static void WriteEnumerable<T>(object message, IEnumerable<T> enumerable)
        {
            string startString = "L:" + enumerable.Count() + " -> [";

            StringBuilder sb = new StringBuilder(startString);

            foreach (var item in enumerable)
                sb.Append(item + ", ");

            // remove last comma+space
            sb.Remove(sb.Length - 2, 2);
            sb.Append("]");

            Write(message + sb.ToString());
        }
    }
}