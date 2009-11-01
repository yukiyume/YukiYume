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
using System.Text;
using log4net;
using NUnit.Framework;
using YukiYume.Logging;
using YukiYume.Security;

#endregion

namespace YukiYume.Tests.Security
{
    [TestFixture]
    public class AesWrapperFixture
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (AesWrapperFixture));
        private const string Key = "3BE18003D285C8753A9F1974BDE4DD8BCE24929F6D150A03EF8D2A478DC0AD22";
        private const string IV = "CBA491F7F70A88DBAFC1DB80B53BE01A";

        private static readonly byte[] KeyData = new byte[]
        {
            0x3B, 0xE1, 0x80, 0x03, 0xD2, 0x85, 0xC8, 0x75, 0x3A, 0x9F, 0x19, 0x74, 0xBD, 0xE4, 0xDD, 0x8B, 
            0xCE, 0x24, 0x92, 0x9F, 0x6D, 0x15, 0x0A, 0x03, 0xEF, 0x8D, 0x2A, 0x47, 0x8D, 0xC0, 0xAD, 0x22
        };

        private static readonly byte[] IVData = new byte[]
        {
            0xCB, 0xA4, 0x91, 0xF7, 0xF7, 0x0A, 0x88, 0xDB, 0xAF, 0xC1, 0xDB, 0x80, 0xB5, 0x3B, 0xE0, 0x1A
        };

        [Test]
        public void EncryptWithExistingKeyIV()
        {
            using (var aes = new AesWrapper(Key, IV))
            {
                TestEncryption(aes);
            }
        }

        [Test]
        public void EncryptWithExistingKeyNewIV()
        {
            using (var aes = new AesWrapper(KeyData))
            {
                var iv = ConvertWrapper.ToHexString(aes.CreateIV());
                Assert.That(!string.IsNullOrEmpty(iv));
                Log.Info("IV: {0}", iv);

                TestEncryption(aes);
            }
        }

        [Test]
        public void EncryptWithExistingKeyIVData()
        {
            using (var aes = new AesWrapper(KeyData, IVData))
            {
                TestEncryption(aes);
            }
        }

        [Test]
        public void EncryptWithExistingKeyDataIV()
        {
            using (var aes = new AesWrapper(KeyData, IV))
            {
                TestEncryption(aes);
            }
        }

        [Test]
        public void EncryptWithNewKeyIV()
        {
            using (var aes = new AesWrapper())
            {
                var key = ConvertWrapper.ToHexString(aes.CreateKey());
                Assert.That(!string.IsNullOrEmpty(key));
                Log.Info("Key: {0}", key);

                var iv = ConvertWrapper.ToHexString(aes.CreateIV());
                Assert.That(!string.IsNullOrEmpty(iv));
                Log.Info("IV: {0}", iv);

                TestEncryption(aes);
            }
        }

        private static void TestEncryption(AesWrapper aes)
        {
            const string stringToEncrypt = "hello, encrypted world!!!";

            var encrypted = aes.EncryptToString(stringToEncrypt);
            Assert.That(encrypted != null);
            Log.Info("Encrypted: {0}", encrypted);

            var decrypted = aes.DecryptFromString(encrypted);
            Assert.That(stringToEncrypt == decrypted);

            encrypted = aes.EncryptAsHexString(stringToEncrypt);
            Assert.That(encrypted != null);
            Log.Info("Encrypted: {0}", encrypted);

            decrypted = aes.DecryptFromHexString(encrypted);
            Assert.That(stringToEncrypt == decrypted);

            var encryptedData = aes.Encrypt(stringToEncrypt);
            Assert.That(encryptedData != null);
            var decryptedData = aes.Decrypt(encryptedData);
            Assert.That(decryptedData != null);
            Assert.That(Encoding.Unicode.GetString(decryptedData) == stringToEncrypt);
        }
    }
}
