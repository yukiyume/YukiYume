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
using System.IO;
using System.Security.Cryptography;
using System.Text;

#endregion

namespace YukiYume.Security
{
    /// <summary>
    /// Provides a wrapper around the AesCryptoServiceProvider class to more 
    /// easily work with common uses of Aes
    /// </summary>
    public sealed class AesWrapper : IDisposable
    {
        private bool _disposed;
        private AesCryptoServiceProvider AesCryptoServiceProvider { get; set; }

        /// <summary>
        /// Initializes a new instance of AesWrapper with the specified key and initialization vector
        /// </summary>
        /// <param name="key">Aes key</param>
        /// <param name="initializationVector">Aes initialization vector</param>
        public AesWrapper(byte[] key, byte[] initializationVector)
        {
            AesCryptoServiceProvider = new AesCryptoServiceProvider
            {
                Key = key,
                IV = initializationVector,
                Padding = PaddingMode.PKCS7
            };
        }

        /// <summary>
        /// Initializes a new instance of AesWrapper with the specified key and initialization vector
        /// </summary>
        /// <param name="key">Aes key</param>
        /// <param name="initializationVector">Aes initialization vector</param>
        public AesWrapper(string key, string initializationVector)
        {
            AesCryptoServiceProvider = new AesCryptoServiceProvider
            {
                Key = ConvertWrapper.ToByteArray(key),
                IV = ConvertWrapper.ToByteArray(initializationVector),
                Padding = PaddingMode.PKCS7
            };
        }

        /// <summary>
        /// Initializes a new instance of AesWrapper with the specified key and initialization vector
        /// </summary>
        /// <param name="key">Aes key</param>
        /// <param name="initializationVector">Aes initialization vector</param>
        public AesWrapper(byte[] key, string initializationVector)
        {
            AesCryptoServiceProvider = new AesCryptoServiceProvider
            {
                Key = key,
                IV = ConvertWrapper.ToByteArray(initializationVector),
                Padding = PaddingMode.PKCS7
            };
        }

        /// <summary>
        /// Initializes a new instance of AesWrapper with the specified key
        /// </summary>
        /// <param name="key">Aes key</param>
        public AesWrapper(byte[] key)
        {
            AesCryptoServiceProvider = new AesCryptoServiceProvider
            {
                Key = key,
                Padding = PaddingMode.PKCS7
            };
        }

        /// <summary>
        /// Initializes a new instance of AesWrapper
        /// </summary>
        public AesWrapper()
        {
            AesCryptoServiceProvider = new AesCryptoServiceProvider { Padding = PaddingMode.PKCS7 };
        }

        ~AesWrapper()
        {
            Dispose(false);
        }

        /// <summary>
        /// Creates a new Aes key for this AesWrapper
        /// </summary>
        /// <returns>the new key</returns>
        public byte[] CreateKey()
        {
            AesCryptoServiceProvider.GenerateKey();
            return AesCryptoServiceProvider.Key;
        }

        /// <summary>
        /// Createa a new Aes initialization vector for this AesWrapper
        /// </summary>
        /// <returns>the new initialization vector</returns>
        public byte[] CreateIV()
        {
            AesCryptoServiceProvider.GenerateIV();
            return AesCryptoServiceProvider.IV;
        }

        /// <summary>
        /// Encrypts the input string
        /// </summary>
        /// <param name="dataToEncrypt">string to encrypt</param>
        /// <returns>encrypted form of the input</returns>
        public byte[] Encrypt(string dataToEncrypt)
        {
            if (dataToEncrypt == null)
                throw new ArgumentNullException("dataToEncrypt");

            return Encrypt(Encoding.Unicode.GetBytes(dataToEncrypt));
        }

