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

namespace YukiYume
{
    /// <summary>
    /// Generic Singleton class
    /// </summary>
    /// <typeparam name="T">type of class to create a singleton of</typeparam>
    public class Singleton<T> where T : new()
    {
        /// <summary>
        /// static member to hold the single instance of T
        /// </summary>
        private static readonly T _instance = new T();

        /// <summary>
        /// static constructor to make sure no beforefieldinit
        /// </summary>
        static Singleton()
        {
        }

        /// <summary>
        /// constructor is private to control creation of new Singleton objects
        /// </summary>
        private Singleton()
        {
        }

        /// <summary>
        /// Returns the single instance of type T
        /// if this property has not been called before, a new object of type T will be created
        /// </summary>
        public static T Instance
        {
            get { return _instance; }
        }
    }
}
