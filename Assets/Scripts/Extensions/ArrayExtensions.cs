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

        public static bool HasIndex<T>(this T[,] array, int index0, int index1)
        {
            bool hasD0Index = index0 >= 0 && index0 < array.GetLength(0);
            bool hasD1Index = index1 >= 0 && index1 < array.GetLength(1);

            return hasD0Index && hasD1Index;
        }

        /// <summary>
        /// Return the index of an element in this array.
        /// </summary>
        public static int IndexOf<T>(this T[] array, T item)
        {
            for (int i = 0; i < array.Length; i++)
                if (item.Equals(array[i]))
                    return i;
            return -1;
        }
    }
}