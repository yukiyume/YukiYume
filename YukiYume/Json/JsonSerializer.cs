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
using System.Linq;
using System.Reflection;

#endregion

namespace YukiYume.Json
{
    /// <summary>
    /// Serializes an object to either a string or the intermediate form IJsonValue
    /// </summary>
    public static class JsonSerializer
    {
        /// <summary>
        /// Serializes an object to a string
        /// </summary>
        /// <param name="value">object to serialize</param>
        /// <returns>JSON string representation of the object</returns>
        public static string Serialize(object value)
        {
            return SerializeAsJsonValue(value).ToString();
        }

        /// <summary>
        /// Serializes an object to IJsonValue
        /// </summary>
        /// <param name="value">object to serialize</param>
        /// <returns>IJsonValue representation of the object</returns>
        public static IJsonValue SerializeAsJsonValue(object value)
        {
            IJsonValue serializedValue = Singleton<JsonNull>.Instance;

            if (value == null)
                return serializedValue;

            var type = value.GetType();

            if (!SerializationHelper.IsIgnore(type))
            {
                if (type.IsPrimitive)
                    serializedValue = SerializePrimitive(value, type);
                else if (Type.GetTypeCode(type) == TypeCode.String)
                    serializedValue = new JsonString(value as string);
                else if (value is System.Collections.IEnumerable)
                    serializedValue = SerializeEnumerable(value);
                else if (value is DateTime)
                    serializedValue = SerializeDateTime((DateTime)value);
                else if (type.IsClass || type.IsValueType)
                    serializedValue = SerializeClassOrValueType(value, type);
                //else not supported
            }

            return serializedValue;
        }

        /// <summary>
        /// Serializes a DateTime to JsonString
        /// </summary>
        /// <param name="value">DateTime to serialize</param>
        /// <returns>IJsonValue as a JsonString representation of the DateTime</returns>
        private static IJsonValue SerializeDateTime(DateTime value)
        {
            return new JsonString(((DateTimeOffset)value).ToString());
        }

        /// <summary>
        /// Serializes a class or value type to IJsonValue by setting
        /// its fields and properties where possible.
        /// Only fields that are public and do not have the JsonIgnore attribute will be written to
        /// </summary>
        /// <param name="value">class or value type to serialize</param>
        /// <param name="type">type of the class or value type</param>
        /// <returns>IJsonValue representation of the class or value type</returns>
        private static IJsonValue SerializeClassOrValueType(object value, Type type)
        {
            var jsonObject = new JsonObject();

            type.GetFields().Each(fieldInfo =>
            {
                if (fieldInfo.IsPublic && !SerializationHelper.IsIgnore(fieldInfo))
                    jsonObject.Members.Add(SerializationHelper.MemberName(fieldInfo), SerializeAsJsonValue(fieldInfo.GetValue(value)));
            });

            type.GetProperties().Each(propertyInfo =>
            {
                if (propertyInfo.PropertyType.IsPublic && propertyInfo.CanRead && !SerializationHelper.IsIgnore(propertyInfo))
                    jsonObject.Members.Add(SerializationHelper.MemberName(propertyInfo), SerializeAsJsonValue(propertyInfo.GetValue(value, null)));
            });

            return jsonObject;
        }

        /// <summary>
        /// Serializes an object of type IEnumerable (Arrays, Lists, Dictionaries, etc) to IJsonValue
        /// </summary>
        /// <param name="value">IEnumerable to serialize</param>
        /// <returns>IJsonValue representation as a JsonArray of the IEnumerable</returns>
        private static IJsonValue SerializeEnumerable(object value)
        {
            var jsonArray = new JsonArray();
            var enumerable = (System.Collections.IEnumerable)value;

            foreach (var o in enumerable)
                jsonArray.Values.Add(SerializeAsJsonValue(o));

            return jsonArray;
        }

        /// <summary>
        /// Serializes a primitive type to IJsonValue
        /// </summary>
        /// <param name="value">primitive type to serialize</param>
        /// <param name="type">Type of the primitive type</param>
        /// <returns>IJsonValue representation of the primitive type</returns>
        private static IJsonValue SerializePrimitive(object value, Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Int32:
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    return new JsonNumber(ConvertWrapper.ToInt64(value));

                case TypeCode.Double:
                case TypeCode.Single:
                    return new JsonNumber(ConvertWrapper.ToDouble(value));

                case TypeCode.Boolean:
                    return new JsonBoolean(ConvertWrapper.ToBoolean(value));

                case TypeCode.Char:
                    return new JsonString((char)value);

                default:
                    return Singleton<JsonNull>.Instance;
            }
        }
    }
}
