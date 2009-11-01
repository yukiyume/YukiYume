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
using System.Web;

#endregion

namespace YukiYume.Caching
{
    /// <summary>
    /// Implements Cache using the ASP.NET cache
    /// </summary>
    public class AspNetCache : Cache
    {
        public override T Get<T>(string key)
        {
            CheckArgument(key);

            return HttpContext.Current.Cache.Get(key) as T;
        }

        public override bool Store<T>(string key, T value)
        {
            CheckArguments(key, value);

            HttpContext.Current.Cache.Insert(key, value);

            return true;
        }

        public override bool Store<T>(string key, T value, DateTime expiresAt)
        {
            CheckArguments(key, value);

            if (expiresAt < DateTime.Now)
                return false;

            HttpContext.Current.Cache.Insert(key, value, null, expiresAt, System.Web.Caching.Cache.NoSlidingExpiration);

            return true;
        }

        public override bool Store<T>(string key, T value, TimeSpan validFor)
        {
            CheckArguments(key, value);

            HttpContext.Current.Cache.Insert(key, value, null, System.Web.Caching.Cache.NoAbsoluteExpiration, validFor);

            return true;
        }

        public override bool Remove(string key)
        {
            CheckArgument(key);

            return HttpContext.Current.Cache.Remove(key) != null;
        }
    }
}
