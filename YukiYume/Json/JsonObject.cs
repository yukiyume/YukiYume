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
using System.Linq;
using System.Text;

#endregion

namespace YukiYume.Json
{
    /// <summary>
    /// Represents a JSON object
    /// </summary>
    public class JsonObject : IJsonValue, IEnumerable<KeyValuePair<string, IJsonValue>>
    {
        private readonly Dictionary<string, IJsonValue> _members = new Dictionary<string, IJsonValue>();

        public Dictionary<string, IJsonValue> Members
        {
            get { return _members; }
        }

        /// <summary>
        /// indexer for the JsonObject
        /// </summary>
        /// <param name="index">string index (key) to access the JsonObject</param>
        /// <returns>IJsonValue at the index if it exists, null otherwise</returns>
        public IJsonValue this[string index]
        {
            get { return Members.ContainsKey(index) ? Members[index] : null; }

            set
            {
                if (Members.ContainsKey(index))
                    Members[index] = value;
                else
                    Members.Add(index, value);
            }
        }

        public override string ToString()
        {
            var jsonBuilder = new StringBuilder("{ ");

            for (var i = 0; i < Members.Count; ++i)
            {
                var pair = Members.ElementAt(i);

                jsonBuilder.AppendFormat(@"""{0}"" : {1}", pair.Key, pair.Value);

                if (i < Members.Count - 1)
                    jsonBuilder.Append(", ");
            }

            jsonBuilder.Append(" }");

            return jsonBuilder.ToString();
        }

        #region IEnumerable<KeyValuePair<string,IJsonValue>> Members

        public IEnumerator<KeyValuePair<string, IJsonValue>> GetEnumerator()
        {
            return Members.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Members.GetEnumerator();
        }

        #endregion
    }
}
