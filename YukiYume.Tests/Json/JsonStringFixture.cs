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
using NUnit.Framework;
using log4net;
using YukiYume.Json;
using YukiYume.Logging;

#endregion

namespace YukiYume.Tests.Json
{
    [TestFixture]
    public class JsonStringFixture
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(JsonStringFixture));

        private static readonly string[] TestStrings = 
        {
            "R\\ise", "like", "lions", "after", "slumber",
            "in", "unvanquish\"able", "number"
        };

        [Test]
        public void JsonStrings()
        {
            var jsonString = new JsonString();

            Log.Info("Testing default value = string.Empty");

            Assert.That(jsonString.Value != null);
            Assert.That(string.Empty == jsonString);

            foreach (var testString in TestStrings)
            {
                Log.Info("Setting value = {0}", testString);
                jsonString = testString;

                Log.Info("Testing string => {0}", jsonString);
                Assert.That(jsonString.Value.Length, Is.GreaterThan(0));
            }
        }
    }
}
