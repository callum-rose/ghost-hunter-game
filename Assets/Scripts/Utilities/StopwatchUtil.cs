using System.Diagnostics;
using System.Collections.Generic;
using System.Reflection;
using System;

namespace Utils
{
    public static class StopwatchUtil
    {

        static bool m_initialised;

        // for storing which method called 
        static Dictionary<MethodBase, Stopwatch> m_stopwatchByCallerDict;

        /// <summary>
        /// Starts a timer. The timer is linked to the calling method
        /// </summary>
        public static void Start()
        {
            if (!m_initialised)
                Init();

            // get calling frame
            StackFrame frame = new StackTrace().GetFrame(1);
            MethodBase method = frame.GetMethod();
            Type type = method.DeclaringType;

            if (m_stopwatchByCallerDict.ContainsKey(method))
            {
                LogUtil.WriteWarning("Cannot run more than one timer from "
                + type.Name + "." + method.Name);
                return;
            }

            CheckTimers();

            Stopwatch stopwatch = new Stopwatch();
            m_stopwatchByCallerDict.Add(method, stopwatch);
            stopwatch.Start();
        }

        public static void Stop()
        {
            if (!m_initialised)
                Init();

            // get calling frame
            StackFrame frame = new StackTrace().GetFrame(1);
            MethodBase method = frame.GetMethod();
            Type type = method.DeclaringType;

            if (m_stopwatchByCallerDict.ContainsKey(method))
            {
                Stopwatch stopwatch = m_stopwatchByCallerDict[method];
                stopwatch.Stop();
                LogUtil.Write("Timer started from " + type.Name + "." + method.Name
                    + " elapsed " + stopwatch.ElapsedMilliseconds + "ms.");

                m_stopwatchByCallerDict.Remove(method);
            }
            else
            {
                LogUtil.WriteWarning("No timer has been started from "
                    + type.Name + "." + method.Name);
            }

            CheckTimers();
        }

        static void CheckTimers()
        {
            foreach (var keyValuePair in m_stopwatchByCallerDict)
            {
                if (keyValuePair.Value.ElapsedMilliseconds > 10 * 1000)
                    LogUtil.WriteWarning("Timer started by " + keyValuePair.Key.DeclaringType.Name
                        + "." + keyValuePair.Key + " has been running for "
                        + keyValuePair.Value.ElapsedMilliseconds + "ms");
            }
        }

        static void Init()
        {
            m_stopwatchByCallerDict = new Dictionary<MethodBase, Stopwatch>();

            m_initialised = true;
        }
    }
}