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
using System.Text.RegularExpressions;

#endregion

namespace YukiYume
{
    /// <summary>
    /// Used by Inflector for handling singular/plural rules
    /// </summary>
    internal class InflectorRuleReplacement
    {
        internal Regex Rule { get; set; }
        internal string Replacement { get; set; }

        internal InflectorRuleReplacement()
        {
        }

        internal InflectorRuleReplacement(string rule)
        {
            Rule = new Regex(rule, RegexOptions.IgnoreCase);
        }

        internal InflectorRuleReplacement(string rule, string replacement)
            : this(rule)
        {
            Replacement = replacement;
        }

        /// <summary>
        /// Applies the replacement to the input word if there is a match with the rule
        /// </summary>
        /// <param name="word">word to transform</param>
        /// <returns>word transformed by the replacement if the rule matches, null otherwise</returns>
        internal string Apply(string word)
        {
            return Rule.IsMatch(word) ? Rule.Replace(word, Replacement) : null;
        }
    }
}
