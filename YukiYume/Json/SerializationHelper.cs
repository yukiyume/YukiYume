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
using System.Reflection;

#endregion

namespace YukiYume.Json
{
    /// <summary>
    /// Helper class for JsonSerializer and JsonDeserializer
    /// </summary>
    internal static class SerializationHelper
    {
        /// <summary>
        /// Checks to see if memberInfo has the attribute JsonIgnore
        /// </summary>
        /// <param name="memberInfo">MemberInfo of the type to check</param>
        /// <returns>true iff MemberInfo has the JsonIgnore attribute</returns>
        internal static bool IsIgnore(MemberInfo memberInfo)
        {
            return FindAttribute<JsonIgnoreAttribute>(memberInfo) != null;
        }

        /// <summary>
        /// Determines the name that should be used for writing the MemberInfo name to JSON
        /// or what name to use for reading from JSON for a MemberInfo name.
        /// The property/field name is used if the the MemberInfo doesn't have
        /// the JsonName attribute, otherwise the name specified by the JsonName attribute
        /// will be used
        /// </summary>
        /// <param name="memberInfo">MemberInfo to lookup the name for</param>
        /// <returns>string containing the name to use for the given MemberInfo</returns>
        internal static string MemberName(MemberInfo memberInfo)
        {
            var jsonNameAttribute = FindAttribute<JsonNameAttribute>(memberInfo);

            return jsonNameAttribute != null ? jsonNameAttribute.Name : memberInfo.Name;
        }

        /// <summary>
        /// Checks to see if MemberInfo has the attribute T
        /// </summary>
        /// <typeparam name="T">Attribute to look for</typeparam>
        /// <param name="memberInfo">MemberInfo of the type to check</param>
        /// <returns>the attribute of type T if it is found in MemberInfo, default(T) otherwise</returns>
        private static T FindAttribute<T>(MemberInfo memberInfo) where T : Attribute
        {
            foreach (var attribute in Attribute.GetCustomAttributes(memberInfo))
                if (attribute is T)
                    return (T)attribute;

            return default(T);
        }
    }
}
