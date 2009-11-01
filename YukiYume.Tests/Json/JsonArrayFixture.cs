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
using System.Linq;
using NUnit.Framework;
using YukiYume.Json;

#endregion

namespace YukiYume.Tests.Json
{
    [TestFixture]
    public class JsonArrayFixture
    {
        [Test]
        public void CanSelectWithLinq()
        {
            var jsonArray = new JsonArray();

            jsonArray.Values.Add(new JsonNumber(1));
            jsonArray.Values.Add(new JsonNumber(2));
            jsonArray.Values.Add(new JsonBoolean(true));
            jsonArray.Values.Add(new JsonNumber(3));
            jsonArray.Values.Add(new JsonNumber(4));
            jsonArray.Values.Add(new JsonNumber(5));
            jsonArray.Values.Add(new JsonString("Test"));

            var x = (from y in jsonArray
                     where y is JsonNumber
                     select y).ToList();

            Assert.That(x.Count, Is.EqualTo(5));
        }
    }
}
