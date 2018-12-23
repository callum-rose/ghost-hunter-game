using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace Utils
{
    public class ClashCompressUtil
    {
        void Start()
        {
            classes = new List<ClashValue>();

            AddClass("a", 0, 5, 1);
            AddClass("b", 0, 3, 1);
            AddClass("c", 0, 4, 1);

            Debug.Log(Compress());
        }

        List<ClashValue> classes;
        int[] cumulativeRangeProducts;

        public void AddClass(String _name, int _rangeLow, int _rangeHigh, int _value)
        {
            var c = new ClashValue(_name, _rangeLow, _rangeHigh, _value);
            classes.Add(c);

            UpdateClassOrder();
            UpdateRangeProducts();
        }

        public int Compress()
        {
            int output = 0;

            for (int i = 0; i < classes.Count; i++)
            {
                output += classes[i].value * cumulativeRangeProducts[i];
            }

            return output;
        }

        public int[] Uncompress(int compressedValue)
        {
            int[] output = new int[classes.Count];

            //for (int i = 0; i < classes.Count; i++)
            //{
            //    output += classes[i].value * cumulativeRangeProducts[i];
            //}

            return output;
        }

        void UpdateClassOrder()
        {
            // sort in order of range
            classes = classes.OrderBy(p => p.Range).ToList();
            PrintEnumerable(classes);
        }

        void UpdateRangeProducts()
        {
            cumulativeRangeProducts = new int[classes.Count];
            cumulativeRangeProducts[0] = 1;
            for (int i = 1; i < classes.Count; i++)
            {
                cumulativeRangeProducts[i] = classes[i - 1].Range * cumulativeRangeProducts[i - 1];
            }

            PrintEnumerable(cumulativeRangeProducts);
        }

        void PrintEnumerable<T>(IEnumerable<T> enumerable)
        {
            String output = "";
            foreach (T value in enumerable)
            {
                output += value.ToString() + ", ";
            }

            Debug.Log(output);
        }

        class ClashValue
        {

            public int value;

            public readonly String name;
            readonly int rangeLow, rangeHigh;

            public int Range
            {
                get { return rangeHigh - rangeLow; }
            }

            public ClashValue(String _name, int _rangeLow, int _rangeHigh, int _value)
            {
                name = _name;
                rangeLow = _rangeLow;
                rangeHigh = _rangeHigh;
                value = _value;
            }

            public override String ToString()
            {
                return String.Format("Clash Class '{0}' - Range {1} - Value {2}", name, Range, value);
            }
        }
    }
}