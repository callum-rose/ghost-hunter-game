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

        /// <summary>
        /// Put this float inside the range specified. I.e. -20 put into the range 0 to 50 would return 30.
        /// </summary>
        public static float Rangeify(this float number, float min, float max)
        {
            float span = max - min;
            int numTimesOutside = Mathf.FloorToInt(number / span);
            return number -= span * numTimesOutside;
        }
    }
}