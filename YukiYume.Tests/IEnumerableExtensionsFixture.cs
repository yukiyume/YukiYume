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

#endregion

namespace YukiYume.Tests
{
    [TestFixture]
    public class IEnumerableExtensionsFixture
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(IEnumerableExtensionsFixture));

        private readonly List<string> AnimeList = new List<string>
        {
            "Death Note", "Naruto", "Bleach", "Gundam", "Macross", "Ouran High School Host Club", "Detroit Metal City", "Special A", "The Melancholy of Suzumiya Haruhi"
        };

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void NullListWithActionEach()
        {
            const List<string> nullList = null;
            nullList.Each(animeName => Log.Info(animeName));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullListWithNullActionEach()
        {
            const List<string> nullList = null;
            const Action<string> nullAction = null;
            nullList.Each(nullAction);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void ListWithNullActionEach()
        {
            const Action<string> nullAction = null;
            AnimeList.Each(nullAction);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void NullListWithActionIndexEach()
        {
            const List<string> nullList = null;
            nullList.Each((animeName, index) => Log.Info(animeName));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullListWithNullActionIndexEach()
        {
            const List<string> nullList = null;
            const Action<string, int> nullAction = null;
            nullList.Each(nullAction);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void ListWithNullActionIndexEach()
        {
            const Action<string, int> nullAction = null;
            AnimeList.Each(nullAction);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void NullListWithPredicateEach()
        {
            const List<string> nullList = null;
            nullList.Each((animeName) => true);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullListWithNullPredicateEach()
        {
            const List<string> nullList = null;
            const Predicate<string> nullPredicate = null;
            nullList.Each(nullPredicate);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void ListWithNullPredicateEach()
        {
            const Predicate<string> nullPredicate = null;
            AnimeList.Each(nullPredicate);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void NullListWithPredicateIndexEach()
        {
            const List<string> nullList = null;
            nullList.Each((animeName, index) => true);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullListWithNullPredicateIndexEach()
        {
            const List<string> nullList = null;
            const Func<string, int, bool> nullPredicate = null;
            nullList.Each(nullPredicate);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void ListWithNullPredicateIndexEach()
        {
            const Func<string, int, bool> nullPredicate = null;
            AnimeList.Each(nullPredicate);
        }

        [Test]
        public void ListWithActionEach()
        {
            Assert.That(AnimeList != null);
            var count = 0;

            AnimeList.Each(animeName =>
            {
                Assert.That(animeName != null);
                Assert.That(AnimeList[count], Is.EqualTo(animeName));
                Assert.That(AnimeList[count], Is.SameAs(animeName));
                Log.Info(animeName);
                count++;
            });

            Assert.That(AnimeList.Count, Is.EqualTo(count));
        }

        [Test]
        public void ListWithActionIndexEach()
        {
            Assert.That(AnimeList != null);
            var count = 0;

            AnimeList.Each((animeName, index) =>
            {
                Assert.That(animeName != null);
                Assert.That(AnimeList[index], Is.EqualTo(animeName));
                Assert.That(AnimeList[index], Is.SameAs(animeName));
                Log.Info(animeName);
                count++;
            });

            Assert.That(AnimeList.Count, Is.EqualTo(count));
        }

        [Test]
        public void ListWithPredicateMatchEach()
        {
            Assert.That(AnimeList != null);
            var gundamIndex = 3;
            var count = 0;

            AnimeList.Each(animeName =>
            {
                Assert.That(animeName != null);
                Assert.That(AnimeList[count], Is.EqualTo(animeName));
                Assert.That(AnimeList[count], Is.SameAs(animeName));
                Log.Info(animeName);
                count++;

                return animeName.Equals(AnimeList[gundamIndex]);
            });

            Assert.That(count, Is.EqualTo(gundamIndex + 1));
        }

        [Test]
        public void ListWithPredicateNoMatchEach()
        {
            Assert.That(AnimeList != null);
            var count = 0;

            AnimeList.Each(animeName =>
            {
                Assert.That(animeName != null);
                Assert.That(AnimeList[count], Is.EqualTo(animeName));
                Assert.That(AnimeList[count], Is.SameAs(animeName));
                Log.Info(animeName);
                count++;

                return animeName.Equals("Gundam Wing");
            });

            Assert.That(count, Is.EqualTo(AnimeList.Count));
        }

        [Test]
        public void ListWithPredicateMatchIndexEach()
        {
            Assert.That(AnimeList != null);
            var gundamIndex = 3;
            var count = 0;

            AnimeList.Each((animeName, index) =>
            {
                Assert.That(animeName != null);
                Assert.That(AnimeList[index], Is.EqualTo(animeName));
                Assert.That(AnimeList[index], Is.SameAs(animeName));
                Log.Info(animeName);
                count++;

                return animeName.Equals(AnimeList[gundamIndex]);
            });

            Assert.That(count, Is.EqualTo(gundamIndex + 1));
        }

        [Test]
        public void ListWithPredicateNoMatchIndexEach()
        {
            Assert.That(AnimeList != null);
            var count = 0;

            AnimeList.Each((animeName, index) =>
            {
                Assert.That(animeName != null);
                Assert.That(AnimeList[index], Is.EqualTo(animeName));
                Assert.That(AnimeList[index], Is.SameAs(animeName));
                Log.Info(animeName);
                count++;

                return animeName.Equals("Gundam Wing");
            });

            Assert.That(count, Is.EqualTo(AnimeList.Count));
        }

        [Test]
        public void Reduce()
        {
            var sum = 5.UpTo(10).Reduce((int result, int n) => result + n);

            Assert.That(sum == 45);
        }
    }
}

