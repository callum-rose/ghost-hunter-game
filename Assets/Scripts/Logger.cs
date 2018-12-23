using System;
using System.Diagnostics;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.Text;

public static class Logger {

    public static void Write(object message)
    {
        StackFrame stackFrame = new StackFrame(1);
        MethodBase callerMethod = stackFrame.GetMethod();
        Type callingClass = callerMethod.DeclaringType;

        UnityEngine.Debug.Log("[" + DateTime.Now + "] "
                + callingClass + "." + callerMethod.Name + "() -> " 
                + message);
    }

    public static void WriteWarning(object message)
    {
        UnityEngine.Debug.LogWarning("See Below");
        Write(" WARNING -> " + message);
    }

    public static void WriteError(object message)
    {
        UnityEngine.Debug.LogError("See Below");
        Write(" ERROR -> " + message);
    }

    public static void WriteEnumerable<T> (IEnumerable<T> enumerable)
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