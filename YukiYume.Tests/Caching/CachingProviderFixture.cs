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
using System.Threading;
using NUnit.Framework;
using YukiYume.Caching;
using Ninject;

#endregion

namespace YukiYume.Tests.Caching
{
    [TestFixture]
    public class CachingProviderFixture
    {
        private Cache Cache { get; set; }

        [SetUp]
        public void Setup()
        {
            Cache = SetupFixture.Kernel.Get<Cache>();
        }

        [Test]
        public void StoreString()
        {
            Assert.That(Cache.Store("TestKey", "Test Value 123"));
            Assert.That(Cache.Get<string>("TestKey"), Is.EqualTo("Test Value 123"));

            Assert.That(Cache.Store("TestKey2", "Test Value 456"));
            Assert.That(Cache.Get<string>("TestKey2"), Is.EqualTo("Test Value 456"));
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void StoreNullKey()
        {
            Cache.Store(null, "test");
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void StoreNullValue()
        {
            Cache.Store<string>("test", null);
        }

        [Test]
        public void RemoveString()
        {
            Assert.That(Cache.Store("TestKey3", "Test Value 123"));
            Assert.That(Cache.Get<string>("TestKey3"), Is.EqualTo("Test Value 123"));

            Assert.That(Cache.Remove("TestKey3"));
            Assert.That(Cache.Get<string>("TestKey3"), Is.Null);

        }

        [Test]
        public void RemoveNonExistingString()
        {
            Assert.That(!Cache.Remove("TestKey55"));
        }

        [Test]
        public void StoreDateTime()
        {
            Assert.That(Cache.Store("TestKey4", "Test Value 123", DateTime.Now.AddSeconds(5.0d)));
            Assert.That(Cache.Get<string>("TestKey4"), Is.EqualTo("Test Value 123"));

            Thread.Sleep(8000);

            Assert.That(Cache.Get<string>("TestKey4"), Is.Null);
        }

        [Test]
        public void StoreTimeSpan()
        {
            Assert.That(Cache.Store("TestKey5", "Test Value 123", TimeSpan.FromSeconds(5.0d)));
            Assert.That(Cache.Get<string>("TestKey5"), Is.EqualTo("Test Value 123"));

            Thread.Sleep(8000);

            Assert.That(Cache.Get<string>("TestKey5"), Is.Null);
        }
    }
}
