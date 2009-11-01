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
using System.Linq;
using System.Text.RegularExpressions;

#endregion

namespace YukiYume
{
    /// <summary>
    /// 
    /// </summary>
    public static class CultureInfoWrapper
    {
        /// <summary>
        /// Looks up the CultureInfo specified by culture
        /// by trying to find a CultureInfo that matches
        /// by Name, TwoLetterISOLanguageName, ThreeLetterISOLanguageName,
        /// ThreeLetterWindowsLanguageName, DisplayName, EnglishName, or NativeName
        /// If a full match is not found, SearchCulture will be called to attempt a 
        /// partial match of Name, TwoLetterISOLanguageName, etc
        /// </summary>
        /// <param name="culture">culture to lookup</param>
        /// <returns>CultureInfo that matches culture if one is found, null otherwise</returns>
        public static CultureInfo LookupCulture(string culture)
        {
            if (culture == null)
                throw new ArgumentNullException("culture");

            if (culture.Length == 0)
                return null;

            var lookupCulture = (from cult in CultureInfo.GetCultures(CultureTypes.NeutralCultures)
                                 let isName = string.Compare(cult.Name, culture, StringComparison.OrdinalIgnoreCase) == 0
                                 let isTwoLetterISO = string.Compare(cult.TwoLetterISOLanguageName, culture, StringComparison.OrdinalIgnoreCase) == 0
                                 let isThreeLetterISO = string.Compare(cult.ThreeLetterISOLanguageName, culture, StringComparison.OrdinalIgnoreCase) == 0
                                 let isThreeLetterWindows = string.Compare(cult.ThreeLetterWindowsLanguageName, culture, StringComparison.OrdinalIgnoreCase) == 0
                                 let isDisplayName = string.Compare(cult.DisplayName, culture, StringComparison.OrdinalIgnoreCase) == 0
                                 let isEnglishName = string.Compare(cult.EnglishName, culture, StringComparison.OrdinalIgnoreCase) == 0
                                 let isNativeName = string.Compare(cult.NativeName, culture, StringComparison.OrdinalIgnoreCase) == 0
                                 where isName || isTwoLetterISO || isThreeLetterISO || isThreeLetterWindows || isDisplayName || isEnglishName || isNativeName
                                 select cult).FirstOrDefault();

            return lookupCulture ?? SearchCulture(culture);
        }

        /// <summary>
        /// Searches up the CultureInfo specified by culture by attempting to find a CultureInfo
        /// that has the string specified by parameter culture as part of 
        /// Name, TwoLetterISOLanguageName, ThreeLetterISOLanguageName,
        /// ThreeLetterWindowsLanguageName, DisplayName, EnglishName, or NativeName
        /// </summary>
        /// <param name="culture">culture to search for</param>
        /// <returns>CultureInfo that matches culture if one is found, null otherwise</returns>
        public static CultureInfo SearchCulture(string culture)
        {
            if (culture == null)
                throw new ArgumentNullException("culture");

            if (culture.Length == 0)
                return null;

            var cultureRegex = new Regex(string.Format(CultureInfo.InvariantCulture, "^.*{0}.*$", culture), RegexOptions.IgnoreCase);

            return (from cult in CultureInfo.GetCultures(CultureTypes.FrameworkCultures)
                    where
                        cultureRegex.IsMatch(cult.Name) || cultureRegex.IsMatch(cult.TwoLetterISOLanguageName) ||
                        cultureRegex.IsMatch(cult.ThreeLetterISOLanguageName) ||
                        cultureRegex.IsMatch(cult.ThreeLetterWindowsLanguageName) ||
                        cultureRegex.IsMatch(cult.DisplayName) ||
                        cultureRegex.IsMatch(cult.EnglishName) || cultureRegex.IsMatch(cult.NativeName)
                    select cult).FirstOrDefault();
        }
    }
}
