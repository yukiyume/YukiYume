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
using log4net;
using NUnit.Framework;
using YukiYume.Logging;

#endregion

namespace YukiYume.Tests
{
    [TestFixture]
    public class StringExtensionsFixture
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(StringExtensionsFixture));
        private static readonly List<Pair<string, string>> SingularPluralPairs = new List<Pair<string, string>>
        { // example words taken from Ruby on Rails inflector.rb
            new Pair<string, string> { First = "post", Second = "posts" },
            new Pair<string, string> { First = "octopus", Second = "octopi" },
            new Pair<string, string> { First = "sheep", Second = "sheep" },
            new Pair<string, string> { First = "word", Second = "words" },
            new Pair<string, string> { First = "the blue mailman", Second = "the blue mailmen" },
            new Pair<string, string> { First = "CamelOctopus", Second = "CamelOctopi" },
            new Pair<string, string> { First = "person", Second = "people" },
            new Pair<string, string> { First = "money", Second = "money" }
        };

        #region ToOrdinal

        [Test]
        public void ToOrdinal()
        {
            var numbers = new List<Pair<string, string>>
            {
                new Pair<string, string> { First = "1", Second = "1st" },
                new Pair<string, string> { First = "2", Second = "2nd" },
                new Pair<string, string> { First = "1002", Second = "1002nd" },
                new Pair<string, string> { First = "1003", Second = "1003rd" },
                new Pair<string, string> { First = "10013", Second = "10013th" },
                new Pair<string, string> { First = "20026", Second = "20026th" },
                new Pair<string, string> { First = "-1", Second = "-1st" },
                new Pair<string, string> { First = "-2", Second = "-2nd" },
                new Pair<string, string> { First = "-1002", Second = "-1002nd" },
                new Pair<string, string> { First = "-1003", Second = "-1003rd" },
                new Pair<string, string> { First = "-10013", Second = "-10013th" },
                new Pair<string, string> { First = "-20026", Second = "-20026th" }
            };

            numbers.Each(pair =>
            {
                Log.Info("{0} -> {1} = {2}?", pair.First, pair.First.ToOrdinal(), pair.Second);
                Assert.That(pair.First.ToOrdinal(), Is.EqualTo(pair.Second));
            });
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void ToOrdinalNull()
        {
            const string number = null;
            number.ToOrdinal();
        }

        [Test]
        public void ToOrdinalNonNumber()
        {
            SingularPluralPairs.Each(pair =>
            {
                Assert.That(pair.First.ToOrdinal() == pair.First);
                Assert.That(pair.Second.ToOrdinal() == pair.Second);
            });
        }

        #endregion

        #region ToPlural

        [Test]
        public void ToPlural()
        {
            SingularPluralPairs.Each(pair =>
            {
                Log.Info("{0} -> {1} = {2}?", pair.First, pair.First.ToPlural(), pair.Second);
                Assert.That(pair.First.ToPlural(), Is.EqualTo(pair.Second));
            });
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void ToPluralNull()
        {
            const string word = null;
            word.ToPlural();
        }

        #endregion

        #region ToSingular

        [Test]
        public void ToSingular()
        {
            SingularPluralPairs.Each(pair =>
            {
                Log.Info("{0} -> {1} = {2}?", pair.Second, pair.Second.ToSingular(), pair.First);
                Assert.That(pair.Second.ToSingular(), Is.EqualTo(pair.First));
            });
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void ToSingularNull()
        {
            const string word = null;
            word.ToSingular();
        }

        #endregion

        #region ToTitleCase

        [Test]
        public void ToTitleCase()
        {
            var titlePairs = new List<Pair<string, string>>
            { // example words taken from Ruby on Rails inflector.rb
                new Pair<string, string> { First = "man from the boondocks", Second = "Man From The Boondocks" },
                new Pair<string, string> { First = "x-men: the last stand", Second = "X-Men: The Last Stand" }
            };

            titlePairs.Each(pair =>
            {
                Log.Info("{0} -> {1} = {2}?", pair.First, pair.First.ToTitleCase(), pair.Second);
                Assert.That(pair.First.ToTitleCase(), Is.EqualTo(pair.Second));
            });
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void ToTitleCaseNull()
        {
            const string word = null;
            word.ToTitleCase();
        }

        #endregion

        #region ToCapitalCase

        [Test]
        public void ToCapitalCase()
        {
            var titlePairs = new List<Pair<string, string>>
            { // example words taken from Ruby on Rails inflector.rb
                new Pair<string, string> { First = "man from the boondocks", Second = "Man from the boondocks" },
                new Pair<string, string> { First = "x-men: the last stand", Second = "X-men: the last stand" }
            };

            titlePairs.Each(pair =>
            {
                Log.Info("{0} -> {1} = {2}?", pair.First, pair.First.ToCapitalCase(), pair.Second);
                Assert.That(pair.First.ToCapitalCase(), Is.EqualTo(pair.Second));
            });
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void ToCapitalCaseNull()
        {
            const string word = null;
            word.ToCapitalCase();
        }

        #endregion
    }
}
