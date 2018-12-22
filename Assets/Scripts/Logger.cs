using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;

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

    public static void WriteEnumerable (IEnumerable enumerable)
    {
        string enumerableString = "[";
        foreach (var item in enumerable)
        {
            enumerableString += item + ",";
        }
        enumerableString = enumerableString.Remove(enumerableString.Length - 1);
        enumerableString += "]";

        Write(enumerableString);
    }

    public static void WriteEnumerable(object message, IEnumerable enumerable)
    {
        string enumerableString = "[";
        foreach (var item in enumerable)
        {
            enumerableString += item + ",";
        }
        enumerableString = enumerableString.Remove(enumerableString.Length - 1);
        enumerableString += "]";

        Write(message + enumerableString);
    }
}