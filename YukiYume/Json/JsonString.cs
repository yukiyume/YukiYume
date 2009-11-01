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
using System.Globalization;
using System.Text.RegularExpressions;

#endregion

namespace YukiYume.Json
{
    /// <summary>
    /// Represents a JSON string
    /// </summary>
    public class JsonString : IJsonValue
    {
        private static readonly Regex DoubleQuoteRegex = new Regex("\"", RegexOptions.Compiled);
        private static readonly Regex BackSlashRegex = new Regex("\\\\", RegexOptions.Compiled);

        /// <summary>
        /// implicit conversion from JsonString to string
        /// </summary>
        /// <param name="jsonString">JsonString to convert</param>
        /// <returns>string representing the JsonString</returns>
        public static implicit operator string(JsonString jsonString)
        {
            if (jsonString == null)
                throw new ArgumentNullException("jsonString");

            return jsonString.Value;
        }

        /// <summary>
        /// implicit conversion from string to JsonString
        /// </summary>
        /// <param name="value">string to convert</param>
        /// <returns>JsonString representing the string</returns>
        public static implicit operator JsonString(string value)
        {
            return new JsonString(value);
        }

        /// <summary>
        /// implicit conversion from char to JsonString
        /// </summary>
        /// <param name="value">char to convert</param>
        /// <returns>JsonString representation of the char</returns>
        public static implicit operator JsonString(char value)
        {
            return new JsonString(value);
        }

        /// <summary>
        /// string value of the JsonString
        /// </summary>
        public string Value { get; private set; }

        public JsonString()
        {
            Value = string.Empty;
        }

        public JsonString(string value)
        {
            Value = value;
        }

        public JsonString(char value)
        {
            Value = value.ToString();
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, @"""{0}""", string.IsNullOrEmpty(Value) ? string.Empty : DoubleQuoteRegex.Replace(BackSlashRegex.Replace(Value, "\\\\"), "\\\""));
        }
    }
}
