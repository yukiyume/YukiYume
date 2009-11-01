#region MIT License

/*
 * Copyright (c) 2009 Kristopher Baker (ao@yukiyume.net)
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

#endregion

#region using

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

#endregion

namespace YukiYume.Json
{
    /// <summary>
    /// Represents a JSON array
    /// </summary>
    public class JsonArray : IJsonValue, IEnumerable<IJsonValue>
    {
        private readonly List<IJsonValue> _values = new List<IJsonValue>();

        public List<IJsonValue> Values
        {
            get { return _values; }
        }

        /// <summary>
        /// indexer for the JsonArray
        /// </summary>
        /// <param name="index">index of the value to access</param>
        /// <returns>IJsonValue at the index</returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public IJsonValue this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new IndexOutOfRangeException();

                return Values[index];
            }

            set 
            {
                if (index < 0 || index >= Count)
                    throw new IndexOutOfRangeException();

                Values[index] = value; 
            }
        }

        /// <summary>
        /// explicit conversion from JsonArray to List&lt;int&gt;
        /// </summary>
        /// <param name="intArray">JsonArray to convert</param>
        /// <returns>List&lt;int&gt; representation of the JsonArray</returns>
        public static explicit operator List<int>(JsonArray intArray)
        {
            var r = new List<int>();

            intArray.Each(x =>
            {
                if (x is JsonNumber)
                    r.Add((JsonNumber)x);
            });

            return r;
        }

        /// <summary>
        /// explicit conversion from JsonArray to List&lt;double&gt;
        /// </summary>
        /// <param name="doubleArray">JsonArray to convert</param>
        /// <returns>List&lt;double&gt; representation of the JsonArray</returns>
        public static explicit operator List<double>(JsonArray doubleArray)
        {
            var r = new List<double>();

            doubleArray.Each(x =>
            {
                if (x is JsonNumber)
                    r.Add((JsonNumber)x);
            });

            return r;
        }

        /// <summary>
        /// explicit conversion from JsonArray to List&lt;long&gt;
        /// </summary>
        /// <param name="longArray">JsonArray to convert</param>
        /// <returns>List&lt;long&gt; representation of the JsonArray</returns>
        public static explicit operator List<long>(JsonArray longArray)
        {
            var r = new List<long>();

            longArray.Each(x =>
            {
                if (x is JsonNumber)
                    r.Add((JsonNumber)x);
            });

            return r;
        }

        /// <summary>
        /// explicit conversion from JsonArray to List&lt;string&gt;
        /// </summary>
        /// <param name="stringArray">JsonArray to convert</param>
        /// <returns>List&lt;string&gt; representation of the JsonArray</returns>
        public static explicit operator List<string>(JsonArray stringArray)
        {
            var r = new List<string>();

            stringArray.Each(x =>
            {
                if (x is JsonString)
                    r.Add((JsonString)x);
            });

            return r;
        }

        /// <summary>
        /// Number of elements in the JsonArray
        /// </summary>
        public int Count
        {
            get { return _values.Count; }
        }

        public override string ToString()
        {
            var jsonBuilder = new StringBuilder("[");

            for (var i = 0; i < Values.Count; ++i)
            {
                if (i < Values.Count - 1)
                {
                    jsonBuilder.AppendFormat(CultureInfo.CurrentCulture, "{0}, ", Values[i]);
                }
                else
                {
                    jsonBuilder.Append(Values[i]);
                }
            }

            jsonBuilder.Append("]");

            return jsonBuilder.ToString();
        }

        #region IEnumerable<IJsonValue> Members

        public IEnumerator<IJsonValue> GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        #endregion
    }
}
