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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

#endregion

namespace YukiYume.Json
{
    /// <summary>
    /// Provides functionality to deserialize json strings to objects
    /// </summary>
    public static class JsonDeserializer
    {
        /// <summary>
        /// Deserializes the input json string to an object of type T
        /// </summary>
        /// <typeparam name="T">type of the object to deserialize to</typeparam>
        /// <param name="json">json string</param>
        /// <returns>an object of type T if deserialization is successful, null otherwise</returns>
        public static T Deserialize<T>(string json) where T : class
        {
            if (string.IsNullOrEmpty(json))
                return null;

            var jsonValue = JsonParser.Parse(json);

            T deserialized = default(T);

            if (jsonValue is JsonObject)
                deserialized = DeserializeObject((JsonObject)jsonValue, typeof(T)) as T;
            else if (jsonValue is JsonArray)
                deserialized = DeserializeEnumerable((JsonArray)jsonValue, typeof(T)) as T;

            return deserialized;
        }

        /// <summary>
        /// Deserializes arrays, generic ICollections, generic Dictionaries, and non-generic ICollections
        /// </summary>
        /// <param name="jsonArray"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static object DeserializeEnumerable(JsonArray jsonArray, Type type)
        {
            if (type.IsArray)
                return DeserializeArray(jsonArray, type);
            else if (type.IsGenericType)
            {
                var genericTypeDefinition = type.GetGenericTypeDefinition();

                if (IsBaseType(genericTypeDefinition, typeof(IDictionary<,>).GetGenericTypeDefinition()))
                    return DeserializeGenericDictionary(jsonArray, type);
                else if (IsBaseType(genericTypeDefinition, typeof(ICollection<>).GetGenericTypeDefinition()) || genericTypeDefinition.Equals(typeof(IEnumerable<>)))
                    return DeserializeGenericCollection(jsonArray, type);
            }
            else if (IsBaseType(type, typeof(ICollection)))
                return DeserializeCollection(jsonArray, type);

            return null;
        }

