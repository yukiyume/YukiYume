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

#endregion

namespace YukiYume
{
    /// <summary>
    /// number extension methods
    /// </summary>
    public static class NumberExtensions
    {
        /// <summary>
        /// Transforms the input string to ordinal form
        /// </summary>
        /// <param name="number">number to transform to ordinal form</param>
        /// <returns>string with the appropriate ordinal string added (st, nd, rd, th)</returns>
        public static string ToOrdinal(this int number)
        {
            return Inflector.Ordinalize(number.ToString());
        }

        /// <summary>
        /// Generates a sequence of integers starting from lowerbound up to upperbound inclusive
        /// </summary>
        /// <param name="lowerbound">lowerbound of the sequence</param>
        /// <param name="upperbound">upperbound of the sequence</param>
        /// <returns>A new sequence of integers ranging from lowerbound to upperbound</returns>
        public static IEnumerable<int> UpTo(this int lowerbound, int upperbound)
        {
            var list = new List<int>();

            for (var index = lowerbound; index <= upperbound; index++)
                list.Add(index);

            return list;
        }

        /// <summary>
        /// Generates a sequence of integers starting from lowerbound up to upperbound inclusive
        /// incremented by step 
        /// </summary>
        /// <param name="lowerbound">lowerbound of the sequence</param>
        /// <param name="upperbound">upperbound of the sequence</param>
        /// <param name="step">amount to increment by</param>
        /// <returns>A new sequence of integers ranging from lowerbound to upperbound</returns>
        public static IEnumerable<int> Step(this int lowerbound, int upperbound, int step)
        {
            var list = new List<int>();

            for (var index = lowerbound; index <= upperbound; index += step)
                list.Add(index);

            return list;
        }

        /// <summary>
        /// Generates a sequence of integers starting from upperbound down to lowerbound inclusive
        /// </summary>
        /// <param name="lowerbound">lowerbound of the sequence</param>
        /// <param name="upperbound">upperbound of the sequence</param>
        /// <returns>A new sequence of integers ranging from upperbound to lowerbound</returns>
        public static IEnumerable<int> DownTo(this int upperbound, int lowerbound)
        {
            var list = new List<int>();

            for (var index = upperbound; index >= lowerbound; index--)
                list.Add(index);

            return list;
        }

        /// <summary>
        /// Determines if the number is even
        /// </summary>
        /// <param name="number">number to check</param>
        /// <returns>true if number is even, false otherwise</returns>
        public static bool IsEven(this int number)
        {
            return number % 2 == 0;
        }

        /// <summary>
        /// Returns the input integer + 1
        /// </summary>
        /// <param name="number">number to increment</param>
        /// <returns>number + 1</returns>
        /// <exception cref="System.OverflowException"></exception>
        public static int Next(this int number)
        {
            if (number == int.MaxValue)
                throw new OverflowException("number is maximum value for int");

            return number + 1;
        }

        /// <summary>
        /// Returns the input integer - 1
        /// </summary>
        /// <param name="number">number to decrement</param>
        /// <returns>number - 1</returns>
        /// <exception cref="System.OverflowException"></exception>
        public static int Previous(this int number)
        {
            if (number == int.MinValue)
                throw new OverflowException("number is minimum value for int");

            return number - 1;
        }

        /// <summary>
        /// Determines if the number is odd
        /// </summary>
        /// <param name="number">number to check</param>
        /// <returns>true if number is odd, false otherwise</returns>
        public static bool IsOdd(this int number)
        {
            return !IsEven(number);
        }

        /// <summary>
        /// Processes action number times, passing in values from 0 to number - 1
        /// </summary>
        /// <param name="number">number of times to process action</param>
        /// <param name="action">action to process</param>
        public static void Times(this int number, Action<int> action)
        {
            for (var index = 0; index < number; index++)
                action(index);
        }

        /// <summary>
        /// Processes action number times
        /// </summary>
        /// <param name="number">number of times to process action</param>
        /// <param name="action">action to process</param>
        public static void Times(this int number, Action action)
        {
            for (var index = 0; index < number; index++)
                action();
        }

        /// <summary>
        /// Determines if number is between lowerbound and upperbound, inclusive
        /// </summary>
        /// <param name="number">number to check</param>
        /// <param name="lowerbound">lowerbound</param>
        /// <param name="upperbound">upperbound</param>
        /// <returns>true if number is between lowerbound and upperbound, false otherwise</returns>
        public static bool IsBetween(this int number, int lowerbound, int upperbound)
        {
            return (lowerbound <= number) && (number <= upperbound);
        }
    }
}
