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

#endregion

namespace YukiYume.Tests.Json
{
    [TestFixture]
    public class JsonSerializerFixture
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(JsonSerializerFixture));

        [Test]
        public void SerializeList()
        {
            var stringList = new List<string>
            {
                "Imagination",
                "is",
                "more",
                "important",
                "than",
                "knowledge"
            };

            var jsonString = JsonSerializer.Serialize(stringList);
            Assert.That(jsonString == @"[""Imagination"", ""is"", ""more"", ""important"", ""than"", ""knowledge""]");
            Log.Info(jsonString);
        }

        [Test]
        public void SerializeArray()
        {
            var values = new[] { "hello", "serialized", "world" };
            var jsonString = JsonSerializer.Serialize(values);
            Assert.That(!string.IsNullOrEmpty(jsonString));
            Log.Info(jsonString);

            var someStuff = new[]
            {
                new SomeClass { PropertyOne = "Hello", PropertyTwo = 1 },
                new SomeClass { PropertyOne = "World", PropertyTwo = 2 },
                new SomeClass { PropertyOne = "!!!!!", PropertyTwo = 3 }
            };

            var jsonStuff = JsonSerializer.SerializeAsJsonValue(someStuff);
            Assert.That(jsonStuff, !Is.SameAs(Singleton<JsonNull>.Instance));

            jsonString = jsonStuff.ToString();
            Log.Info(jsonString);
            Assert.That(jsonString == @"[{ ""propertyTwo"" : 1, ""PropertyThree"" : 0, ""PropertyFour"" : 0, ""PropertyFive"" : false, ""PropertySix"" : null }, { ""propertyTwo"" : 2, ""PropertyThree"" : 0, ""PropertyFour"" : 0, ""PropertyFive"" : false, ""PropertySix"" : null }, { ""propertyTwo"" : 3, ""PropertyThree"" : 0, ""PropertyFour"" : 0, ""PropertyFive"" : false, ""PropertySix"" : null }]");
        }

        [Test]
        public void SerializeDictionary()
        {
            var dictionary = new Dictionary<string, string> { { "key1", "value1" }, { "key2", "value2" }, { "key3", "value3" } };
            var jsonDictionary = JsonSerializer.SerializeAsJsonValue(dictionary);
            Assert.That(jsonDictionary != Singleton<JsonNull>.Instance);

            var jsonString = jsonDictionary.ToString();
            Assert.That(!string.IsNullOrEmpty(jsonString));
            Log.Info((jsonString));
        }

        [Test]
        public  void SerializeFields()
        {
            var someOtherClass = new SomeOtherClass {PublicField = 1};
            var jsonFieldClass = JsonSerializer.SerializeAsJsonValue(someOtherClass);
            Assert.That(jsonFieldClass != Singleton<JsonNull>.Instance);

            var jsonString = jsonFieldClass.ToString();
            Assert.That(!string.IsNullOrEmpty(jsonString));
            Assert.That(jsonString == @"{ ""PublicField"" : 1 }");
            Log.Info((jsonString));
        }

        [Test]
        public void SerializeNull()
        {
            var json = JsonSerializer.SerializeAsJsonValue(null);
            Assert.That(json == Singleton<JsonNull>.Instance);
        }

        [Test]
        public void SerializeAnonymous()
        {
            var anonymous = new { Hello = "World", Konnichiwa = "Sekai" };
            var jsonAnonymous = JsonSerializer.SerializeAsJsonValue(anonymous);
            Assert.That(jsonAnonymous != Singleton<JsonNull>.Instance);

            var jsonString = jsonAnonymous.ToString();
            Assert.That(!string.IsNullOrEmpty(jsonString));
            Assert.That(jsonString == @"{ ""Hello"" : ""World"", ""Konnichiwa"" : ""Sekai"" }");
            Log.Info((jsonString));
        }

        [Test]
        public void SerializeDateTime()
        {
            var anonymous = new { Date = DateTime.Now };
            var jsonAnonymous = JsonSerializer.SerializeAsJsonValue(anonymous);
            Assert.That(jsonAnonymous != Singleton<JsonNull>.Instance);

            var jsonString = jsonAnonymous.ToString();
            Assert.That(!string.IsNullOrEmpty(jsonString));
            Log.Info((jsonString));
        }
    }
}
