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
using System.Text;
using System.Text.RegularExpressions;

#endregion

namespace YukiYume
{
    /// <summary>
    /// Provides wrapper methods with Exception handling to System.Convert methods as well as additional conversion methods
    /// </summary>
    public static class ConvertWrapper
    {
        /// <summary>
        /// short (Int16) constant zero
        /// </summary>
        private const short ShortZero = 0;

        /// <summary>
        /// Characters that are allowed in hexadecimal
        /// </summary>
        private static readonly Regex HexDigits = new Regex("^[A-Fa-f0-9]+$", RegexOptions.Compiled);

        /// <summary>
        /// Converts a byte array to a hexadecimal string representation
        /// </summary>
        /// <param name="data">the byte array to convert</param>
        /// <returns>string containing the the hexadecimal values of the byte array</returns>
        public static string ToHexString(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            var hexBuilder = new StringBuilder();

            for (var index = 0; index < data.Length; ++index)
            {
                var hexString = Convert.ToString(data[index], 16).ToUpper();

                if (hexString.Length == 1)
                    hexBuilder.Append("0" + hexString);
                else
                    hexBuilder.Append(hexString);
            }

            return hexBuilder.ToString();
        }

        /// <summary>
        /// Converts a hexadecimal string representation to a byte array
        /// </summary>
        /// <param name="input">the string to convert</param>
        /// <returns>a byte array of the values in the input string</returns>
        public static byte[] ToByteArray(string input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // input length should be even
            if ((input.Length & 0x01) == 0x01)
                throw new ArgumentException("invalid input length", "input");

            if (!HexDigits.IsMatch(input))
                throw new ArgumentException("input contains invalid characters", "input");

            if (input.Length == 0)
                throw new ArgumentException("input should not be empty", "input");

            var data = new byte[input.Length >> 1];

            for (var i = 0; i < data.Length; ++i)
                data[i] = Convert.ToByte(input.Substring(i << 1, 2), 0x10);

            return data;
        }

        /// <summary>
        /// Converts a string to short
        /// </summary>
        /// <param name="value">string representation of the short</param>
        /// <returns>short value of the string, 0 if the conversion fails</returns>
        public static short ToInt16(string value)
        {
            return ConvertWithDefault(value, x => string.IsNullOrEmpty(x) ? ShortZero : Convert.ToInt16(x, CultureInfo.CurrentCulture), ShortZero);
        }

        /// <summary>
        /// Converts an object to short (unboxes)
        /// </summary>
        /// <param name="value">object representation of the short</param>
        /// <returns>short value of the object, 0 if the conversion fails</returns>
        public static short ToInt16(object value)
        {
            return ConvertWithDefault(value, x => x == null ? ShortZero : Convert.ToInt16(x, CultureInfo.CurrentCulture), ShortZero);
        }

        /// <summary>
        /// Converts an long to short (unboxes)
        /// </summary>
        /// <param name="value">long representation of the short</param>
        /// <returns>short value of the long, 0 if the conversion fails</returns>
        public static short ToInt16(long value)
        {
            return ConvertWithDefault(value, x => Convert.ToInt16(x, CultureInfo.CurrentCulture), ShortZero);
        }

        /// <summary>
        /// Converts a string to int
        /// </summary>
        /// <param name="value">string representation of the int</param>
        /// <returns>int value of the string, 0 if the conversion fails</returns>
        public static int ToInt32(string value)
        {
            return ConvertWithDefault(value, x => string.IsNullOrEmpty(x) ? 0 : Convert.ToInt32(x, CultureInfo.CurrentCulture), 0);
        }

        /// <summary>
        /// Converts a string to int
        /// </summary>
        /// <param name="value">string representation of the int</param>
        /// <param name="fromBase">base to convert from</param>
        /// <returns>int value of the string, 0 if the conversion fails</returns>
        public static int ToInt32(string value, int fromBase)
        {
            return ConvertWithDefault(value, x => string.IsNullOrEmpty(x) ? 0 : Convert.ToInt32(x, fromBase), 0);
        }

        /// <summary>
        /// Converts an object to int (unboxes)
        /// </summary>
        /// <param name="value">object representation of the int</param>
        /// <returns>int value of the object, 0 if the conversion fails</returns>
        public static int ToInt32(object value)
        {
            return ConvertWithDefault(value, x => x == null ? 0 : Convert.ToInt32(x, CultureInfo.CurrentCulture), 0);
        }

        /// <summary>
        /// Converts an long to int (unboxes)
        /// </summary>
        /// <param name="value">long representation of the int</param>
        /// <returns>int value of the long, 0 if the conversion fails</returns>
        public static int ToInt32(long value)
        {
            return ConvertWithDefault(value, x => Convert.ToInt32(x, CultureInfo.CurrentCulture), 0);
        }

        /// <summary>
        /// Converts an double to int (unboxes)
        /// </summary>
        /// <param name="value">double representation of the int</param>
        /// <returns>int value of the double, 0 if the conversion fails</returns>
        public static int ToInt32(double value)
        {
            return ConvertWithDefault(value, x => Convert.ToInt32(x, CultureInfo.CurrentCulture), 0);
        }

        /// <summary>
        /// Converts string to long
        /// </summary>
        /// <param name="value">string representation of the long</param>
        /// <returns>long value of the string, 0 if the conversion fails</returns>
        public static long ToInt64(string value)
        {
            return ConvertWithDefault(value, x => string.IsNullOrEmpty(x) ? 0L : Convert.ToInt64(x, CultureInfo.CurrentCulture), 0L);
        }

