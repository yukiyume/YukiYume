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
using System.Collections;
using System.Collections.Generic;
using log4net;
using NUnit.Framework;
using YukiYume.Json;
using YukiYume.Logging;

#endregion

namespace YukiYume.Tests.Json
{
    [TestFixture]
    public class JsonDeserializerFixture
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(JsonDeserializerFixture));

        [Test]
        public void DeserializeBasicObject()
        {
            var json = @" { ""PropertyOne"" : ""Test"", ""propertyTwo"" : 101, ""PropertyThree"" : 23, ""PropertyFour"" : 323423, ""PropertyFive"" : true, ""PropertySix"" : null }";

            var jsonValue = JsonDeserializer.Deserialize<SomeClass>(json);

            Assert.That(jsonValue != null);
            Assert.That(jsonValue.PropertyOne == null);
            Assert.That(jsonValue.PropertyTwo == 101);
            Assert.That(jsonValue.PropertyThree == 23);
            Assert.That(jsonValue.PropertyFour == 323423L);
            Assert.That(jsonValue.PropertyFive);
            Assert.That(jsonValue.PropertySix == null);
        }

        [Test]
        public void DeserializeStringArray()
        {
            var json = @"[ ""one"", ""two"", ""three"" ]";
            var array = JsonDeserializer.Deserialize<string[]>(json);

            Assert.That(array != null);
            Assert.That(array.Length == 3);
            array.Each(item =>
            {
                Assert.That(!string.IsNullOrEmpty(item));
                Log.Info(item);
            });
        }

        [Test]
        public void DeserializeIntArray()
        {
            var json = @"[ 1, 2, 3 ]";
            var array = JsonDeserializer.Deserialize<int[]>(json);

            Assert.That(array != null);
            Assert.That(array.Length == 3);
            array.Each(item =>
            {
                Assert.That(item > 0);
                Log.Info(item);
            });
        }

        [Test]
        public void DeserializeDateTime()
        {
            var json = @"{ ""created_at"" : ""2009/10/16 12:21:01 -0700"" }";
            var date = JsonDeserializer.Deserialize<SomeDateClass>(json);

            Assert.That(date != null);
            Assert.That(date.CreatedAt != default(DateTime));
            Log.Info(date.CreatedAt);
        }

        [Test]
        public void DeserializeBooleanArray()
        {
            var json = @"[ true, false, true, false ]";
            var array = JsonDeserializer.Deserialize<bool[]>(json);

            Assert.That(array != null);
            Assert.That(array.Length == 4);

            var initial = false;

            array.Each(item =>
            {
                initial = !initial;
                Assert.That(item == initial);
                Log.Info(item);
            });
        }

        [Test]
        public void DeserializeSomeObjectArray()
        {
            var json = @"[{ ""PropertyOne"" : ""Test"", ""propertyTwo"" : 101, ""PropertyThree"" : 23, ""PropertyFour"" : 323423, ""PropertyFive"" : true, ""PropertySix"" : null }, { ""PropertyOne"" : ""Test2"", ""propertyTwo"" : 102, ""PropertyThree"" : 24, ""PropertyFour"" : 323424, ""PropertyFive"" : false, ""PropertySix"" : null }]";

            var array = JsonDeserializer.Deserialize<SomeClass[]>(json);

            Assert.That(array != null);
            Assert.That(array.Length == 2);

            array.Each(item =>
            {
                Assert.That(item != null);
                Assert.That(item.PropertyOne == null);
                Assert.That(item.PropertyTwo == 101 || item.PropertyTwo == 102);
                Assert.That(item.PropertyThree == 23 || item.PropertyThree == 24);
                Assert.That(item.PropertyFour == 323423L || item.PropertyFour == 323424L);
                Assert.That(item.PropertySix == null);
            });
        }

        [Test]
        public void DeserializeGenericCollection()
        {
            var list = @"[""hello"", ""generic"", ""world!""]";
            var collection = JsonDeserializer.Deserialize<List<string>>(list);
            Assert.That(collection != null);
            Assert.That(collection.Count > 0);

            collection.Each(item =>
            {
                Assert.That(!string.IsNullOrEmpty(item));
                Log.Info(item);
            });
        }

        [Test]
        public void DeserializeGenericStringDictionary()
        {
            var list = @"[{ ""Key"" : ""key1"", ""Value"" : ""value1"" }, { ""Key"" : ""key2"", ""Value"" : ""value2"" }, { ""Key"" : ""key3"", ""Value"" : ""value3"" }]";
            var dictionary = JsonDeserializer.Deserialize<Dictionary<string, string>>(list);
            Assert.That(dictionary != null);
            Assert.That(dictionary.Count > 0);

            dictionary.Each(item =>
            {
                Assert.That(!string.IsNullOrEmpty(item.Key));
                Assert.That(!string.IsNullOrEmpty(item.Value));
                Log.Info(item);
            });
        }

        [Test]
        public void DeserializeGenericStringIDictionary()
        {
            var list = @"[{ ""Key"" : ""key1"", ""Value"" : ""value1"" }, { ""Key"" : ""key2"", ""Value"" : ""value2"" }, { ""Key"" : ""key3"", ""Value"" : ""value3"" }]";
            var dictionary = JsonDeserializer.Deserialize<IDictionary<string, string>>(list);
            Assert.That(dictionary != null);
            Assert.That(dictionary.Count > 0);

            dictionary.Each(item =>
            {
                Assert.That(!string.IsNullOrEmpty(item.Key));
                Assert.That(!string.IsNullOrEmpty(item.Value));
                Log.Info(item);
            });
        }

        [Test]
        public void DeserializeCollection()
        {
            var list = @"[""hello"", ""primitive"", ""world!""]";
            var collection = JsonDeserializer.Deserialize<ArrayList>(list);
            Assert.That(collection != null);
            Assert.That(collection.Count > 0);

            foreach (string item in collection)
            {
                Assert.That(!string.IsNullOrEmpty(item));
                Log.Info(item);
            }
        }

        [Test]
        public void DeserializeNestedObject()
        {
            var json = @"{ ""nested_two"" : { ""nested_three"" : { ""property_three"" : ""three"", ""property_four"" : 5 }, ""property_one"" : ""hello"", ""property_two"" : 22 } }";
            var nested = JsonDeserializer.Deserialize<Nested>(json);
            Assert.That(nested != null);
            Assert.That(nested.NestedTwo != null);
            Assert.That(nested.NestedTwo.NestedThree != null);
            Assert.That(nested.NestedTwo.PropertOne == "hello");
            Assert.That(nested.NestedTwo.PropertyTwo == 22);
            Assert.That(nested.NestedTwo.NestedThree.PropertyThree == "three");
            Assert.That(nested.NestedTwo.NestedThree.PropertyFour == 5);
        }

        [Test]
        public void DeserializeObjectWithArray()
        {
            var json = @"{ ""PropertyOne"" : ""hello"", ""Numbers"": [ 2, 3, 5, 7, 11 ] }";
            var objectWithArray = JsonDeserializer.Deserialize<ObjectWithArray>(json);
            Assert.That(objectWithArray != null);
            Assert.That(objectWithArray.PropertyOne == "hello");
            Assert.That(objectWithArray.Numbers != null);
            Assert.That(objectWithArray.Numbers.Length == 5);
            objectWithArray.Numbers.Each(number => Assert.That(number > 0));
        }

        [Test]
        public void DeserializeGenericIList()
        {
            var list = @"[""hello"", ""interface"", ""world!""]";
            var collection = JsonDeserializer.Deserialize<IList<string>>(list);
            Assert.That(collection != null);
            Assert.That(collection.Count > 0);

            collection.Each(item =>
            {
                Assert.That(!string.IsNullOrEmpty(item));
                Log.Info(item);
            });
        }
    }
}
