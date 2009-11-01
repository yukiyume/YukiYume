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
using System.Configuration;
using NUnit.Framework;

#endregion

namespace YukiYume.Tests.Configuration
{
    [TestFixture]
    public class ConfigurationFixture
    {
        private YukiYumeTestsSectionHandler ConfigSection { get; set; }

        [SetUp]
        public void Setup()
        {
            ConfigSection = ConfigurationManager.GetSection(("YukiYume.Tests")) as YukiYumeTestsSectionHandler;
        }

        [Test]
        public void GetConfig()
        {
            Assert.That(ConfigSection != null);
        }

        [Test]
        public void GetSettings()
        {
            Assert.That(ConfigSection != null);
            Assert.That(ConfigSection.Settings != null);
            Assert.That(ConfigSection.Settings.Count > 0);
        }

        [Test]
        public void GetSettingElements()
        {
            Assert.That(ConfigSection != null);
            Assert.That(ConfigSection.Settings != null);
            Assert.That(ConfigSection.Settings.Count > 0);

            ConfigSection.Settings.Each(setting =>
            {
                Assert.That(!string.IsNullOrEmpty(setting.Name));
                Assert.That(!string.IsNullOrEmpty(setting.Value));
            });
        }

        [Test]
        public void GetOther()
        {
            Assert.That(ConfigSection != null);
            Assert.That(ConfigSection.Other != null);
            Assert.That(ConfigSection.Other.IsPlaceHolder);
        }

        [Test]
        public  void GetOptions()
        {
            Assert.That(ConfigSection != null);
            Assert.That(ConfigSection.Other != null);
            Assert.That(ConfigSection.Other.Options != null);
            Assert.That(ConfigSection.Other.Options.IsOption);
        }
    }
}