        /// <summary>
        /// Checks to see if type implements or inherits from baseType
        /// </summary>
        /// <param name="type">type to check</param>
        /// <param name="baseType">type to check against</param>
        /// <returns>true iff type implements or inherits from baseType</returns>
        private static bool IsBaseType(Type type, Type baseType)
        {
            if (type.Equals(baseType))
                return true;

            var interfaces = type.GetInterfaces();

            foreach (var @interface in interfaces)
            {
                if (((@interface.IsGenericType || @interface.IsGenericTypeDefinition) && @interface.GetGenericTypeDefinition().Equals(baseType)) ||
                    (@interface.Equals(baseType)))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Deserializes a generic Dictionary according to type, using jsonArray for members
        /// The generic Dictionary must have an "Add" method that takes two parameters,
        /// the Key and Value.
        /// </summary>
        /// <param name="jsonArray">JsonArray of JsonObjects to fill the Dictionary with, 
        /// objects should have two members, "Key", and "Value"</param>
        /// <param name="type">type of the generic Dictionary</param>
        /// <returns>a new generic Dictionary according to type, null if creation is unsuccessful</returns>
        private static object DeserializeGenericDictionary(JsonArray jsonArray, Type type)
        {
            var genericArguments = type.GetGenericArguments();
            if (genericArguments == null || genericArguments.Length != 2)
                return null;

            var genericTypeDefinition = type.GetGenericTypeDefinition();

            if (genericTypeDefinition.Equals(typeof(IDictionary<,>)))
                type = typeof(Dictionary<,>).MakeGenericType(genericArguments);

            var instance = Activator.CreateInstance(type);

            if (instance != null)
            {
                var addMethod = type.GetMethod("Add");
                if (addMethod == null)
                    return null;

                for (var index = 0; index < jsonArray.Count; ++index)
                {
                    var keyValue = jsonArray[index] as JsonObject;

                    if (keyValue == null || !keyValue.Members.ContainsKey("Key"))
                        continue;

                    addMethod.Invoke(instance, new[] { DeserializeJsonValue(keyValue["Key"], genericArguments[0]), DeserializeJsonValue(keyValue["Value"], genericArguments[1]) });
                }
            }

            return instance;
        }

        /// <summary>
        /// Deserializes a generic ICollection (such as System.Collections.Generic.List)
        /// The generic ICollection must have an "Add" method that takes one parameter.
        /// </summary>
        /// <param name="jsonArray">JsonArray of elements to fill the collection with.</param>
        /// <param name="type">type of the generic ICollection</param>
        /// <returns>a new generic ICollection according to type, null if creation is unsuccessful</returns>
        private static object DeserializeGenericCollection(JsonArray jsonArray, Type type)
        {
            var genericArguments = type.GetGenericArguments();
            if (genericArguments == null || genericArguments.Length != 1)
                return null;

            var genericTypeDefinition = type.GetGenericTypeDefinition();

            if (genericTypeDefinition.Equals(typeof(IList<>)) || genericTypeDefinition.Equals(typeof(ICollection<>)) || genericTypeDefinition.Equals(typeof(IEnumerable<>)))
                type = typeof(List<>).MakeGenericType(genericArguments); ;
            
            var instance = Activator.CreateInstance(type);

            if (instance != null)
            {
                var addMethod = type.GetMethod("Add");
                if (addMethod == null)
                    return null;

                for (var index = 0; index < jsonArray.Count; ++index)
                    addMethod.Invoke(instance, new[] { DeserializeJsonValue(jsonArray[index], genericArguments[0]) });
            }

            return instance;
        }

        /// <summary>
        /// Deserializes a non-generic ICollection
        /// The ICollection must have an "Add" method that takes one parameter.
        /// </summary>
        /// <param name="jsonArray">JsonArray of elements to fill the collection with.</param>
        /// <param name="type">type of the non-generic ICollection</param>
        /// <returns>a new non-generic ICollection according to type, null if creation is unsuccessful</returns>
        private static object DeserializeCollection(JsonArray jsonArray, Type type)
        {
            if (type.Equals(typeof(IList)) || type.Equals(typeof(ICollection)))
                type = typeof(ArrayList);

            var instance = Activator.CreateInstance(type);

            if (instance != null)
            {
                var addMethod = type.GetMethod("Add");
                if (addMethod == null)
                    return null;

                for (var index = 0; index < jsonArray.Count; ++index)
                    addMethod.Invoke(instance, new[] { DeserializeJsonValue(jsonArray[index], null) });
            }

            return instance;
        }

        /// <summary>
        /// Deserializes an array.
        /// </summary>
        /// <param name="jsonArray">JsonArray of elements to fill the array with.</param>
        /// <param name="type">type of the array</param>
        /// <returns>a new arraw according to type, null if creation is unsuccessful</returns>
        private static object DeserializeArray(JsonArray jsonArray, Type type)
        {
            var constructorInfo = type.GetConstructor(new[] { typeof(int) });

            if (constructorInfo != null)
            {
                var newArray = (Array)constructorInfo.Invoke(new object[] { jsonArray.Count });
                var elementType = type.GetElementType();

                for (var i = 0; i < jsonArray.Count; ++i)
                    newArray.SetValue(DeserializeJsonValue(jsonArray[i], elementType), i);

                return newArray;
            }

            return null;
        }

        /// <summary>
        /// Deserializes an object.
        /// </summary>
        /// <param name="json">JsonObject containing members to fill the object with.</param>
        /// <param name="type">type of the object.</param>
        /// <returns>a new object according to type, null if creation is unsuccessful</returns>
        private static object DeserializeObject(JsonObject json, Type type)
        {
            var instance = Activator.CreateInstance(type);

            if (instance == null)
                return null;

            type.GetProperties().Each(propertyInfo => DeserializeProperty(instance, propertyInfo, json));
            type.GetFields().Each(fieldInfo => DeserializeField(instance, fieldInfo, json));

            return instance;
        }

        /// <summary>
        /// Deserializes an object's field.
        /// Only fields that are public, writeable to, 
        /// and do not have the JsonIgnore attribute will be written to
        /// </summary>
        /// <param name="instance">instance of the object containing the field</param>
        /// <param name="fieldInfo">FieldInfo of the field to deserialize</param>
        /// <param name="json">JsonObject containg members to fill the object with</param>
        private static void DeserializeField(object instance, FieldInfo fieldInfo, JsonObject json)
        {
            var memberName = SerializationHelper.MemberName(fieldInfo);

            if (fieldInfo.IsPublic && !fieldInfo.IsLiteral && !fieldInfo.IsInitOnly && !SerializationHelper.IsIgnore(fieldInfo) && json.Members.ContainsKey(memberName))
            {
                var jsonValue = json[memberName];
                fieldInfo.SetValue(instance, DeserializeJsonValue(jsonValue, fieldInfo.FieldType));
            }
        }

        /// <summary>
        /// Deserializes an object's field.
        /// Only properties that are writeable to, 
        /// and do not have the JsonIgnore attribute will be written to
        /// </summary>
        /// <param name="instance">instance of the object containing the property</param>
        /// <param name="propertyInfo">PropertyInfo of the field to deserialize</param>
        /// <param name="json">JsonObject containg members to fill the object with</param>
        private static void DeserializeProperty(object instance, PropertyInfo propertyInfo, JsonObject json)
        {
            var memberName = SerializationHelper.MemberName(propertyInfo);

            if (propertyInfo.CanWrite && !SerializationHelper.IsIgnore(propertyInfo) && json.Members.ContainsKey(memberName))
            {
                var jsonValue = json[memberName];
                propertyInfo.SetValue(instance, DeserializeJsonValue(jsonValue, propertyInfo.PropertyType), null);
            }
        }

        /// <summary>
        /// Deserializes an IJsonValue to the appropriate type
        /// </summary>
        /// <param name="jsonValue">IJsonValue to deserialize</param>
        /// <param name="type">type being deserialized</param>
        /// <returns>a new object according to the type of IJsonValue</returns>
        private static object DeserializeJsonValue(IJsonValue jsonValue, Type type)
        {
            object result = null;

            if (type != null && (type.Equals(typeof(DateTime)) || type.Equals(typeof(DateTime?))) && jsonValue is JsonString)
            {
                DateTime date;
                if (DateTime.TryParse((JsonString)jsonValue, out date))
                    result = date;
            }
            else if (jsonValue is JsonString)
                result = (string)(JsonString)jsonValue;
            else if (jsonValue is JsonNumber)
                result = DeserializeNumber(jsonValue, type);
            else if (jsonValue is JsonBoolean)
                result = (bool)(JsonBoolean)jsonValue;
            //else if (jsonValue is JsonNull) // redundant
            //    result = null;
            else if (jsonValue is JsonArray)
                result = DeserializeEnumerable((JsonArray)jsonValue, type);
            else if (jsonValue is JsonObject)
                result = DeserializeObject((JsonObject)jsonValue, type);

            return result;
        }

        /// <summary>
        /// Deserializes a JsonNumber
        /// </summary>
        /// <param name="jsonValue">JsonNumber to deserialize</param>
        /// <param name="type">type of the JsonNumber</param>
        /// <returns>an number according to the type being deserialized</returns>
        private static object DeserializeNumber(IJsonValue jsonValue, Type type)
        {
            var jsonNumber = (JsonNumber)jsonValue;

            if (type.Equals(typeof(object)))
                return jsonNumber.Value;

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Int32:
                    return Convert.ToInt32(jsonNumber.Value);

                case TypeCode.Byte:
                    return Convert.ToByte(jsonNumber.Value);

                case TypeCode.SByte:
                    return Convert.ToSByte(jsonNumber.Value);

                case TypeCode.Int16:
                    return Convert.ToInt16(jsonNumber.Value);

                case TypeCode.UInt16:
                    return Convert.ToUInt16(jsonNumber.Value);

                case TypeCode.UInt32:
                    return Convert.ToUInt32(jsonNumber.Value);

                case TypeCode.Int64:
                    return Convert.ToInt64(jsonNumber.Value);

                case TypeCode.UInt64:
                    return Convert.ToUInt64(jsonNumber.Value);

                case TypeCode.Double:
                    return Convert.ToDouble(jsonNumber.Value);

                case TypeCode.Single:
                    return Convert.ToSingle(jsonNumber.Value);

                default:
                    return null;
            }
        }
    }
}
