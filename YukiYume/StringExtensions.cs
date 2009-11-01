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
using System.Text;

#endregion

namespace YukiYume
{
    /// <summary>
    /// string extension methods
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Transforms the input string to titlecase
        /// </summary>
        /// <param name="word">string to transform to titlecase</param>
        /// <returns>input string in titlecase</returns>
        /// <exception cref="System.ArgumentNullException">if input string is null</exception>
        public static string ToTitleCase(this string word)
        {
            return Inflector.Titleize(word);
        }

        /// <summary>
        /// Transforms the input string to singular form
        /// </summary>
        /// <param name="word">string to transform to singular form</param>
        /// <returns>input string in singular form</returns>
        /// <exception cref="System.ArgumentNullException">if input string is null</exception>
        public static string ToSingular(this string word)
        {
            return Inflector.Singularize(word);
        }

        /// <summary>
        /// Transforms the input string to plural form
        /// </summary>
        /// <param name="word">string to transform to plural form</param>
        /// <returns>input string in plural form</returns>
        /// <exception cref="System.ArgumentNullException">if input string is null</exception>
        public static string ToPlural(this string word)
        {
            return Inflector.Pluralize(word);
        }

        /// <summary>
        /// Transforms the input string to ordinal form
        /// </summary>
        /// <param name="number">number to transform to ordinal form</param>
        /// <returns>string with the appropriate ordinal string added (st, nd, rd, th) if input is an integer,
        /// otherwise returns the input string
        /// </returns>
        /// <exception cref="System.ArgumentNullException">if input string is null</exception>
        public static string ToOrdinal(this string number)
        {
            return Inflector.Ordinalize(number);
        }

        /// <summary>
        /// Transforms the input string to capitalcase
        /// </summary>
        /// <param name="word">string to transform to capitalcase</param>
        /// <returns>input string in capitalcase</returns>
        /// <exception cref="System.ArgumentNullException">if input string is null</exception>
        public static string ToCapitalCase(this string word)
        {
            return Inflector.Capitalize(word);
        }

        /// <summary>
        /// Creates a new string containing n copies of the input string
        /// </summary>
        /// <param name="word">string to create copies of</param>
        /// <param name="n">number of copies to make</param>
        /// <returns>new string containing n copies of the input string</returns>
        /// <exception cref="System.ArgumentNullException">if input string is null</exception>
        public static string Times(this string word, int n)
        {
            if (word == null)
                throw new ArgumentNullException("word");

            var stringBuilder = new StringBuilder();

            n.Times(() => stringBuilder.Append(word));

            return stringBuilder.ToString();
        }
    }
}
