using System.Collections.Generic;
using System;
using System.Linq;

namespace CustomExtensions
{
    public static class ListExtensions
    {

        public static List<T> Shuffle<T>(this List<T> list)
        {
            Random rnd = new Random();
            return list.OrderBy(item => rnd.Next()).ToList();
        }

        /// <summary>
        /// Returns value at the index of the remainder of the index by the list length.
        /// </summary>
        public static T GetRemainderEntry<T>(this List<T> list, int index)
        {
            int remainderIndex = index % list.Count;
            return list[remainderIndex];
        }

        public static T GetRandom<T>(this List<T> list)
        {
            int index = UnityEngine.Random.Range(0, list.Count);
            return list[index];
        }
    }
}