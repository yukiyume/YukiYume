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
using YukiYume.Json;
using YukiYume.Logging;

#endregion

namespace YukiYume.Tests.Json
{
    [TestFixture]
    public class JsonLexicalAnalyzerFixture
    {
        private readonly ILog Log = LogManager.GetLogger(typeof(JsonLexicalAnalyzerFixture));

        [Test]
        public void CanScanGoogleTranslation()
        {
            var json = @"{""responseData"": {""translatedText"":""誰ですか？""}, ""responseDetails"": null, ""responseStatus"": 200}";
            using (var lex = new JsonLexicalAnalyzer(json))
            {
                JsonLexicalType lexType;

                while ((lexType = lex.Scan()) != JsonLexicalType.EndOfJson && lexType != JsonLexicalType.Error)
                {
                    if (lexType == JsonLexicalType.Number)
                        Log.Info("{0} {1}", lexType, lex.CurrentDouble);
                    else if (lexType == JsonLexicalType.String)
                        Log.Info("{0} {1}", lexType, lex.CurrentString);
                    else
                        Log.Info("{0}", lexType);
                }

                if (lexType == JsonLexicalType.Error)
                {
                    Log.Info("Error at position: {0}", lex.Index);
                    Assert.Fail();
                }
            }
        }

        [Test]
        public void ScanEscapedUnicode()
        {
            var json = @"""\u005C""";
            using (var lex = new JsonLexicalAnalyzer(json))
            {
                var lexType = lex.Scan();
                Assert.That(JsonLexicalType.String == lexType);
                Assert.That(lex.CurrentString == @"\");
            }
        }

        [Test]
        public void ScanBadEscapedUnicode()
        {
            var jsonList = new List<string> { @"""\u005X""", @"""\u00X5""", @"""\u0X05""", @"""\uX005""", @"""\u500""", @"""\u05""", @"""\u5""", @"""\u""" };

            jsonList.Each(json =>
            {
                using (var lex = new JsonLexicalAnalyzer(json))
                {
                    Assert.That(JsonLexicalType.Error == lex.Scan());
                }
            });
        }

        [Test]
        public void ScanDoubleWithExponent()
        {
            var json = @"-1.063E-02";

            using (var lex = new JsonLexicalAnalyzer(json))
            {
                var lexType = lex.Scan();
                Assert.That(JsonLexicalType.Number == lexType);
                Assert.That(-0.01063 == lex.CurrentDouble);
            }
        }

        [Test]
        public void ScanIntegerWithExponent()
        {
            var json = @"1043E+04";

            using (var lex = new JsonLexicalAnalyzer(json))
            {
                var lexType = lex.Scan();
                Assert.That(JsonLexicalType.Number == lexType);
                Assert.That(10430000L == lex.CurrentInt64);
            }
        }
    }
}

