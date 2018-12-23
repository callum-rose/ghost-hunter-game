using System.Collections.Generic;
using System;
using System.Linq;

namespace CustomExtensions
{
    public static class ArrayExtensions
    {

        public static T[] Shuffle<T>(this T[] array)
        {
            Random rnd = new Random();
            return array.OrderBy(item => rnd.Next()).ToArray();
        }

        /// <summary>
        /// Returns value at the index of the remainder of the index by the array length.
        /// </summary>
        public static T GetRemainderEntry<T>(this T[] array, int index)
        {
            int remainderIndex = index % array.Length;
            return array[remainderIndex];
        }

        public static T GetRandom<T>(this T[] array)
        {
            int index = UnityEngine.Random.Range(0, array.Length);
            return array[index];
        }

        public static bool HasIndex<T>(this T[] array, int index)
        {
            return index >= 0 && index < array.Length;
        }
    }
}