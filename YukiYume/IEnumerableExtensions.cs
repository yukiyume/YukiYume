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
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;

#endregion

namespace YukiYume
{
    /// <summary>
    /// Container class for extensions to IEnumerable, ICollection
    /// </summary>
    public static class IEnumerableExtensions
    {
        #region Each

        /// <summary>
        /// Similar to System.Collections.Generic.List.ForEach, but will work with IEnumerable
        /// Performs the action specified by fun on all elements in the collection
        /// </summary>
        /// <typeparam name="T">Type of the the collection</typeparam>
        /// <param name="collection">IEnumerable collection to perform actions on</param>
        /// <param name="action">action to perform on the collection</param>
        public static IEnumerable<T> Each<T>(this IEnumerable<T> collection, Action<T> action)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            if (action == null)
                throw new ArgumentNullException("action");

            foreach (var item in collection)
                action(item);

            return collection;
        }

        public static IEnumerable Each(this IEnumerable collection, Action action)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            if (action == null)
                throw new ArgumentNullException("action");

            foreach (var item in collection)
                action();

            return collection;
        }

        public static IEnumerable<T> Each<T>(this IEnumerable<T> collection, Action<T, int> action)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            if (action == null)
                throw new ArgumentNullException("action");

            var index = 0;

            foreach (var item in collection)
                action(item, index++);

            return collection;
        }

        public static IEnumerable<T> Each<T>(this IEnumerable<T> collection, Predicate<T> predicate)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            if (predicate == null)
                throw new ArgumentNullException("predicate");

            foreach (var item in collection)
                if (predicate(item))
                    break;

            return collection;
        }

        public static IEnumerable<T> Each<T>(this IEnumerable<T> collection, Func<T, int, bool> func)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            if (func == null)
                throw new ArgumentNullException("func");

            var index = 0;

            foreach (var item in collection)
                if (func(item, index++))
                    break;

            return collection;
        }

        #endregion

        #region EachReverse

        public static IEnumerable<T> EachReverse<T>(this IEnumerable<T> collection, Action<T> action)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            if (action == null)
                throw new ArgumentNullException("action");

            var stack = new Stack<T>();

            foreach (var item in collection)
                stack.Push(item);

            while (stack.Count > 0)
                action(stack.Pop());

            return collection;
        }

        public static IEnumerable EachReverse(this IEnumerable collection, Action action)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            if (action == null)
                throw new ArgumentNullException("action");

            var stack = new Stack();

            foreach (var item in collection)
                stack.Push(item);

            while (stack.Count > 0)
                action();

            return collection;
        }

        public static IEnumerable<T> EachReverse<T>(this IEnumerable<T> collection, Action<T, int> action)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            if (action == null)
                throw new ArgumentNullException("action");

            var stack = new Stack<T>();

            foreach (var item in collection)
                stack.Push(item);

            var index = stack.Count - 1;

            while (stack.Count > 0)
                action(stack.Pop(), index--);

            return collection;
        }

        public static IEnumerable<T> EachReverse<T>(this IEnumerable<T> collection, Predicate<T> predicate)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            if (predicate == null)
                throw new ArgumentNullException("predicate");

            var stack = new Stack<T>();

            foreach (var item in collection)
                stack.Push(item);

            while (stack.Count > 0)
                if (predicate(stack.Pop()))
                    break;

            return collection;
        }

        public static IEnumerable<T> EachReverse<T>(this IEnumerable<T> collection, Func<T, int, bool> func)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            if (func == null)
                throw new ArgumentNullException("func");

            var stack = new Stack<T>();

            foreach (var item in collection)
                stack.Push(item);

            var index = stack.Count - 1;

            while (stack.Count > 0)
                if (func(stack.Pop(), index--))
                    break;

            return collection;
        }

        #endregion

        #region IsValueIn

        public static bool IsValueIn(this IEnumerable<string> collection, string value)
        {
            var result = false;

            foreach (var item in collection)
                if (string.Compare(item, value) == 0)
                {
                    result = true;
                    break;
                }

            return result;
        }

        public static bool IsValueIn(this IEnumerable<string> collection, string value, bool ignoreCase)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            if (value == null)
                throw new ArgumentNullException("value");

            var result = false;

            foreach (var item in collection)
                if (string.Compare(item, value, ignoreCase) == 0)
                {
                    result = true;
                    break;
                }

            return result;
        }

        #endregion

        #region Add

        public static void Add(this ICollection<string> collection, string format, params object[] args)
        {
            collection.Add(string.Format(format, args));
        }

        #endregion

        #region Map

        // alias for IEnumerable<T>.Select
        public static IEnumerable<TResult> Map<T, TResult>(this IEnumerable<T> collection, Func<T, TResult> func)
        {
            return collection.Select(func);
        }

        // alias for IEnumerable<T>.Select
        public static IEnumerable<TResult> Map<T, TResult>(this IEnumerable<T> collection, Func<T, int, TResult> func)
        {
            return collection.Select(func);
        }

        #endregion

        #region Reduce

        public static TResult Reduce<T, TResult>(this IEnumerable<T> collection, Func<TResult, T, TResult> func)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            if (func == null)
                throw new ArgumentNullException("func");

            var result = default(TResult);

            foreach (var item in collection)
                result = func(result, item);

            return result;
        }

        public static TResult ReduceReverse<T, TResult>(this IEnumerable<T> collection, Func<TResult, T, TResult> func)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            if (func == null)
                throw new ArgumentNullException("func");

            var result = default(TResult);

            var stack = new Stack<T>();

            foreach (var item in collection)
                stack.Push(item);

            while (stack.Count > 0)
                result = func(result, stack.Pop());

            return result;
        }

        #endregion

        #region Grep

        public static IEnumerable<T> Grep<T>(this IEnumerable<T> collection, string regExp)
        {
            return Grep(collection, regExp, false);
        }

        public static IEnumerable<T> Grep<T>(this IEnumerable<T> collection, string regExp, bool ignoreCase)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            if (regExp == null)
                throw new ArgumentNullException("regExp");

            var regex = ignoreCase ? new Regex(regExp, RegexOptions.IgnoreCase) : new Regex(regExp);
            var list = new List<T>();

            foreach (var item in collection)
                if (regex.IsMatch(regExp))
                    list.Add(item);

            return list;
        }

        #endregion
    }
}
