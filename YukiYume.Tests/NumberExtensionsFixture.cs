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
using log4net;
using NUnit.Framework;
using YukiYume.Logging;

#endregion

namespace YukiYume.Tests
{
    [TestFixture]
    public class NumberExtensionsFixture
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(StringExtensionsFixture));

        #region ToOrdinal

        [Test]
        public void ToOrdinal()
        {
            var numbers = new List<Pair<int, string>>
            {
                new Pair<int, string> { First = 1, Second = "1st" },
                new Pair<int, string> { First = 2, Second = "2nd" },
                new Pair<int, string> { First = 1002, Second = "1002nd" },
                new Pair<int, string> { First = 1003, Second = "1003rd" },
                new Pair<int, string> { First = 10013, Second = "10013th" },
                new Pair<int, string> { First = 20026, Second = "20026th" }
            };

            numbers.Each(pair =>
            {
                Log.Info("{0} -> {1} = {2}?", pair.First, pair.First.ToOrdinal(), pair.Second);
                Assert.That(pair.First.ToOrdinal(), Is.EqualTo(pair.Second));
            });
        }

        #endregion

        #region UpTo

        [Test]
        public void UpToPositiveRange()
        {
            var count = 0;
            1.UpTo(10).Each(index => Assert.That(index == ++count));
            Assert.That(count == 10);
        }

        [Test]
        public void UpToBadRange()
        {
            var count = 0;
            10.UpTo(1).Each(index => count++);

            Assert.That(count == 0);
        }

        [Test]
        public void UpToNegativeRange()
        {
            var count = -11;
            (-10).UpTo(10).Each(index => Assert.That(index == ++count));
            Assert.That(count == 10);
        }

        #endregion

        #region DownTo

        [Test]
        public void DownToPositiveRange()
        {
            var count = 21;
            20.DownTo(10).Each(index => Assert.That(index == --count));
            Assert.That(count == 10);
        }

        [Test]
        public void DownToBadRange()
        {
            var count = 0;
            10.DownTo(20).Each(index => count++);

            Assert.That(count == 0);
        }

        [Test]
        public void DownToNegativeRange()
        {
            var count = -9;
            (-10).DownTo(-20).Each(index => Assert.That(index == --count));
            Assert.That(count == -20);
        }

        #endregion

        #region EvenOdd

        [Test]
        public void IsEven()
        {
            Assert.That((-2).IsEven());
            Assert.That(0.IsEven());
            Assert.That(2.IsEven());

        }

        [Test]
        public void IsOdd()
        {
            Assert.That((-1).IsOdd());
            Assert.That(1.IsOdd());

        }

        #endregion

        #region Step

        [Test]
        public void StepPositiveRange()
        {
            var count = 0;
            2.Step(10, 2).Each(index => 
            {
                count += 2;
                Assert.That(index == count);
            });

            Assert.That(count == 10);
        }

        [Test]
        public void StepBadRange()
        {
            var count = 0;
            10.Step(2, -2).Each(index => count++);

            Assert.That(count == 0);
        }

        [Test]
        public void StepNegativeRange()
        {
            var count = -12;
            (-10).Step(10, 2).Each(index =>
            {
                count += 2;
                Assert.That(index == count);
            });

            Assert.That(count == 10);
        }

        #endregion

        #region Times

        [Test]
        public void TimesPostive()
        {
            var count = 0;
            20.Times(index => count++);
            Assert.That(count == 20);
        }

        [Test]
        public void TimesZero()
        {
            var count = 0;
            0.Times(index => count++);
            Assert.That(count == 0);
        }

        [Test]
        public void TimesNegative()
        {
            var count = 0;
            (-20).Times(index => count++);
            Assert.That(count == 0);
        }

        #endregion

        #region Next

        [Test]
        public void NextPositive()
        {
            Assert.That(1.Next() == 2);
        }

        [Test]
        public void NextZero()
        {
            Assert.That(0.Next() == 1);
        }

        [Test]
        public void NextNegative()
        {
            Assert.That((-1).Next() == 0);
        }

        [Test, ExpectedException(typeof(OverflowException))]
        public void NextMax()
        {
            Assert.That(int.MaxValue.Next() > 0);
        }

        #endregion

        #region Previous

        [Test]
        public void PreviousPositive()
        {
            Assert.That(1.Previous() == 0);
        }

        [Test]
        public void PreviousZero()
        {
            Assert.That(0.Previous() == -1);
        }

        [Test]
        public void PreviousNegative()
        {
            Assert.That((-1).Previous() == -2);
        }

        [Test, ExpectedException(typeof(OverflowException))]
        public void PreviousMin()
        {
            Assert.That(int.MinValue.Previous() > 0);
        }

        #endregion

        #region InBetween

        [Test]
        public void IsBetween()
        {
            Assert.That(5.IsBetween(0, 10));
        }

        [Test]
        public void IsBetweenOnLowerbound()
        {
            Assert.That(5.IsBetween(5, 10));
        }

        [Test]
        public void IsBetweenOnUpperbound()
        {
            Assert.That(10.IsBetween(5, 10));
        }

        [Test]
        public void IsNotBetween()
        {
            Assert.That(!5.IsBetween(8, 10));
        }

        #endregion
    }
}
