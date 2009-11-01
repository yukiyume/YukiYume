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

#endregion

namespace YukiYume.Caching
{
    /// <summary>
    /// Base Cache class
    /// </summary>
    public abstract class Cache
    {
        /// <summary>
        /// checks key to see if it is null
        /// </summary>
        /// <param name="key">Cache key</param>
        protected static void CheckArgument(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");
        }

        /// <summary>
        /// checks key and value to see if either is null
        /// </summary>
        /// <typeparam name="T">type of value</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="value">value</param>
        protected static void CheckArguments<T>(string key, T value) where T : class
        {
            CheckArgument(key);

            if (value == null)
                throw new ArgumentNullException("value");
        }

        /// <summary>
        /// Retrieves a value from the Cache
        /// </summary>
        /// <typeparam name="T">type of value</typeparam>
        /// <param name="key">Cache key</param>
        /// <returns>value of type T if the item is in the Cache, null otherwise</returns>
        public abstract T Get<T>(string key) where T : class;

        /// <summary>
        /// Stores a value in the Cache
        /// </summary>
        /// <typeparam name="T">type of value</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="value">value to store</param>
        /// <returns>true iff the value was able to be stored in the Cache</returns>
        public abstract bool Store<T>(string key, T value) where T : class;

        /// <summary>
        /// Stores a value in the Cache and automatically removes the item from the cache at expiresAt
        /// </summary>
        /// <typeparam name="T">type of value</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="value">value to store</param>
        /// <param name="expiresAt">DateTime to remove the item from Cache</param>
        /// <returns>true iff the value was able to be stored in the Cache</returns>
        public abstract bool Store<T>(string key, T value, DateTime expiresAt) where T : class;

        /// <summary>
        /// Stores a value in the Cache for validFor time
        /// </summary>
        /// <typeparam name="T">type of value</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="value">value to store</param>
        /// <param name="validFor">TimeSpan for the item to remain in Cache</param>
        /// <returns>true iff the value was able to be stored in the Cache</returns>
        public abstract bool Store<T>(string key, T value, TimeSpan validFor) where T : class;

        /// <summary>
        /// Removes an item from the Cache
        /// </summary>
        /// <param name="key">Cache key</param>
        /// <returns>true iff the item was removed from the Cache</returns>
        public abstract bool Remove(string key);
    }
}
