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
using YukiYume.Json;

#endregion

namespace YukiYume.Tests.Json
{
    // various classes to help with testing serialization/deserialization

    public class SomeOtherClass
    {
        public int PublicField;
        private int PrivateField;
    }

    public class SomeDateClass
    {
        [JsonName("created_at")]
        public DateTime CreatedAt { get; set; }
    }

    public class Nested
    {
        [JsonName("nested_two")]
        public NestedTwo NestedTwo { get; set; }
    }

    public class NestedTwo
    {
        [JsonName("nested_three")]
        public NestedThree NestedThree { get; set; }

        [JsonName("property_one")]
        public string PropertOne { get; set; }

        [JsonName("property_two")]
        public int PropertyTwo { get; set; }
    }

    public class NestedThree
    {
        [JsonName("property_three")]
        public string PropertyThree { get; set; }

        [JsonName("property_four")]
        public int PropertyFour { get; set; }
    }

    public class ObjectWithArray
    {
        public string PropertyOne { get; set; }
        public int[] Numbers { get; set; }
    }

    public class SomeClass
    {
        [JsonIgnore]
        public string PropertyOne { get; set; }

        [JsonName("propertyTwo")]
        public int PropertyTwo { get; set; }

        public short PropertyThree { get; set; }
        public long PropertyFour { get; set; }
        public bool PropertyFive { get; set; }
        public object PropertySix { get; set; }
    }
}
