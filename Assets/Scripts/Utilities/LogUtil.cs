using System;
using System.Diagnostics;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.IO;
using System.IO.Compression;

namespace Utils
{
    public static class LogUtil
    {
        static readonly string fileDirectory = Application.persistentDataPath + "/Logs";

        static readonly int numFramesShownInStackTrace = 20;

        public static void Write(object message, bool withStackTrace)
        {
            StackFrame stackFrame = GetFirstFrameOutside();
            MethodBase callerMethod = stackFrame.GetMethod();
            Type callingClass = callerMethod.ReflectedType;

            StringBuilder messageSB = new StringBuilder("[" 
                    + TimeUtil.GetNiceDateFormat(DateTime.Now) + " "
                    + TimeUtil.GetNiceTimeFormat(DateTime.Now) 
                    + "] "
                    + callingClass + "." + callerMethod.Name + "() -> "
                    + message);

            UnityEngine.Debug.Log(messageSB.ToString());

            if (withStackTrace)
            {
                messageSB.Append(" -> Stacktrace:");

                StackTrace stackTrace = new StackTrace();
                List<StackFrame> stackFrames = GetAllFramesOutside();
                for (int f = 0; f < Mathf.Min(stackFrames.Count, numFramesShownInStackTrace); f++)
                {
                    StackFrame frame = stackFrames[f];
                    messageSB.Append(string.Format("\n\t{0}: {1}", f, FormatStackFrame(frame)));
                }
            }

            WriteToFile(messageSB.ToString());
        }

        static void WriteToFile(string message)
        {
            if (!Directory.Exists(fileDirectory))
            {
                try
                {
                    Directory.CreateDirectory(fileDirectory);
                }
                catch (Exception e)
                {
                    return;
                }
            }

            string filename = "LOG_" + TimeUtil.GetNiceDateFormat(DateTime.Now);
            string filePath = fileDirectory + "/" + filename + ".txt";

            try
            {
                File.AppendAllText(filePath, message + "\n");
            }
            catch (Exception e)
            {
            }
        }

        public static void Write(object message)
        {
            Write(message, false);
        }

        public static void WriteWarning(object message)
        {
            UnityEngine.Debug.LogWarning("See Below");
            Write("WARNING -> " + message, true);
        }

        public static void WriteError(object message)
        {
            UnityEngine.Debug.LogError("See Below");
            Write("ERROR -> " + message, true);
        }

        public static void WriteException(Exception e)
        {
            UnityEngine.Debug.LogError("See Below");
            Write("CAUGHT EXCEPTION -> " + e.Message, true);
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

        static StackFrame GetFirstFrameOutside()
        {
            StackTrace stackTrace = new StackTrace();
            for (int i = 0; i < stackTrace.FrameCount; i++)
            {
                StackFrame frame = stackTrace.GetFrame(i);
                if (frame.GetMethod().ReflectedType != typeof(LogUtil))
                    return frame;
            }
            return stackTrace.GetFrame(0);
        }

        static List<StackFrame> GetAllFramesOutside()
        {
            List<StackFrame> stackFrames = new List<StackFrame>();

            StackTrace stackTrace = new StackTrace();
            for (int i = 0; i < stackTrace.FrameCount; i++)
            {
                StackFrame frame = stackTrace.GetFrame(i);
                if (stackFrames.Count > 0 || frame.GetMethod().ReflectedType != typeof(LogUtil))
                    stackFrames.Add(frame);
            }
            return stackFrames;
        }

        static string FormatStackFrame(StackFrame frame)
        {
            // frame was called outside the logger so log it!
            var method = frame.GetMethod();

            string argTypes = "(";
            int numArgs = method.GetGenericArguments().Length;
            for (int i = 0; i < numArgs; i++)
                argTypes += method.GetGenericArguments()[i].Name + (i < numArgs - 1 ? ", " : "");
            argTypes += ")";

            string frameInfo = string.Format("{0}:{1}{2}",
                method.ReflectedType.FullName,
                method.Name,
                argTypes);

            return frameInfo;
        }
    }
}