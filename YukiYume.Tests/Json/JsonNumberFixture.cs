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
    public class JsonNumberFixture
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(JsonNumberFixture));

        [Test]
        public void JsonNumbers()
        {
            var number = new JsonNumber();

            Log.Info("Testing default value = 0");

            Assert.That(number.Int64Value.HasValue);
            Assert.That(number.Int64Value.Value == 0);
            Assert.That(number.ToString() == "0");
            Assert.That(!number.DoubleValue.HasValue);

            for (var i = -10; i < 10; ++i)
            {
                Log.Info("Setting and testing value = {0}", i);

                number.Set(i);

                Assert.That(number.Int64Value.HasValue);
                Assert.That(number.Int64Value.Value == i);
                Assert.That(number.ToString() == i.ToString());
                Assert.That(!number.DoubleValue.HasValue);

                for (var j = -1.0d; j < 1.0d; j += 0.1d)
                {
                    Log.Info("Setting and testing value = {0}", j);

                    number.Set(j);

                    Assert.That(number.DoubleValue.HasValue);
                    Assert.That(number.DoubleValue.Value == j);
                    Assert.That(number.ToString() == j.ToString());
                    Assert.That(!number.Int64Value.HasValue);
                }
            }
        }
    }
}

