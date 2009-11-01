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
using System.Text.RegularExpressions;

#endregion


namespace YukiYume
{
    /// <summary>
    /// Based on inflector.rb in Ruby on Rails (http://rubyonrails.org/) and Inflector.cs in Castle ActiveRecord (http://www.castleproject.org/castle/)
    /// Inflector transforms words from singular to plural, plural to singular, integers to ordinal, words to title case
    /// This class is internal and use from outside the assembly should be done through StringExtensions string extension methods
    /// </summary>
    internal static class Inflector
    {
        /// <summary>
        /// Regex for determining if the 'th' ordinal should be used, which is the case when
        /// the number ends in 11, 12, 13, 0, or any digit from 4 to 9
        /// </summary>
        private static readonly Regex OrdinalTh = new Regex(@"^[-]?\d*((11)|(12)|(13)|(0)|[4-9])$");

        /// <summary>
        /// Regex for determining if a string is an integer
        /// </summary>
        private static readonly Regex IntegerRegex = new Regex(@"^[-]?\d+$");

        /// <summary>
        /// Default plural rules
        /// </summary>
        private static readonly List<InflectorRuleReplacement> _plurals = new List<InflectorRuleReplacement>
        {
            new InflectorRuleReplacement("$", "s"),
			new InflectorRuleReplacement("s$", "s"),
			new InflectorRuleReplacement("(ax|test)is$", "$1es"),
			new InflectorRuleReplacement("(octop|vir)us$", "$1i"),
			new InflectorRuleReplacement("(alias|status)$", "$1es"),
			new InflectorRuleReplacement("(bu)s$", "$1ses"),
			new InflectorRuleReplacement("(buffal|tomat)o$", "$1oes"),
			new InflectorRuleReplacement("([ti])um$", "$1a"),
			new InflectorRuleReplacement("sis$", "ses"),
			new InflectorRuleReplacement("(?:([^f])fe|([lr])f)$", "$1$2ves"),
			new InflectorRuleReplacement("(hive)$", "$1s"),
			new InflectorRuleReplacement("([^aeiouy]|qu)y$", "$1ies"),
			new InflectorRuleReplacement("(x|ch|ss|sh)$", "$1es"),
			new InflectorRuleReplacement("(matr|vert|ind)ix|ex$", "$1ices"),
			new InflectorRuleReplacement("([m|l])ouse$", "$1ice"),
			new InflectorRuleReplacement("^(ox)$", "$1en"),
			new InflectorRuleReplacement("(quiz)$", "$1zes")
        };

        /// <summary>
        /// Default singular rules
        /// </summary>
        private static readonly List<InflectorRuleReplacement> _singulars = new List<InflectorRuleReplacement>
        {
            new InflectorRuleReplacement("s$", ""),
			new InflectorRuleReplacement("(n)ews$", "$1ews"),
			new InflectorRuleReplacement("([ti])a$", "$1um"),
			new InflectorRuleReplacement("((a)naly|(b)a|(d)iagno|(p)arenthe|(p)rogno|(s)ynop|(t)he)ses$", "$1$2sis"),
			new InflectorRuleReplacement("(^analy)ses$", "$1sis"),
			new InflectorRuleReplacement("([^f])ves$", "$1fe"),
			new InflectorRuleReplacement("(hive)s$", "$1"),
			new InflectorRuleReplacement("(tive)s$", "$1"),
			new InflectorRuleReplacement("([lr])ves$", "$1f"),
			new InflectorRuleReplacement("([^aeiouy]|qu)ies$", "$1y"),
			new InflectorRuleReplacement("(s)eries$", "$1eries"),
			new InflectorRuleReplacement("(m)ovies$", "$1ovie"),
			new InflectorRuleReplacement("(x|ch|ss|sh)es$", "$1"),
			new InflectorRuleReplacement("([m|l])ice$", "$1ouse"),
			new InflectorRuleReplacement("(bus)es$", "$1"),
			new InflectorRuleReplacement("(o)es$", "$1"),
			new InflectorRuleReplacement("(shoe)s$", "$1"),
			new InflectorRuleReplacement("(cris|ax|test)es$", "$1is"),
			new InflectorRuleReplacement("(octop|vir)i$", "$1us"),
			new InflectorRuleReplacement("(alias|status)es$", "$1"),
			new InflectorRuleReplacement("^(ox)en", "$1"),
			new InflectorRuleReplacement("(vert|ind)ices$", "$1ex"),
			new InflectorRuleReplacement("(matr)ices$", "$1ix"),
			new InflectorRuleReplacement("(quiz)zes$", "$1")
        };

        /// <summary>
        /// Default uncountable words
        /// </summary>
        private static readonly List<string> _uncountables = new List<string>
        {
            "equipment",
			"information",
			"rice",
			"money",
			"species",
			"series",
			"fish",
			"sheep"
        };