        /// <summary>
        /// Encrypts the input data
        /// </summary>
        /// <param name="dataToEncrypt">data to encrypt</param>
        /// <returns>encrypted form of the input</returns>
        public byte[] Encrypt(byte[] dataToEncrypt)
        {
            if (dataToEncrypt == null)
                throw new ArgumentNullException("dataToEncrypt");

            using (var memoryStream = new MemoryStream())
            using (var encryptor = AesCryptoServiceProvider.CreateEncryptor())
            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
            {
                cryptoStream.Write(dataToEncrypt, 0, dataToEncrypt.Length);
                cryptoStream.FlushFinalBlock();

                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Encrypts the input string using the conversionFunc to transform
        /// the encrypted form to a string
        /// </summary>
        /// <param name="dataToEncrypt">string to encrypt</param>
        /// <param name="conversionFunc">transformation func</param>
        /// <returns>encrypted form of the input as a string</returns>
        public string EncryptToString(string dataToEncrypt, Func<byte[], string> conversionFunc)
        {
            if (dataToEncrypt == null)
                throw new ArgumentNullException("dataToEncrypt");

            return dataToEncrypt.Length == 0 ? dataToEncrypt : conversionFunc(Encrypt(Encoding.Unicode.GetBytes(dataToEncrypt)));
        }

        /// <summary>
        /// Encrypts the input string using Convert.ToBase64String to transform
        /// the encrypted form to a string
        /// </summary>
        /// <param name="dataToEncrypt">string to encrypt</param>
        /// <returns>encrypted form of the input as a string</returns>
        public string EncryptToString(string dataToEncrypt)
        {
            return EncryptToString(dataToEncrypt, Convert.ToBase64String);
        }

        /// <summary>
        /// Encrypts the input string using ConvertWrapper.ToHexString to transform
        /// the encrypted form to a string
        /// </summary>
        /// <param name="dataToEncrypt">string to encrypt</param>
        /// <returns>encrypted form of the input as a string</returns>
        public string EncryptAsHexString(string dataToEncrypt)
        {
            return EncryptToString(dataToEncrypt, ConvertWrapper.ToHexString);
        }

        /// <summary>
        /// Decrypts the input data
        /// </summary>
        /// <param name="encryptedData">data to decrypt</param>
        /// <returns>decrypted form of the input</returns>
        public byte[] Decrypt(byte[] encryptedData)
        {
            if (encryptedData == null)
                throw new ArgumentNullException("encryptedData");

            using (var memoryStream = new MemoryStream())
            using (var decryptor = AesCryptoServiceProvider.CreateDecryptor())
            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write))
            {
                cryptoStream.Write(encryptedData, 0, encryptedData.Length);
                cryptoStream.FlushFinalBlock();

                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Decrypts the input data using conversionFunc to transform
        /// the decrypted form to a string
        /// </summary>
        /// <param name="encryptedInput">data to decrypt</param>
        /// <param name="conversionFunc">transformation func</param>
        /// <returns>decrypted form of the input</returns>
        public string DecryptFromString(string encryptedInput, Func<string, byte[]> conversionFunc)
        {
            if (string.IsNullOrEmpty(encryptedInput))
                return null;

            return encryptedInput.Length == 0 ? encryptedInput : Encoding.Unicode.GetString(Decrypt(conversionFunc(encryptedInput)));
        }

        /// <summary>
        /// Decrypts the input data using Convert.FromBase64String to transform
        /// the decrypted form to a string
        /// </summary>
        /// <param name="encryptedInput">data to decrypt</param>
        /// <returns>decrypted form of the input</returns>
        public string DecryptFromString(string encryptedInput)
        {
            return DecryptFromString(encryptedInput, Convert.FromBase64String);
        }

        /// <summary>
        /// Decrypts the input data using ConvertWrapper.ToByteArray to transform
        /// the decrypted form to a string
        /// </summary>
        /// <param name="encryptedInput">data to decrypt</param>
        /// <returns>decrypted form of the input</returns>
        public string DecryptFromHexString(string encryptedInput)
        {
            return DecryptFromString(encryptedInput, ConvertWrapper.ToByteArray);
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    AesCryptoServiceProvider.Clear();
                }

                _disposed = true;
            }
        }

        #endregion
    }
}
