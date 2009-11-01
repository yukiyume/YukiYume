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
using YukiYume.Logging;

#endregion

namespace YukiYume.Json
{
    /// <summary>
    /// handbuilt LR JSON parser using an SLR table
    /// see the dragon book or wikipedia for more information on LR parsers
    /// </summary>
    public static class JsonParser
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(JsonParser));

        #region Parser Support

        private enum ShiftReduceType
        {
            Error,
            Shift,
            Reduce,
            Accept,
            Goto
        }

        private class ShiftReduce
        {
            public ShiftReduceType Type { get; set; }
            public int Action { get; set; }
        }

        private enum GotoType
        {
            None = -1,
            S = 12,
            O = 13,
            A = 14,
            M = 15,
            P = 16,
            E = 17,
            V = 18
        }

        private static readonly int[] ReduceLengths =
        {
            1, // rule #0:  S’ → S
            1, // rule #1:  S  → O
            1, // rule #2:  S  → A
            2, // rule #3:  O  → { }
            3, // rule #4:  O  → { M }
            3, // rule #5:  M  → P, M
            1, // rule #6:  M  → P
            3, // rule #7:  P  → string : V
            3, // rule #8:  A  → [ E ]
            2, // rule #9:  A  → [ ]
            3, // rule #10: E  → V, E
            1, // rule #11: E  → V
            1, // rule #12: V  → string
            1, // rule #13: V  → number
            1, // rule #14: V  → O
            1, // rule #15: V  → A
            1, // rule #16: V  → true
            1, // rule #17: V  → false
            1  // rule #18: V  → null
        };

        private static readonly int[] ReduceLeftHandSides = 
        {
            (int)GotoType.None, // rule #0:  S’ → S
            (int)GotoType.S,    // rule #1:  S  → O
            (int)GotoType.S,    // rule #2:  S  → A
            (int)GotoType.O,    // rule #3:  O  → { }
            (int)GotoType.O,    // rule #4:  O  → { M }
            (int)GotoType.M,    // rule #5:  M  → P, M
            (int)GotoType.M,    // rule #6:  M  → P
            (int)GotoType.P,    // rule #7:  P  → string : V
            (int)GotoType.A,    // rule #8:  A  → [ E ]
            (int)GotoType.A,    // rule #9:  A  → [ ]
            (int)GotoType.E,    // rule #10: E  → V, E
            (int)GotoType.E,    // rule #11: E  → V
            (int)GotoType.V,    // rule #12: V  → string
            (int)GotoType.V,    // rule #13: V  → number
            (int)GotoType.V,    // rule #14: V  → O
            (int)GotoType.V,    // rule #15: V  → A
            (int)GotoType.V,    // rule #16: V  → true
            (int)GotoType.V,    // rule #17: V  → false
            (int)GotoType.V     // rule #18: V  → null
        };

        private enum ProductionType
        {
            Accept = 0,
            Object,
            Array,
            EmptyObject,
            NonEmptyObject,
            MemberMultiple,
            MemberSingle,
            Pair,
            NonEmptyArray,
            EmptyArray,
            ElementMultiple,
            ElementSingle,
            StringValue,
            NumberValue,
            ObjectValue,
            ArrayValue,
            TrueValue,
            FalseValue,
            NullValue
        }

        private static readonly string[] Productions = 
        {
            "S’ → S",
            "S  → O",
            "S  → A",
            "O  → { }",
            "O  → { M }",
            "M  → P, M",
            "M  → P",
            "P  → string : V",
            "A  → [ E ]",
            "A  → [ ]",
            "E  → V, E",
            "E  → V",
            "V  → string",
            "V  → number",
            "V  → O",
            "V  → A",
            "V  → true",
            "V  → false",
            "V  → null"
        };

        private static readonly ShiftReduce ShiftReduceError = new ShiftReduce { Type = ShiftReduceType.Error, Action = -1 };

        #region ActionGotoTable

        private static readonly ShiftReduce[,] ActionGotoTable = 
        {
            { // state 0
                new ShiftReduce { Type = ShiftReduceType.Shift, Action = 4 },             // {
                ShiftReduceError,                                                         // }
                new ShiftReduce { Type = ShiftReduceType.Shift, Action = 5 },             // [
                ShiftReduceError,                                                         // ]
                ShiftReduceError,                                                         // ,
                ShiftReduceError,                                                         // :
                ShiftReduceError,                                                         // string
                ShiftReduceError,                                                         // number
                ShiftReduceError,                                                         // true 
                ShiftReduceError,                                                         // false
                ShiftReduceError,                                                         // null
                ShiftReduceError,                                                         // $
                new ShiftReduce { Type = ShiftReduceType.Goto, Action = 1 },              // S
                new ShiftReduce { Type = ShiftReduceType.Goto, Action = 2 },              // O
                new ShiftReduce { Type = ShiftReduceType.Goto, Action = 3 },              // A
                ShiftReduceError,                                                         // M
                ShiftReduceError,                                                         // P
                ShiftReduceError,                                                         // E
                ShiftReduceError                                                          // V
            },
            { // state 1
                ShiftReduceError,                                                         // {
                ShiftReduceError,                                                         // }
                ShiftReduceError,                                                         // [
                ShiftReduceError,                                                         // ]
                ShiftReduceError,                                                         // ,
                ShiftReduceError,                                                         // :
                ShiftReduceError,                                                         // string
                ShiftReduceError,                                                         // number
                ShiftReduceError,                                                         // true 
                ShiftReduceError,                                                         // false
                ShiftReduceError,                                                         // null
                new ShiftReduce { Type = ShiftReduceType.Accept },                         // $
                ShiftReduceError,                                                         // S
                ShiftReduceError,                                                         // O
                ShiftReduceError,                                                         // A
                ShiftReduceError,                                                         // M
                ShiftReduceError,                                                         // P
                ShiftReduceError,                                                         // E
                ShiftReduceError                                                          // V
            },
            { // state 2
                ShiftReduceError,                                                         // {
                ShiftReduceError,                                                         // }
                ShiftReduceError,                                                         // [
                ShiftReduceError,                                                         // ]
                ShiftReduceError,                                                         // ,
                ShiftReduceError,                                                         // :
                ShiftReduceError,                                                         // string
                ShiftReduceError,                                                         // number
                ShiftReduceError,                                                         // true 
                ShiftReduceError,                                                         // false
                ShiftReduceError,                                                         // null
                new ShiftReduce { Type = ShiftReduceType.Reduce, Action = 1 },            // $
                ShiftReduceError,                                                         // S
                ShiftReduceError,                                                         // O
                ShiftReduceError,                                                         // A
                ShiftReduceError,                                                         // M
                ShiftReduceError,                                                         // P
                ShiftReduceError,                                                         // E
                ShiftReduceError                                                          // V
            },
            { // state 3
                ShiftReduceError,                                                         // {
                ShiftReduceError,                                                         // }
                ShiftReduceError,                                                         // [
                ShiftReduceError,                                                         // ]
                ShiftReduceError,                                                         // ,
                ShiftReduceError,                                                         // :
                ShiftReduceError,                                                         // string
                ShiftReduceError,                                                         // number
                ShiftReduceError,                                                         // true 
                ShiftReduceError,                                                         // false
                ShiftReduceError,                                                         // null
                new ShiftReduce { Type = ShiftReduceType.Reduce, Action = 2 },            // $
                ShiftReduceError,                                                         // S
                ShiftReduceError,                                                         // O
                ShiftReduceError,                                                         // A
                ShiftReduceError,                                                         // M
                ShiftReduceError,                                                         // P
                ShiftReduceError,                                                         // E
                ShiftReduceError                                                          // V
            },
            { // state 4
                ShiftReduceError,                                                         // {
                new ShiftReduce { Type = ShiftReduceType.Shift, Action = 6 },             // }
                ShiftReduceError,                                                         // [
                ShiftReduceError,                                                         // ]
                ShiftReduceError,                                                         // ,
                ShiftReduceError,                                                         // :
                new ShiftReduce { Type = ShiftReduceType.Shift, Action = 9 },             // string
                ShiftReduceError,                                                         // number
                ShiftReduceError,                                                         // true 
                ShiftReduceError,                                                         // false
                ShiftReduceError,                                                         // null
                ShiftReduceError,                                                         // $
                ShiftReduceError,                                                         // S
                ShiftReduceError,                                                         // O
                ShiftReduceError,                                                         // A
                new ShiftReduce { Type = ShiftReduceType.Goto, Action = 7 },              // M
                new ShiftReduce { Type = ShiftReduceType.Goto, Action = 8 },              // P
                ShiftReduceError,                                                         // E
                ShiftReduceError                                                          // V
            }
            ,
            { // state 5
                new ShiftReduce { Type = ShiftReduceType.Shift, Action = 4 },             // {
                ShiftReduceError,                                                         // }
                new ShiftReduce { Type = ShiftReduceType.Shift, Action = 5 },             // [
                new ShiftReduce { Type = ShiftReduceType.Shift, Action = 12 },            // ]
                ShiftReduceError,                                                         // ,
                ShiftReduceError,                                                         // :
                new ShiftReduce { Type = ShiftReduceType.Shift, Action = 17 },            // string
                new ShiftReduce { Type = ShiftReduceType.Shift, Action = 18 },            // number
                new ShiftReduce { Type = ShiftReduceType.Shift, Action = 19 },            // true 
                new ShiftReduce { Type = ShiftReduceType.Shift, Action = 20 },            // false
                new ShiftReduce { Type = ShiftReduceType.Shift, Action = 21 },            // null
                ShiftReduceError,                                                         // $
                ShiftReduceError,                                                         // S
                new ShiftReduce { Type = ShiftReduceType.Goto, Action = 14 },             // O
                new ShiftReduce { Type = ShiftReduceType.Goto, Action = 15 },             // A
                ShiftReduceError,                                                         // M
                ShiftReduceError,                                                         // P
                new ShiftReduce { Type = ShiftReduceType.Goto, Action = 13 },             // E
                new ShiftReduce { Type = ShiftReduceType.Goto, Action = 16 }              // V
            }
            ,
            { // state 6
                ShiftReduceError,                                                         // {
                new ShiftReduce { Type = ShiftReduceType.Reduce, Action = 3 },            // }
                ShiftReduceError,                                                         // [
                new ShiftReduce { Type = ShiftReduceType.Reduce, Action = 3 },            // ]
                new ShiftReduce { Type = ShiftReduceType.Reduce, Action = 3 },            // ,
                ShiftReduceError,                                                         // :
                ShiftReduceError,                                                         // string
                ShiftReduceError,                                                         // number
                ShiftReduceError,                                                         // true 
                ShiftReduceError,                                                         // false
                ShiftReduceError,                                                         // null
                new ShiftReduce { Type = ShiftReduceType.Reduce, Action = 3 },            // $
                ShiftReduceError,                                                         // S
                ShiftReduceError,                                                         // O
                ShiftReduceError,                                                         // A
                ShiftReduceError,                                                         // M
                ShiftReduceError,                                                         // P
                ShiftReduceError,                                                         // E
                ShiftReduceError                                                          // V
            }
            ,
            { // state 7
                ShiftReduceError,                                                         // {
                new ShiftReduce { Type = ShiftReduceType.Shift, Action = 27},             // }
                ShiftReduceError,                                                         // [
                ShiftReduceError,                                                         // ]
                ShiftReduceError,                                                         // ,
                ShiftReduceError,                                                         // :
                ShiftReduceError,                                                         // string
                ShiftReduceError,                                                         // number
                ShiftReduceError,                                                         // true 
                ShiftReduceError,                                                         // false
                ShiftReduceError,                                                         // null
                ShiftReduceError,                                                         // $
                ShiftReduceError,                                                         // S
                ShiftReduceError,                                                         // O
                ShiftReduceError,                                                         // A
                ShiftReduceError,                                                         // M
                ShiftReduceError,                                                         // P
                ShiftReduceError,                                                         // E
                ShiftReduceError                                                          // V
            }
            ,
            { // state 8
                ShiftReduceError,                                                         // {
                new ShiftReduce { Type = ShiftReduceType.Reduce, Action = 6 },            // }
                ShiftReduceError,                                                         // [
                ShiftReduceError,                                                         // ]
                new ShiftReduce { Type = ShiftReduceType.Shift, Action = 10 },            // ,
                ShiftReduceError,                                                         // :
                ShiftReduceError,                                                         // string
                ShiftReduceError,                                                         // number
                ShiftReduceError,                                                         // true 
                ShiftReduceError,                                                         // false
                ShiftReduceError,                                                         // null
                ShiftReduceError,                                                         // $
                ShiftReduceError,                                                         // S
                ShiftReduceError,                                                         // O
                ShiftReduceError,                                                         // A
                ShiftReduceError,                                                         // M
                ShiftReduceError,                                                         // P
                ShiftReduceError,                                                         // E
                ShiftReduceError                                                          // V
            }
            ,
            { // state 9
                ShiftReduceError,                                                         // {
                ShiftReduceError,                                                         // }
                ShiftReduceError,                                                         // [
                ShiftReduceError,                                                         // ]
                ShiftReduceError,                                                         // ,
                new ShiftReduce { Type = ShiftReduceType.Shift, Action = 23 },            // :
                ShiftReduceError,                                                         // string
                ShiftReduceError,                                                         // number
                ShiftReduceError,                                                         // true 
                ShiftReduceError,                                                         // false
                ShiftReduceError,                                                         // null
                ShiftReduceError,                                                         // $
                ShiftReduceError,                                                         // S
                ShiftReduceError,                                                         // O
                ShiftReduceError,                                                         // A
                ShiftReduceError,                                                         // M
                ShiftReduceError,                                                         // P
                ShiftReduceError,                                                         // E
                ShiftReduceError                                                          // V
            }
            ,
            { // state 10
                ShiftReduceError,                                                         // {
                ShiftReduceError,                                                         // }
                ShiftReduceError,                                                         // [
                ShiftReduceError,                                                         // ]
                ShiftReduceError,                                                         // ,
                ShiftReduceError,                                                         // :
                new ShiftReduce { Type = ShiftReduceType.Shift, Action = 9 },             // string
                ShiftReduceError,                                                         // number
                ShiftReduceError,                                                         // true 
                ShiftReduceError,                                                         // false
                ShiftReduceError,                                                         // null
                ShiftReduceError,                                                         // $
                ShiftReduceError,                                                         // S
                ShiftReduceError,                                                         // O
                ShiftReduceError,                                                         // A
                new ShiftReduce { Type = ShiftReduceType.Goto, Action = 11 },             // M
                new ShiftReduce { Type = ShiftReduceType.Goto, Action = 8 },              // P
                ShiftReduceError,                                                         // E
                ShiftReduceError                                                          // V
            }
            ,
            { // state 11
                ShiftReduceError,                                                         // {
                new ShiftReduce { Type = ShiftReduceType.Reduce, Action = 5 },            // }
                ShiftReduceError,                                                         // [
                ShiftReduceError,                                                         // ]
                ShiftReduceError,                                                         // ,
                ShiftReduceError,                                                         // :
                ShiftReduceError,                                                         // string
                ShiftReduceError,                                                         // number
                ShiftReduceError,                                                         // true 
                ShiftReduceError,                                                         // false
                ShiftReduceError,                                                         // null
                ShiftReduceError,                                                         // $
                ShiftReduceError,                                                         // S
                ShiftReduceError,                                                         // O
                ShiftReduceError,                                                         // A
                ShiftReduceError,                                                         // M
                ShiftReduceError,                                                         // P
                ShiftReduceError,                                                         // E
                ShiftReduceError                                                          // V
            }
            ,
            { // state 12
                ShiftReduceError,                                                         // {
                new ShiftReduce { Type = ShiftReduceType.Reduce, Action = 9 },            // }
                ShiftReduceError,                                                         // [
                new ShiftReduce { Type = ShiftReduceType.Reduce, Action = 9 },            // ]
                new ShiftReduce { Type = ShiftReduceType.Reduce, Action = 9 },            // ,
                ShiftReduceError,                                                         // :
                ShiftReduceError,                                                         // string
                ShiftReduceError,                                                         // number
                ShiftReduceError,                                                         // true 
                ShiftReduceError,                                                         // false
                ShiftReduceError,                                                         // null
                new ShiftReduce { Type = ShiftReduceType.Reduce, Action = 9 },            // $
                ShiftReduceError,                                                         // S
                ShiftReduceError,                                                         // O
                ShiftReduceError,                                                         // A
                ShiftReduceError,                                                         // M
                ShiftReduceError,                                                         // P
                ShiftReduceError,                                                         // E
                ShiftReduceError                                                          // V
            }
            ,
            { // state 13
                ShiftReduceError,                                                         // {
                ShiftReduceError,                                                         // }
                ShiftReduceError,                                                         // [
                new ShiftReduce { Type = ShiftReduceType.Shift, Action = 22 },            // ]
                ShiftReduceError,                                                         // ,
                ShiftReduceError,                                                         // :
                ShiftReduceError,                                                         // string
                ShiftReduceError,                                                         // number
                ShiftReduceError,                                                         // true 
                ShiftReduceError,                                                         // false
                ShiftReduceError,                                                         // null
                ShiftReduceError,                                                         // $
                ShiftReduceError,                                                         // S
                ShiftReduceError,                                                         // O
                ShiftReduceError,                                                         // A
                ShiftReduceError,                                                         // M
                ShiftReduceError,                                                         // P
                ShiftReduceError,                                                         // E
                ShiftReduceError                                                          // V
            }
            ,
            { // state 14
                ShiftReduceError,                                                         // {
                new ShiftReduce { Type = ShiftReduceType.Reduce, Action = 14 },           // }
                ShiftReduceError,                                                         // [
                new ShiftReduce { Type = ShiftReduceType.Reduce, Action = 14 },           // ]
                new ShiftReduce { Type = ShiftReduceType.Reduce, Action = 14 },           // ,
                ShiftReduceError,                                                         // :
                ShiftReduceError,                                                         // string
                ShiftReduceError,                                                         // number
                ShiftReduceError,                                                         // true 
                ShiftReduceError,                                                         // false
                ShiftReduceError,                                                         // null
                ShiftReduceError,                                                         // $
                ShiftReduceError,                                                         // S
                ShiftReduceError,                                                         // O
                ShiftReduceError,                                                         // A
                ShiftReduceError,                                                         // M
                ShiftReduceError,                                                         // P
                ShiftReduceError,                                                         // E
                ShiftReduceError                                                          // V
            }
            ,
            { // state 15
                ShiftReduceError,                                                         // {
                new ShiftReduce { Type = ShiftReduceType.Reduce, Action = 15 },           // }
                ShiftReduceError,                                                         // [
                new ShiftReduce { Type = ShiftReduceType.Reduce, Action = 15 },           // ]
                new ShiftReduce { Type = ShiftReduceType.Reduce, Action = 15 },           // ,
                ShiftReduceError,                                                         // :
                ShiftReduceError,                                                         // string
                ShiftReduceError,                                                         // number
                ShiftReduceError,                                                         // true 
                ShiftReduceError,                                                         // false
                ShiftReduceError,                                                         // null
                ShiftReduceError,                                                         // $
                ShiftReduceError,                                                         // S
                ShiftReduceError,                                                         // O
                ShiftReduceError,                                                         // A
                ShiftReduceError,                                                         // M
                ShiftReduceError,                                                         // P
                ShiftReduceError,                                                         // E
                ShiftReduceError                                                          // V
            }
            ,
            { // state 16
                ShiftReduceError,                                                         // {
                ShiftReduceError,                                                         // }
                ShiftReduceError,                                                         // [
                new ShiftReduce { Type = ShiftReduceType.Reduce, Action = 11 },           // ]
                new ShiftReduce { Type = ShiftReduceType.Shift, Action = 25 },            // ,
                ShiftReduceError,                                                         // :
                ShiftReduceError,                                                         // string
                ShiftReduceError,                                                         // number
                ShiftReduceError,                                                         // true 
                ShiftReduceError,                                                         // false
                ShiftReduceError,                                                         // null
                ShiftReduceError,                                                         // $
                ShiftReduceError,                                                         // S
                ShiftReduceError,                                                         // O
                ShiftReduceError,                                                         // A
                ShiftReduceError,                                                         // M
                ShiftReduceError,                                                         // P
                ShiftReduceError,                                                         // E
                ShiftReduceError                                                          // V
            }
            ,
            { // state 17
                ShiftReduceError,                                                         // {
                new ShiftReduce { Type = ShiftReduceType.Reduce, Action = 12 },           // }
                ShiftReduceError,                                                         // [
                new ShiftReduce { Type = ShiftReduceType.Reduce, Action = 12 },           // ]
                new ShiftReduce { Type = ShiftReduceType.Reduce, Action = 12 },           // ,
                ShiftReduceError,                                                         // :
                ShiftReduceError,                                                         // string
                ShiftReduceError,                                                         // number
                ShiftReduceError,                                                         // true 
                ShiftReduceError,                                                         // false
                ShiftReduceError,                                                         // null
                ShiftReduceError,                                                         // $
                ShiftReduceError,                                                         // S
                ShiftReduceError,                                                         // O
                ShiftReduceError,                                                         // A
                ShiftReduceError,                                                         // M
                ShiftReduceError,                                                         // P
                ShiftReduceError,                                                         // E
                ShiftReduceError                                                          // V
            }
            ,
            { // state 18
                ShiftReduceError,                                                         // {
                new ShiftReduce { Type = ShiftReduceType.Reduce, Action = 13 },           // }
                ShiftReduceError,                                                         // [
                new ShiftReduce { Type = ShiftReduceType.Reduce, Action = 13 },           // ]
                new ShiftReduce { Type = ShiftReduceType.Reduce, Action = 13 },           // ,
                ShiftReduceError,                                                         // :
                ShiftReduceError,                                                         // string
                ShiftReduceError,                                                         // number
                ShiftReduceError,                                                         // true 
                ShiftReduceError,                                                         // false
                ShiftReduceError,                                                         // null
                ShiftReduceError,                                                         // $
                ShiftReduceError,                                                         // S
                ShiftReduceError,                                                         // O
                ShiftReduceError,                                                         // A
                ShiftReduceError,                                                         // M
                ShiftReduceError,                                                         // P
                ShiftReduceError,                                                         // E
                ShiftReduceError                                                          // V
            }
            ,
            { // state 19
                ShiftReduceError,                                                         // {
                new ShiftReduce { Type = ShiftReduceType.Reduce, Action = 16 },           // }
                ShiftReduceError,                                                         // [
                new ShiftReduce { Type = ShiftReduceType.Reduce, Action = 16 },           // ]
                new ShiftReduce { Type = ShiftReduceType.Reduce, Action = 16 },           // ,
                ShiftReduceError,                                                         // :
                ShiftReduceError,                                                         // string
                ShiftReduceError,                                                         // number
                ShiftReduceError,                                                         // true 
                ShiftReduceError,                                                         // false
                ShiftReduceError,                                                         // null
                ShiftReduceError,                                                         // $
                ShiftReduceError,                                                         // S
                ShiftReduceError,                                                         // O
                ShiftReduceError,                                                         // A
                ShiftReduceError,                                                         // M
                ShiftReduceError,                                                         // P
                ShiftReduceError,                                                         // E
                ShiftReduceError                                                          // V
            }
            ,
            { // state 20
                ShiftReduceError,                                                         // {
                new ShiftReduce { Type = ShiftReduceType.Reduce, Action = 17 },           // }
                ShiftReduceError,                                                         // [
                new ShiftReduce { Type = ShiftReduceType.Reduce, Action = 17 },           // ]
                new ShiftReduce { Type = ShiftReduceType.Reduce, Action = 17 },           // ,
                ShiftReduceError,                                                         // :
                ShiftReduceError,                                                         // string
                ShiftReduceError,                                                         // number
                ShiftReduceError,                                                         // true 
                ShiftReduceError,                                                         // false
                ShiftReduceError,                                                         // null
                ShiftReduceError,                                                         // $
                ShiftReduceError,                                                         // S
                ShiftReduceError,                                                         // O
                ShiftReduceError,                                                         // A
                ShiftReduceError,                                                         // M
                ShiftReduceError,                                                         // P
                ShiftReduceError,                                                         // E
                ShiftReduceError                                                          // V
            }
            ,
            { // state 21
                ShiftReduceError,                                                         // {
                new ShiftReduce { Type = ShiftReduceType.Reduce, Action = 18 },           // }
                ShiftReduceError,                                                         // [
                new ShiftReduce { Type = ShiftReduceType.Reduce, Action = 18 },           // ]
                new ShiftReduce { Type = ShiftReduceType.Reduce, Action = 18 },           // ,
                ShiftReduceError,                                                         // :
                ShiftReduceError,                                                         // string
                ShiftReduceError,                                                         // number
                ShiftReduceError,                                                         // true 
                ShiftReduceError,                                                         // false
                ShiftReduceError,                                                         // null
                ShiftReduceError,                                                         // $
                ShiftReduceError,                                                         // S
                ShiftReduceError,                                                         // O
                ShiftReduceError,                                                         // A
                ShiftReduceError,                                                         // M
                ShiftReduceError,                                                         // P
                ShiftReduceError,                                                         // E
                ShiftReduceError                                                          // V
            }
            ,
            { // state 22
                ShiftReduceError,                                                         // {
                new ShiftReduce { Type = ShiftReduceType.Reduce, Action = 8 },            // }
                ShiftReduceError,                                                         // [
                new ShiftReduce { Type = ShiftReduceType.Reduce, Action = 8 },            // ]
                new ShiftReduce { Type = ShiftReduceType.Reduce, Action = 8 },            // ,
                ShiftReduceError,                                                         // :
                ShiftReduceError,                                                         // string
                ShiftReduceError,                                                         // number
                ShiftReduceError,                                                         // true 
                ShiftReduceError,                                                         // false
                ShiftReduceError,                                                         // null
                new ShiftReduce { Type = ShiftReduceType.Reduce, Action = 8 },            // $
                ShiftReduceError,                                                         // S
                ShiftReduceError,                                                         // O
                ShiftReduceError,                                                         // A
                ShiftReduceError,                                                         // M
                ShiftReduceError,                                                         // P
                ShiftReduceError,                                                         // E
                ShiftReduceError                                                          // V
            }
            ,
            { // state 23
                new ShiftReduce { Type = ShiftReduceType.Shift, Action = 4 },             // {
                ShiftReduceError,                                                         // }
                new ShiftReduce { Type = ShiftReduceType.Shift, Action = 5 },             // [
                ShiftReduceError,                                                         // ]
                ShiftReduceError,                                                         // ,
                ShiftReduceError,                                                         // :
                new ShiftReduce { Type = ShiftReduceType.Shift, Action = 17 },            // string
                new ShiftReduce { Type = ShiftReduceType.Shift, Action = 18 },            // number
                new ShiftReduce { Type = ShiftReduceType.Shift, Action = 19 },            // true 
                new ShiftReduce { Type = ShiftReduceType.Shift, Action = 20 },            // false
                new ShiftReduce { Type = ShiftReduceType.Shift, Action = 21 },            // null
                ShiftReduceError,                                                         // $
                ShiftReduceError,                                                         // S
                new ShiftReduce { Type = ShiftReduceType.Goto, Action = 14 },             // O
                new ShiftReduce { Type = ShiftReduceType.Goto, Action = 15 },             // A
                ShiftReduceError,                                                         // M
                ShiftReduceError,                                                         // P
                ShiftReduceError,                                                         // E
                new ShiftReduce { Type = ShiftReduceType.Goto, Action = 24 }              // V
            }
            ,
            { // state 24
                ShiftReduceError,                                                         // {
                new ShiftReduce { Type = ShiftReduceType.Reduce, Action = 7 },            // }
                ShiftReduceError,                                                         // [
                ShiftReduceError,                                                         // ]
                new ShiftReduce { Type = ShiftReduceType.Reduce, Action = 7 },            // ,
                ShiftReduceError,                                                         // :
                ShiftReduceError,                                                         // string
                ShiftReduceError,                                                         // number
                ShiftReduceError,                                                         // true 
                ShiftReduceError,                                                         // false
                ShiftReduceError,                                                         // null
                ShiftReduceError,                                                         // $
                ShiftReduceError,                                                         // S
                ShiftReduceError,                                                         // O
                ShiftReduceError,                                                         // A
                ShiftReduceError,                                                         // M
                ShiftReduceError,                                                         // P
                ShiftReduceError,                                                         // E
                ShiftReduceError                                                          // V
            }
            ,
            { // state 25
                new ShiftReduce { Type = ShiftReduceType.Shift, Action = 4 },             // {
                ShiftReduceError,                                                         // }
                new ShiftReduce { Type = ShiftReduceType.Shift, Action = 5 },             // [
                ShiftReduceError,                                                         // ]
                ShiftReduceError,                                                         // ,
                ShiftReduceError,                                                         // :
                new ShiftReduce { Type = ShiftReduceType.Shift, Action = 17 },            // string
                new ShiftReduce { Type = ShiftReduceType.Shift, Action = 18 },            // number
                new ShiftReduce { Type = ShiftReduceType.Shift, Action = 19 },            // true 
                new ShiftReduce { Type = ShiftReduceType.Shift, Action = 20 },            // false
                new ShiftReduce { Type = ShiftReduceType.Shift, Action = 21 },            // null
                ShiftReduceError,                                                         // $
                ShiftReduceError,                                                         // S
                new ShiftReduce { Type = ShiftReduceType.Goto, Action = 14 },             // O
                new ShiftReduce { Type = ShiftReduceType.Goto, Action = 15 },             // A
                ShiftReduceError,                                                         // M
                ShiftReduceError,                                                         // P
                new ShiftReduce { Type = ShiftReduceType.Goto, Action = 26 },             // E
                new ShiftReduce { Type = ShiftReduceType.Goto, Action = 16 }              // V
            }
            ,
            { // state 26
                ShiftReduceError,                                                         // {
                ShiftReduceError,                                                         // }
                ShiftReduceError,                                                         // [
                new ShiftReduce { Type = ShiftReduceType.Reduce, Action = 10 },           // ]
                ShiftReduceError,                                                         // ,
                ShiftReduceError,                                                         // :
                ShiftReduceError,                                                         // string
                ShiftReduceError,                                                         // number
                ShiftReduceError,                                                         // true 
                ShiftReduceError,                                                         // false
                ShiftReduceError,                                                         // null
                ShiftReduceError,                                                         // $
                ShiftReduceError,                                                         // S
                ShiftReduceError,                                                         // O
                ShiftReduceError,                                                         // A
                ShiftReduceError,                                                         // M
                ShiftReduceError,                                                         // P
                ShiftReduceError,                                                         // E
                ShiftReduceError                                                          // V
            }
            ,
            { // state 27
                ShiftReduceError,                                                         // {
                new ShiftReduce { Type = ShiftReduceType.Reduce, Action = 4 },            // }
                ShiftReduceError,                                                         // [
                new ShiftReduce { Type = ShiftReduceType.Reduce, Action = 4 },            // ]
                new ShiftReduce { Type = ShiftReduceType.Reduce, Action = 4 },            // ,
                ShiftReduceError,                                                         // :
                ShiftReduceError,                                                         // string
                ShiftReduceError,                                                         // number
                ShiftReduceError,                                                         // true 
                ShiftReduceError,                                                         // false
                ShiftReduceError,                                                         // null
                new ShiftReduce { Type = ShiftReduceType.Reduce, Action = 4 },            // $
                ShiftReduceError,                                                         // S
                ShiftReduceError,                                                         // O
                ShiftReduceError,                                                         // A
                ShiftReduceError,                                                         // M
                ShiftReduceError,                                                         // P
                ShiftReduceError,                                                         // E
                ShiftReduceError                                                          // V
            }
        };

        #endregion

        #endregion

        /// <summary>
        /// Parses a JSON string to create an IJsonValue representation of the JSON
        /// </summary>
        /// <param name="json">JSON string to parse</param>
        /// <returns>IJsonValue representation of the JSON</returns>
        public static IJsonValue Parse(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                if (Log.IsErrorEnabled)
                    Log.Error("Attempt to parse null or empty JSON string");

                throw new JsonException("Attempt to parse null or empty JSON string");
            }

            using (var lex = new JsonLexicalAnalyzer(json))
            {
                return ShiftReduceParse(json, lex);
            }
        }

        private static IJsonValue ShiftReduceParse(string json, JsonLexicalAnalyzer lex)
        {
            var inputStack = new Stack<IJsonValue>();
            var parseStack = new Stack<ParseItem>();
            var shiftReduceStack = new Stack<int>();
            shiftReduceStack.Push(0);

            var currentLexeme = lex.Scan();
            var done = false;

            while (!done)
            {
                if (currentLexeme == JsonLexicalType.Error)
                    LogAndThrowJsonError(lex, json, false);

                var topOfStack = shiftReduceStack.Peek();
                var shiftReduceAction = ActionGotoTable[topOfStack, (int)currentLexeme];

                switch (shiftReduceAction.Type)
                {
                    case ShiftReduceType.Shift:
                        Shift(shiftReduceStack, lex, currentLexeme, shiftReduceAction, inputStack);
                        currentLexeme = lex.Scan();
                        break;

                    case ShiftReduceType.Reduce:
                        Reduce(shiftReduceStack, shiftReduceAction, inputStack, parseStack);
                        break;

                    case ShiftReduceType.Accept:
                        if (Log.IsDebugEnabled)
                            Log.Debug("JSON Parser Accept State");
                        done = true;
                        break;

                    default:
                        LogAndThrowJsonError(lex, json, true);
                        break;
                }
            }

            // ParseRoot should always be left on the parseStack at this point (otherwise an error would have already occured)
            return parseStack.Pop().ToJsonValue();
        }

        private static void LogAndThrowJsonError(JsonLexicalAnalyzer lex, string json, bool isParseError)
        {
            var parseAnalysisError = isParseError ? "parsing" : "analyzing";
            var errorMessage = string.Format("Error {3} JSON at line number {1} character {2}: {0}", json, lex.LineNumber, lex.Index, parseAnalysisError);

            if (Log.IsErrorEnabled)
                Log.Error(errorMessage);

            throw new JsonException(errorMessage);
        }

        private static void Shift(Stack<int> shiftReduceStack, JsonLexicalAnalyzer lex, JsonLexicalType currentLexeme, ShiftReduce shiftReduce, Stack<IJsonValue> inputStack)
        {
            shiftReduceStack.Push((int)currentLexeme);
            shiftReduceStack.Push(shiftReduce.Action);

            //if (Log.IsDebugEnabled)
            //    Log.Debug("Shift {0} s{1}", currentLexeme, shiftReduce.Action);

            switch (currentLexeme)
            {
                case JsonLexicalType.Number:
                    if (lex.CurrentDouble.HasValue)
                        inputStack.Push(new JsonNumber(lex.CurrentDouble.Value));
                    else if (lex.CurrentInt64.HasValue)
                        inputStack.Push(new JsonNumber(lex.CurrentInt64.Value));
                    break;

                case JsonLexicalType.String:
                    inputStack.Push(new JsonString(lex.CurrentString));
                    break;

                case JsonLexicalType.True:
                    inputStack.Push(new JsonBoolean(true));
                    break;

                case JsonLexicalType.False:
                    inputStack.Push(new JsonBoolean(false));
                    break;

                case JsonLexicalType.Null:
                    inputStack.Push(Singleton<JsonNull>.Instance);
                    break;

                default:
                    break;
            }
        }

        private static void Reduce(Stack<int> shiftReduceStack, ShiftReduce shiftReduce, Stack<IJsonValue> inputStack, Stack<ParseItem> parseStack)
        {
            var numPops = ReduceLengths[shiftReduce.Action] << 1;

            for (var i = 0; i < numPops; ++i)
                shiftReduceStack.Pop();

            var topOfStack = shiftReduceStack.Peek();
            var leftHandSide = ReduceLeftHandSides[shiftReduce.Action];

            shiftReduceStack.Push(shiftReduce.Action);
            shiftReduceStack.Push(ActionGotoTable[topOfStack, leftHandSide].Action);

            //if (Log.IsDebugEnabled)
            //    Log.Debug("Reduce r{0} by production: {1}", shiftReduce.Action, Productions[shiftReduce.Action]);

            // build the parse tree for the action at this reduction step
            ReduceAction(shiftReduce, parseStack, inputStack);
        }

        private static void ReduceAction(ShiftReduce shiftReduce, Stack<ParseItem> parseStack, Stack<IJsonValue> inputStack)
        {
            switch ((ProductionType)shiftReduce.Action)
            {
                case ProductionType.EmptyArray:
                    ReduceEmptyArray(parseStack);
                    break;

                case ProductionType.EmptyObject:
                    ReduceEmptyObject(parseStack);
                    break;

                case ProductionType.NonEmptyArray:
                    ReduceNonEmptyArray(parseStack);
                    break;

                case ProductionType.NonEmptyObject:
                    ReduceNonEmptyObject(parseStack);
                    break;

                case ProductionType.MemberMultiple:
                    ReduceMemberMultiple(parseStack);
                    break;

                case ProductionType.Object:
                    ReduceObject(parseStack);
                    break;

                case ProductionType.Array:
                    ReduceArray(parseStack);
                    break;

                case ProductionType.MemberSingle:
                    ReduceMemberSingle(parseStack);
                    break;

                case ProductionType.Pair:
                    ReducePair(inputStack, parseStack);
                    break;

                case ProductionType.ElementMultiple:
                    ReduceElementMultiple(parseStack);
                    break;

                case ProductionType.ElementSingle:
                    ReduceElementSingle(parseStack);
                    break;

                case ProductionType.ObjectValue:
                    ReduceObjectValue(parseStack);
                    break;

                case ProductionType.ArrayValue:
                    ReduceArrayValue(parseStack);
                    break;

                case ProductionType.StringValue:
                case ProductionType.NumberValue:
                case ProductionType.TrueValue:
                case ProductionType.FalseValue:
                case ProductionType.NullValue:
                    ReduceValue(inputStack, parseStack);
                    break;

                default:
                    break;
            }
        }

        private static void ReduceObjectValue(Stack<ParseItem> parseStack)
        {
            parseStack.Push(new ParseValue { ParseObject = (ParseObject)parseStack.Pop() });
        }

        private static void ReduceArrayValue(Stack<ParseItem> parseStack)
        {
            parseStack.Push(new ParseValue { ParseArray = (ParseArray)parseStack.Pop() });
        }

        private static void ReduceValue(Stack<IJsonValue> inputStack, Stack<ParseItem> parseStack)
        {
            parseStack.Push(new ParseValue { Value = inputStack.Pop() });
        }

        private static void ReducePair(Stack<IJsonValue> inputStack, Stack<ParseItem> parseStack)
        {
            parseStack.Push(new ParsePair { Key = (JsonString)inputStack.Pop(), Value = (ParseValue)parseStack.Pop() });
        }

        private static void ReduceElementSingle(Stack<ParseItem> parseStack)
        {
            parseStack.Push(new ParseElement { Value = (ParseValue)parseStack.Pop() });
        }

        private static void ReduceElementMultiple(Stack<ParseItem> parseStack)
        {
            parseStack.Push(new ParseElement { Element = (ParseElement)parseStack.Pop(), Value = (ParseValue)parseStack.Pop() });
        }

        private static void ReduceArray(Stack<ParseItem> parseStack)
        {
            parseStack.Push(new ParseRoot { ParseArray = (ParseArray)parseStack.Pop() });
        }

        private static void ReduceObject(Stack<ParseItem> parseStack)
        {
            parseStack.Push(new ParseRoot { ParseObject = (ParseObject)parseStack.Pop() });
        }

        private static void ReduceMemberSingle(Stack<ParseItem> parseStack)
        {
            parseStack.Push(new ParseMember { Pair = (ParsePair)parseStack.Pop() });
        }

        private static void ReduceMemberMultiple(Stack<ParseItem> parseStack)
        {
            parseStack.Push(new ParseMember { Member = (ParseMember)parseStack.Pop(), Pair = (ParsePair)parseStack.Pop() });
        }

        private static void ReduceNonEmptyObject(Stack<ParseItem> parseStack)
        {
            parseStack.Push(new ParseObject { Member = (ParseMember)parseStack.Pop() });
        }

        private static void ReduceNonEmptyArray(Stack<ParseItem> parseStack)
        {
            parseStack.Push(new ParseArray { Element = (ParseElement)parseStack.Pop() });
        }

        private static void ReduceEmptyObject(Stack<ParseItem> parseStack)
        {
            parseStack.Push(new ParseObject());
        }

        private static void ReduceEmptyArray(Stack<ParseItem> parseStack)
        {
            parseStack.Push(new ParseArray());
        }
    }
}
