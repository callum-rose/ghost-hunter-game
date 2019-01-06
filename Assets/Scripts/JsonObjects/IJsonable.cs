using System.Reflection;
using System;
using Utils;

public interface IJsonable {

    /// <summary>
    /// Checks all properties are valid e.g. reference types are non-<see langword="null"/> 
    /// and enumerables have length greater than 0.
    /// </summary>
    bool CheckPropertiesValid();
}