        /// <summary>
        /// Converts an object to long (unboxes)
        /// </summary>
        /// <param name="value">object representation of the long</param>
        /// <returns>long value of the object, 0 if the conversion fails</returns>
        public static long ToInt64(object value)
        {
            return ConvertWithDefault(value, x => x == null ? 0L : Convert.ToInt64(x, CultureInfo.CurrentCulture), 0L);
        }

        /// <summary>
        /// Converts a double to long (unboxes)
        /// </summary>
        /// <param name="value">double representation of the long</param>
        /// <returns>long value of the double, 0 if the conversion fails</returns>
        public static long ToInt64(double value)
        {
            return ConvertWithDefault(value, x => Convert.ToInt64(x, CultureInfo.CurrentCulture), 0L);
        }

        /// <summary>
        /// Converts string to float
        /// </summary>
        /// <param name="value">string representation of the float</param>
        /// <returns>float value of the string, 0 if the conversion fails</returns>
        public static float ToSingle(string value)
        {
            return ConvertWithDefault(value, x => string.IsNullOrEmpty(x) ? 0.0f : Convert.ToSingle(x, CultureInfo.CurrentCulture), 0.0f);
        }

        /// <summary>
        /// Converts an object to float (unboxes)
        /// </summary>
        /// <param name="value">object representation of the float</param>
        /// <returns>float value of the object, 0 if the conversion fails</returns>
        public static float ToSingle(object value)
        {
            return ConvertWithDefault(value, x => x == null ? 0.0f : Convert.ToSingle(x, CultureInfo.CurrentCulture), 0.0f);
        }

        /// <summary>
        /// Converts an double to float (unboxes)
        /// </summary>
        /// <param name="value">double representation of the float</param>
        /// <returns>float value of the double, 0 if the conversion fails</returns>
        public static float ToSingle(double value)
        {
            return ConvertWithDefault(value, x => Convert.ToSingle(x, CultureInfo.CurrentCulture), 0.0f);
        }

        /// <summary>
        /// Converts a string to double
        /// </summary>
        /// <param name="value">string representation of the double</param>
        /// <returns>double value of the string, 0 if the conversion fails</returns>
        public static double ToDouble(string value)
        {
            return ConvertWithDefault(value, x => string.IsNullOrEmpty(x) ? 0.0d : Convert.ToDouble(x, CultureInfo.CurrentCulture), 0.0d);
        }

        /// <summary>
        /// Converts an object to double (unboxes)
        /// </summary>
        /// <param name="value">object representation of the double</param>
        /// <returns>double value of the object, 0 if the conversion fails</returns>
        public static double ToDouble(object value)
        {
            return ConvertWithDefault(value, x => x == null ? 0.0d : Convert.ToDouble(x, CultureInfo.CurrentCulture), 0.0d);
        }

        /// <summary>
        /// Converts a string to boolean
        /// </summary>
        /// <param name="value">string representation of the boolean</param>
        /// <returns>boolean value of the string, false if the conversion fails</returns>
        public static bool ToBoolean(string value)
        {
            return ConvertWithDefault(value, x => string.IsNullOrEmpty(x) ? false : Convert.ToBoolean(x, CultureInfo.CurrentCulture), false);
        }

        /// <summary>
        /// Converts an object to boolean
        /// </summary>
        /// <param name="value">object representation of the boolean</param>
        /// <returns>boolean value of the object, false if the conversion fails</returns>
        public static bool ToBoolean(object value)
        {
            return ConvertWithDefault(value, x => x == null ? false : Convert.ToBoolean(x, CultureInfo.CurrentCulture), false);
        }

        /// <summary>
        /// Converts a string to Guid
        /// </summary>
        /// <param name="value">string representation of the Guid</param>
        /// <returns>Guid value of the object, Guid.Empty if the conversion fails</returns>
        public static Guid ToGuid(string value)
        {
            return ConvertWithDefault(value, x => string.IsNullOrEmpty(x) ? Guid.Empty : new Guid(x), Guid.Empty);
        }

        /// <summary>
        /// Converts an object to Guid
        /// </summary>
        /// <param name="value">object representation of the Guid (assumed to actually be a string)</param>
        /// <returns>Guid value of the object, Guid.Empty if the conversion fails</returns>
        public static Guid ToGuid(object value)
        {
            return ConvertWithDefault(value, x => x == null ? Guid.Empty : new Guid(x.ToString()), Guid.Empty);
        }

        /// <summary>
        /// Converts from one type to another using the delegate parameter to do the conversion,
        /// and catching any Exceptions
        /// </summary>
        /// <typeparam name="TArg">type of the value being converted from</typeparam>
        /// <typeparam name="TResult">type of the value being converted to</typeparam>
        /// <param name="value">value to convert</param>
        /// <param name="convertFunc">delegate that does the actual conversion</param>
        /// <param name="defaultValue">default value if conversion fails</param>
        /// <returns>the value converted to type T1</returns>
        private static TResult ConvertWithDefault<TArg, TResult>(TArg value, Func<TArg, TResult> convertFunc, TResult defaultValue)
        {
            TResult convertedValue = defaultValue;

            try
            {
                convertedValue = convertFunc(value);
            }
            catch (FormatException)
            {
            }
            catch (OverflowException)
            {
            }
            catch (InvalidCastException)
            {
            }

            return convertedValue;
        }
    }
}
