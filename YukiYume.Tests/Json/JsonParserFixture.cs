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
using System.Diagnostics;
using NUnit.Framework;
using log4net;
using YukiYume.Json;
using YukiYume.Logging;

#endregion

namespace YukiYume.Tests.Json
{
    [TestFixture]
    public class JsonParserFixture
    {
        private readonly ILog Log = LogManager.GetLogger(typeof(JsonParserFixture));

        [Test]
        public void ParseGoogleTranslation()
        {
            var json = @"{""responseData"": {""translatedText"":""その目だれの目？""}, ""responseDetails"": null, ""responseStatus"": 200}";
            Log.Info("JSON: {0}", json);

            var jsonValue = JsonParser.Parse(json) as JsonObject;

            Assert.That(jsonValue != null);

            Assert.That(jsonValue.Members.ContainsKey("responseData"));
            var responseData = jsonValue["responseData"] as JsonObject;
            Assert.That(responseData != null);

            Assert.That(responseData.Members.ContainsKey("translatedText"));
            var translatedText = responseData["translatedText"] as JsonString;
            Assert.That(translatedText != null);
            Assert.That((string)translatedText, Is.EqualTo("その目だれの目？"));

            Assert.That(jsonValue.Members.ContainsKey("responseDetails"));
            var responseDetails = jsonValue["responseDetails"] as JsonNull;
            Assert.That(responseDetails != null);

            Assert.That(jsonValue.Members.ContainsKey("responseStatus"));
            var responseStatus = jsonValue["responseStatus"] as JsonNumber;
            Assert.That(responseStatus != null);
            Assert.That(responseStatus == 200);
        }

        [Test]
        public  void ParseArrayOfObjects()
        {
            var json = @"[{ ""Key"" : ""key1"", ""Value"" : ""value1"" }, { ""Key"" : ""key2"", ""Value"" : ""value2"" }, { ""Key"" : ""key3"", ""Value"" : ""value3"" }]";
            Log.Info("JSON: {0}", json);

            var jsonObject = JsonParser.Parse(json) as JsonArray;
            Assert.That(jsonObject != null);
            Assert.That(jsonObject.Count == 3);
        }

        [Test]
        public void ParseSimpleObjectOneMember()
        {
            var json = @"{ ""Pi"" : 3.14159 }";
            var jsonObject = JsonParser.Parse(json) as JsonObject;
            Assert.That(jsonObject != null);
            Assert.That(jsonObject.Members.Count == 1);

            Assert.That(jsonObject.Members.ContainsKey("Pi"));
            var pi = jsonObject["Pi"] as JsonNumber;
            Assert.That(pi == 3.14159d);
        }

        [Test]
        public void ParseSimpleObjectTwoMembers()
        {
            var json = @"{ ""Pi"" : 3.14159, ""Test"" : ""Hello, World!!!"" }";
            var jsonObject = JsonParser.Parse(json) as JsonObject;
            Assert.That(jsonObject != null);
            Assert.That(jsonObject.Members.Count == 2);

            Assert.That(jsonObject.Members.ContainsKey("Pi"));
            var pi = jsonObject["Pi"] as JsonNumber;
            Assert.That(pi == 3.14159d);

            Assert.That(jsonObject.Members.ContainsKey("Test"));
            var test = jsonObject["Test"] as JsonString;
            Assert.That(test == "Hello, World!!!");
        }

        [Test]
        public void ParseDoubleArray()
        {
            var json = "[1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0]";

            var jsonValue = JsonParser.Parse(json);

            Assert.That(jsonValue != null);
            Assert.That(jsonValue is JsonArray);

            var jsonArray = (JsonArray)jsonValue;

            Log.Info("{0}", jsonArray);

            Assert.That(jsonArray.Count == 10);

            for (var i = 0; i < jsonArray.Count; ++i)
            {
                var jsonNumber = jsonArray[i] as JsonNumber;
                Assert.That(jsonNumber != null);
                Assert.That((double)(i + 1) == jsonNumber);
            }
        }

        [Test]
        public void ParseIntegerArray()
        {
            var json = "[1, 2, 3, 4, 5, 6, 7, 8, 9, 10]";

            var jsonValue = JsonParser.Parse(json);

            Assert.That(jsonValue != null);
            Assert.That(jsonValue is JsonArray);

            var jsonArray = (JsonArray)jsonValue;

            Log.Info("{0}", jsonArray);

            Assert.That(jsonArray.Count == 10);

            for (var i = 0; i < jsonArray.Count; ++i)
            {
                var jsonNumber = jsonArray[i] as JsonNumber;
                Assert.That(jsonNumber != null);
                Assert.That(i + 1 == jsonNumber);
            }
        }

        [Test]
        public void ParseEmptyArray()
        {
            var json = "[]";

            var jsonValue = JsonParser.Parse(json);

            Assert.That(jsonValue != null);
            Assert.That(jsonValue is JsonArray);

            var jsonArray = (JsonArray)jsonValue;

            Assert.That(jsonArray.Count == 0);
        }

        [Test]
        public void ParseEmptyObject()
        {
            var json = "{}";

            var jsonValue = JsonParser.Parse(json);

            Assert.That(jsonValue != null);
            Assert.That(jsonValue is JsonObject);

            var jsonObject = (JsonObject)jsonValue;

            Assert.That(jsonObject.Members.Count == 0);
        }

        [Test, ExpectedException(typeof(JsonException))]
        public void ParseEmptyString()
        {
            JsonParser.Parse(string.Empty);
        }

        [Test, ExpectedException(typeof(JsonException))]
        public void ParseNullString()
        {
            JsonParser.Parse(null);
        }

        [Test, ExpectedException(typeof(JsonException))]
        public void ParseIncompleteJson()
        {
            var json = @"{ ""blah"" : }";
            JsonParser.Parse(json);
        }

        [Test, ExpectedException(typeof(JsonException))]
        public void ParseBadJson()
        {
            var json = @"{ .8475674 : null }";
            JsonParser.Parse(json);
        }
    }
}


