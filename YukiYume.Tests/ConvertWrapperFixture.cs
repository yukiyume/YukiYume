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
using log4net;
using NUnit.Framework;
using YukiYume.Logging;

#endregion

namespace YukiYume.Tests
{
    [TestFixture]
    public class ConvertWrapperFixture
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ConvertWrapperFixture));
        private static readonly byte[] Data = new byte[] { 0xA1, 0x99, 0x66, 0xFF, 0x4D, 0x04 };

        [Test]
        public void FromByteArrayToHexString()
        {
            var hexString = ConvertWrapper.ToHexString(Data);

            Log.Info(hexString);
            Assert.That(hexString, Is.EqualTo("A19966FF4D04"));
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void FromNullToHexString()
        {
            ConvertWrapper.ToHexString(null);
        }

        [Test]
        public void FromStringToByteArray()
        {
            var hexString = "A19966FF4D04";
            var data = ConvertWrapper.ToByteArray(hexString);

            Assert.That(data != null);
            Assert.That(data, Is.EquivalentTo(Data));
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void FromNullToByteArray()
        {
            ConvertWrapper.ToByteArray(null);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void FromInvalidInputLengthToByteArray()
        {
            ConvertWrapper.ToByteArray("A19");
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void FromInvalidCharactersToByteArray()
        {
            ConvertWrapper.ToByteArray("X0Y24D");
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void FromEmptyStringToByteArray()
        {
            ConvertWrapper.ToByteArray(string.Empty);
        }

        [Test]
        public void ToInt32()
        {
            Assert.That(ConvertWrapper.ToInt32(null), Is.EqualTo(0));
            Assert.That(ConvertWrapper.ToInt32((object)0), Is.EqualTo(0));
            Assert.That(ConvertWrapper.ToInt32(string.Empty), Is.EqualTo(0));
            Assert.That(ConvertWrapper.ToInt32("test"), Is.EqualTo(0));

            for (var i = -1000; i <= 1000; ++i)
                Assert.That(ConvertWrapper.ToInt32(i.ToString()), Is.EqualTo(i));
        }

        [Test]
        public void ToInt16()
        {
            Assert.That(ConvertWrapper.ToInt16(null), Is.EqualTo(0));
            Assert.That(ConvertWrapper.ToInt16((object)0), Is.EqualTo(0));
            Assert.That(ConvertWrapper.ToInt16(string.Empty), Is.EqualTo(0));
            Assert.That(ConvertWrapper.ToInt16("test"), Is.EqualTo(0));

            for (short i = -1000; i <= 1000; ++i)
                Assert.That(ConvertWrapper.ToInt16(i.ToString()), Is.EqualTo(i));
        }

        [Test]
        public void ToInt64()
        {
            Assert.That(ConvertWrapper.ToInt64(null), Is.EqualTo(0L));
            Assert.That(ConvertWrapper.ToInt64((object)0L), Is.EqualTo(0L));
            Assert.That(ConvertWrapper.ToInt64(string.Empty), Is.EqualTo(0L));
            Assert.That(ConvertWrapper.ToInt64("test"), Is.EqualTo(0L));

            for (var i = -1000L; i <= 1000L; ++i)
                Assert.That(ConvertWrapper.ToInt64(i.ToString()), Is.EqualTo(i));
        }

        [Test]
        public void ToSingle()
        {
            Assert.That(ConvertWrapper.ToSingle(null), Is.EqualTo(0.0f));
            Assert.That(ConvertWrapper.ToSingle((object)0.0f), Is.EqualTo(0.0f));
            Assert.That(ConvertWrapper.ToSingle(string.Empty), Is.EqualTo(0.0f));
            Assert.That(ConvertWrapper.ToSingle("test"), Is.EqualTo(0.0f));

            for (var i = -100.0f; i <= 100.0f; i += 0.1f)
                Assert.That(ConvertWrapper.ToSingle(i.ToString()), Is.InRange(i - 0.1d, i + 0.1d));
        }

        [Test]
        public void ToDouble()
        {
            Assert.That(ConvertWrapper.ToDouble(null), Is.EqualTo(0.0d));
            Assert.That(ConvertWrapper.ToDouble((object)0.0d), Is.EqualTo(0.0d));
            Assert.That(ConvertWrapper.ToDouble(string.Empty), Is.EqualTo(0.0d));
            Assert.That(ConvertWrapper.ToDouble("test"), Is.EqualTo(0.0d));

            for (var i = -100.0d; i <= 100.0d; i += 0.1d)
                Assert.That(ConvertWrapper.ToDouble(i.ToString()), Is.InRange(i - 0.1d, i + 0.1d));
        }

        [Test]
        public void ToBoolean()
        {
            Assert.That(!ConvertWrapper.ToBoolean(null));
            Assert.That(ConvertWrapper.ToBoolean((object)true));
            Assert.That(!ConvertWrapper.ToBoolean(string.Empty));
            Assert.That(!ConvertWrapper.ToBoolean("test"));
            Assert.That(!ConvertWrapper.ToBoolean("FALSE"));
            Assert.That(!ConvertWrapper.ToBoolean("false"));
            Assert.That(ConvertWrapper.ToBoolean("true"));
        }

        [Test]
        public void ToGuid()
        {
            Assert.That(ConvertWrapper.ToGuid(null), Is.EqualTo(Guid.Empty));
            Assert.That(ConvertWrapper.ToGuid(string.Empty), Is.EqualTo(Guid.Empty));
            Assert.That(ConvertWrapper.ToGuid("test"), Is.EqualTo(Guid.Empty));

            var guid = Guid.NewGuid();
            Log.Info("Guid: {0}", guid);

            Assert.That(ConvertWrapper.ToGuid(guid.ToString()), Is.EqualTo(guid));
            Assert.That(ConvertWrapper.ToGuid((object)guid), Is.EqualTo(guid));
        }
    }
}

