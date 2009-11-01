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
using System.Threading;

#endregion

namespace YukiYume.Caching
{
    /// <summary>
    /// Simple Cache intended for testing, uses a Dictionary to hold cache data
    /// </summary>
    public class DictionaryCache : Cache
    {
        private static readonly Dictionary<string, object> _cacheDictionary = new Dictionary<string, object>();
        private static readonly object CachePadlock = new object();

        static DictionaryCache()
        {
        }

        private static Dictionary<string, object> CacheDictionary
        {
            get { return _cacheDictionary; }
        }

        public override T Get<T>(string key)
        {
            CheckArgument(key);

            T value;

            lock (CachePadlock)
            {
                value = CacheDictionary.ContainsKey(key) ? CacheDictionary[key] as T : null;
            }

            return value;
        }

        public override bool Store<T>(string key, T value)
        {
            CheckArguments(key, value);

            lock (CachePadlock)
            {
                if (CacheDictionary.ContainsKey(key))
                    CacheDictionary[key] = value;
                else
                    CacheDictionary.Add(key, value);
            }

            return true;
        }

        public override bool Store<T>(string key, T value, DateTime expiresAt)
        {
            CheckArguments(key, value);

            if (expiresAt < DateTime.Now)
                return false;

            lock (CachePadlock)
            {
                if (CacheDictionary.ContainsKey(key))
                    CacheDictionary[key] = value;
                else
                    CacheDictionary.Add(key, value);
            }

            new Thread(() =>
            {
                Thread.Sleep(expiresAt - DateTime.Now);
                Remove(key);
            }).Start();

            return true;
        }

        public override bool Store<T>(string key, T value, TimeSpan validFor)
        {
            CheckArguments(key, value);

            lock (CachePadlock)
            {
                if (CacheDictionary.ContainsKey(key))
                    CacheDictionary[key] = value;
                else
                    CacheDictionary.Add(key, value);
            }

            new Thread(() =>
            {
                Thread.Sleep(validFor);
                Remove(key);
            }).Start();

            return true;
        }

        public override bool Remove(string key)
        {
            CheckArgument(key);
            var removed = false;

            lock (CachePadlock)
            {
                if (CacheDictionary.ContainsKey(key))
                    removed = CacheDictionary.Remove(key);
            }

            return removed;
        }
    }
}
