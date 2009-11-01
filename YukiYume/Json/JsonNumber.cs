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

#endregion

namespace YukiYume.Json
{
    public class JsonNumber : IJsonValue
    {
        private long? _int64Value;
        private double? _doubleValue;

        public static implicit operator long(JsonNumber jsonNumber)
        {
            if (jsonNumber == null)
                throw new ArgumentNullException("jsonNumber");
            else if (jsonNumber.Int64Value.HasValue)
                return jsonNumber.Int64Value.Value;
            else if (jsonNumber.DoubleValue.HasValue)
                return ConvertWrapper.ToInt64(jsonNumber.DoubleValue.Value);
            else
                return default(long);
        }

        public static implicit operator int(JsonNumber jsonNumber)
        {
            if (jsonNumber == null)
                throw new ArgumentNullException("jsonNumber");
            else if (jsonNumber.Int64Value.HasValue)
                return ConvertWrapper.ToInt32(jsonNumber.Int64Value.Value);
            else if (jsonNumber.DoubleValue.HasValue)
                return ConvertWrapper.ToInt32(jsonNumber.DoubleValue.Value);
            else
                return default(int);
        }

        public static implicit operator double(JsonNumber jsonNumber)
        {
            if (jsonNumber == null)
                throw new ArgumentNullException("jsonNumber");
            else if (jsonNumber.DoubleValue.HasValue)
                return jsonNumber.DoubleValue.Value;
            else
                return jsonNumber.Int64Value ?? default(double);
        }

        public static implicit operator JsonNumber(int number)
        {
            return new JsonNumber(number);
        }

        public static implicit operator JsonNumber(double number)
        {
            return new JsonNumber(number);
        }

        public static implicit operator JsonNumber(long number)
        {
            return new JsonNumber(number);
        }

        public long? Int64Value
        {
            get
            {
                return !_int64Value.HasValue && !_doubleValue.HasValue ? 0 : _int64Value;
            }

            set
            {
                _int64Value = value;

                if (value != null)
                    _doubleValue = null;
            }
        }

        public double? DoubleValue
        {
            get
            {
                return _doubleValue;
            }

            set
            {
                _doubleValue = value;

                if (value != null)
                    _int64Value = null;
            }
        }

        public object Value
        {
            get
            {
                if (Int64Value.HasValue)
                    return Int64Value.Value;
                else if (DoubleValue.HasValue)
                    return DoubleValue.Value;
                else
                    return null;
            }
        }

        public JsonNumber()
        {
        }

        public JsonNumber(int value)
        {
            Int64Value = value;
        }

        public JsonNumber(long value)
        {
            Int64Value = value;
        }

        public JsonNumber(double value)
        {
            DoubleValue = value;
        }

        public void Set(int value)
        {
            Int64Value = value;
        }

        public void Set(long value)
        {
            Int64Value = value;
        }

        public void Set(double value)
        {
            DoubleValue = value;
        }

        public override string ToString()
        {
            return Int64Value.HasValue ? Int64Value.Value.ToString(CultureInfo.CurrentCulture) : DoubleValue.Value.ToString(CultureInfo.CurrentCulture);
        }
    }
}
