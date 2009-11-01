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
using NUnit.Framework;
using log4net;
using YukiYume.Logging;

#endregion

namespace YukiYume.Tests
{
    /// <summary>
    /// CultureInfoWrapper unit tests
    /// </summary>
    [TestFixture]
    public class CultureInfoWrapperFixture
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(typeof(CultureInfoWrapperFixture));

        /// <summary>
        /// CultureInfoWrapper.LookupCulture unit test
        /// </summary>
        [Test]
        public void LookupCulture()
        {
            var cultures = new[] { "en", "ja", "fr", "it", "ko", "zh" };

            TestCultures(cultures, LookupCulture);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void LookupNullCulture()
        {
            Log.Info("Lookup culture: null");
            CultureInfoWrapper.LookupCulture(null);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void SearchNullCulture()
        {
            Log.Info("Search culture: null");
            CultureInfoWrapper.SearchCulture(null);
        }

        /// <summary>
        /// CultureInfoWrapper.SearchCulture unit test
        /// </summary>
        [Test]
        public void SearchCulture()
        {

            var cultures = new[] { "lish", "japan", "ench", "lian", "rean", "inese" };

            TestCultures(cultures, SearchCulture);
        }

        /// <summary>
        /// Tests each culture in cultures using the action cultureAction
        /// </summary>
        /// <param name="cultures">cultures to test</param>
        /// <param name="cultureAction">action to lookup or search for cultures</param>
        private static void TestCultures(IEnumerable<string> cultures, Action<string, bool> cultureAction)
        {
            cultures.Each(culture =>
            {
                Log.Info("Testing culture: {0}", culture);
                cultureAction(culture, true);
            });

            Log.Info("Testing culture: (empty)");
            cultureAction(string.Empty, false);
        }

        /// <summary>
        /// Called by CanLookupCulture unit test to test CultureInfoWrapper.LookupCulture
        /// on the culture parameter
        /// </summary>
        /// <param name="culture">TwoLetterISOLanguageName of culture to lookup</param>
        /// <param name="shouldSucceed">true if CultureInfoWrapper.LookupCulture should return a valid CultureInfo, false otherwise</param>
        private static void LookupCulture(string culture, bool shouldSucceed)
        {
            var cultureInfo = CultureInfoWrapper.LookupCulture(culture);

            if (shouldSucceed)
            {
                Assert.That(cultureInfo != null);
                Assert.That(cultureInfo.TwoLetterISOLanguageName, Is.EqualTo(culture));

                Log.Info("{0} {1} {2} {3} {4} {5} {6}",
                          cultureInfo.Name, cultureInfo.TwoLetterISOLanguageName, cultureInfo.ThreeLetterISOLanguageName,
                          cultureInfo.ThreeLetterWindowsLanguageName, cultureInfo.DisplayName, cultureInfo.EnglishName, cultureInfo.NativeName);
            }
            else
            {
                Assert.That(cultureInfo, Is.Null);
                Log.Info("Null returned from LookupCulture");
            }
        }

        /// <summary>
        /// Called by CanSearchCulture unit test to test CultureInfoWrapper.SearchCulture
        /// on the culture parameter
        /// </summary>
        /// <param name="culture">culture to lookup</param>
        /// <param name="shouldSucceed">true if CultureInfoWrapper.SearchCulture should return a valid CultureInfo, false otherwise</param>
        private static void SearchCulture(string culture, bool shouldSucceed)
        {
            var cultureInfo = CultureInfoWrapper.SearchCulture(culture);

            if (shouldSucceed)
            {
                Assert.That(cultureInfo != null);

                Log.Info("{0} {1} {2} {3} {4} {5} {6}",
                          cultureInfo.Name, cultureInfo.TwoLetterISOLanguageName, cultureInfo.ThreeLetterISOLanguageName,
                          cultureInfo.ThreeLetterWindowsLanguageName, cultureInfo.DisplayName, cultureInfo.EnglishName, cultureInfo.NativeName);
            }
            else
            {
                Assert.That(cultureInfo, Is.Null);
                Log.Info("Null returned from SearchCulture");
            }
        }
    }
}

