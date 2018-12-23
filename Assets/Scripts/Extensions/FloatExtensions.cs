using UnityEngine;

namespace CustomExtensions
{
    public static class FloatExtensions
    {

        public static bool Equals(this float number, float otherNumber)
        {
            return Mathf.Abs(number - otherNumber) < float.Epsilon;
        }

        public static bool RoughlyEquals(this float number, float otherNumber, int numDecimalPlaces)
        {
            float d = Mathf.Pow(10f, numDecimalPlaces);
            return Mathf.RoundToInt(number * d) == Mathf.RoundToInt(otherNumber * d);
        }
    }
}