        private static List<InflectorRuleReplacement> Plurals { get { return _plurals; } }
        private static List<InflectorRuleReplacement> Singulars { get { return _singulars; } }
        private static List<string> Uncountables { get { return _uncountables; } }

        static Inflector()
		{
            // add irregular plurals to the singular and plural lists
			AddIrregular("person", "people");
			AddIrregular("man", "men");
			AddIrregular("child", "children");
			AddIrregular("sex", "sexes");
			AddIrregular("move", "moves");
		}

        /// <summary>
        /// Adds an irregular singular/plural pair to the appropriate lists
        /// </summary>
        /// <param name="singular">singular form of the word to add</param>
        /// <param name="plural">plural form of the word to add</param>
		private static void AddIrregular(string singular, string plural)
		{
            Add(Plurals, singular, plural);
            Add(Singulars, plural, singular);
		}

        /// <summary>
        /// Adds a new rule to the replacementRules list according to rule and replacement
        /// </summary>
        /// <param name="replacementRules">list to add rule to</param>
        /// <param name="rule">rule to use for matching</param>
        /// <param name="replacement">replacement to use</param>
		private static void Add(ICollection<InflectorRuleReplacement> replacementRules, string rule, string replacement)
		{
            replacementRules.Add(new InflectorRuleReplacement(string.Format("({0}){1}$", rule[0], rule.Substring(1)), string.Format("$1{0}", replacement.Substring(1))));
		}

        /// <summary>
        /// Applies the rules according to the replacementRules list to the input word
        /// </summary>
        /// <param name="replacementRules">list of rules to use</param>
        /// <param name="word">word to transform</param>
        /// <returns>the transformation of the input word according to the rules if there is a match, the input word unchanged otherwise</returns>
		private static string ApplyRules(IEnumerable<InflectorRuleReplacement> replacementRules, string word)
		{
			var result = word;

			if (!Uncountables.Contains(word.ToLower()))
                replacementRules.EachReverse(rule => (result = rule.Apply(word)) != null);

			return result ?? word;
		}

        /// <summary>
        /// Transforms the input string to plural form
        /// </summary>
        /// <param name="word">string to transform to plural form</param>
        /// <returns>input string in plural form</returns>
        /// <exception cref="System.ArgumentNullException">if input string is null</exception>
        internal static string Pluralize(string word)
        {
            if (word == null)
                throw new ArgumentNullException("word");

            return ApplyRules(Plurals, word);
        }

        /// <summary>
        /// Transforms the input string to singular form
        /// </summary>
        /// <param name="word">string to transform to singular form</param>
        /// <returns>input string in singular form</returns>
        /// <exception cref="System.ArgumentNullException">if input string is null</exception>
        internal static string Singularize(string word)
        {
            if (word == null)
                throw new ArgumentNullException("word");

            return ApplyRules(Singulars, word);
        }

        /// <summary>
        /// Transforms the input string to titlecase
        /// </summary>
        /// <param name="word">string to transform to titlecase</param>
        /// <returns>input string in titlecase</returns>
        /// <exception cref="System.ArgumentNullException">if input string is null</exception>
        internal static string Titleize(string word)
        {
            if (word == null)
                throw new ArgumentNullException("word");

            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(word.ToLower());
        }

        /// <summary>
        /// Transforms the input string to ordinal form
        /// </summary>
        /// <param name="number">number to transform to ordinal form</param>
        /// <returns>string with the appropriate ordinal string added (st, nd, rd, th) if input is an integer,
        /// otherwise returns the input string
        /// </returns>
        /// <exception cref="System.ArgumentNullException">if input string is null</exception>
        internal static string Ordinalize(string number)
        {
            if (number == null)
                throw new ArgumentNullException("number");

            if (!IntegerRegex.IsMatch(number))
                return number;

            var ordinal = number;

            if (OrdinalTh.IsMatch(ordinal))
                ordinal = string.Format("{0}th", ordinal);
            else if (ordinal.EndsWith("1"))
                ordinal = string.Format("{0}st", ordinal);
            else if (ordinal.EndsWith("2"))
                ordinal = string.Format("{0}nd", ordinal);
            else if (ordinal.EndsWith("3"))
                ordinal = string.Format("{0}rd", ordinal);

            return ordinal;
        }

        /// <summary>
        /// Transforms the input string to capitalcase
        /// </summary>
        /// <param name="word">string to transform to capitalcase</param>
        /// <returns>input string in capitalcase</returns>
        /// <exception cref="System.ArgumentNullException">if input string is null</exception>
        internal static string Capitalize(string word)
        {
            if (word == null)
                throw new ArgumentNullException("word");

            if (word.Length == 0)
                return word;

            if (word.Length == 1)
                return word.ToUpper();

            return char.ToUpper(word[0]) + word.Substring(1).ToLower();
        }
    }
}
