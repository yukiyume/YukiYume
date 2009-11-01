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
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

#endregion

namespace YukiYume.Json
{
    public enum JsonLexicalType
    {
        Error = -1,
        LeftBrace = 0,
        RightBrace = 1,
        LeftBracket = 2,
        RightBracket = 3,
        Comma = 4,
        Colon = 5,
        String = 6,
        Number = 7,
        True = 8,
        False = 9,
        Null = 10,
        EndOfJson = 11
    }

    /// <summary>
    /// JSON scanner
    /// </summary>
    public sealed class JsonLexicalAnalyzer : IDisposable
    {
        private static readonly Regex ValidDouble = new Regex(@"-?(([1-9]\d*)|0)\.\d+([eE][+-]\d)?", RegexOptions.Compiled);
        private static readonly Regex ValidInt = new Regex(@"-?(([1-9]\d*)|0)([eE][+-]\d)?", RegexOptions.Compiled);
        private static readonly Regex ValidStringEscape = new Regex(@"\""|\\|\/|b|f|n|r|t", RegexOptions.Compiled);
        private static readonly Regex ValidNumberOrHex = new Regex(@"[0-9a-fA-F]", RegexOptions.Compiled);
        private static readonly char[] TrueChars = new[] {'r', 'u', 'e'};
        private static readonly char[] FalseChars = new[] { 'a', 'l', 's', 'e' };
        private static readonly char[] NullChars = new[] { 'u', 'l', 'l' };

        private bool _disposed;

        public TextReader TextReader { get; private set; }

        public double? CurrentDouble { get; set; }

        public long? CurrentInt64 { get; set; }

        public string CurrentString { get; set; }

        public int Index { get; private set; }

        public int LineNumber { get; private set; }

        /// <summary>
        /// constructs a new scanner for the JSON string
        /// </summary>
        /// <param name="json">JSON string to scan</param>
        /// <exception cref="ArgumentNullException"></exception>
        public JsonLexicalAnalyzer(string json)
        {
            if (json == null)
                throw new ArgumentNullException("json");

            TextReader = new StringReader(json);
            Index = 0;
            LineNumber = 1;
        }

        private void ReadNextChar(out int next, out char ch)
        {
            next = TextReader.Read();
            ch = (char)next;
            ++Index;
        }

        private void PeekNextChar(out int next, out char ch)
        {
            next = TextReader.Peek();
            ch = (char)next;
        }

        /// <summary>
        /// scans the input for the next lex type
        /// </summary>
        /// <returns>JsonLexicalType for the next lex type</returns>
        public JsonLexicalType Scan()
        {
            int next = 0;
            char ch = ' ';

            while (next != -1)
            {
                ReadNextChar(out next, out ch);

                if (next == -1)
                    return JsonLexicalType.EndOfJson;

                if (ch == '\n')
                {
                    LineNumber++;
                    Index = 0;
                }

                if (!char.IsWhiteSpace(ch))
                    break;
            }

            switch (ch)
            {
                case '\"':
                    return ScanString();
                case '[':
                    return JsonLexicalType.LeftBracket;
                case ']':
                    return JsonLexicalType.RightBracket;
                case '{':
                    return JsonLexicalType.LeftBrace;
                case '}':
                    return JsonLexicalType.RightBrace;
                case ':':
                    return JsonLexicalType.Colon;
                case ',':
                    return JsonLexicalType.Comma;
                case 't':
                    return ScanTrue();
                case 'f':
                    return ScanFalse();
                case 'n':
                    return ScanNull();
                default:
                    return (ch == '-' || char.IsNumber(ch)) ? ScanNumber(ch) : JsonLexicalType.Error;
            }
        }

        private bool ScanChars(char[] chars)
        {
            int next;
            char ch;

            for (var i = 0; i < chars.Length; ++i)
            {
                ReadNextChar(out next, out ch);

                if (ch != chars[i])
                    return false;
            }

            return true;
        }

        private JsonLexicalType ScanNull()
        {
            return ScanChars(NullChars) ? JsonLexicalType.Null : JsonLexicalType.Error;
        }

        private JsonLexicalType ScanNumber(char c)
        {
            var numberBuilder = new StringBuilder(c.ToString());

            while (true)
            {
                int next;
                char ch;

                PeekNextChar(out next, out ch);

                if (next == -1 || (!char.IsNumber(ch) && ch != '.' && ch != 'e' && ch != 'E' && ch != '+' && ch != '-'))
                    break;

                ReadNextChar(out next, out ch);
                numberBuilder.Append(ch.ToString());
            }

            var stringValue = numberBuilder.ToString();

            if (ValidDouble.IsMatch(stringValue))
            {
                CurrentDouble = double.Parse(stringValue, NumberStyles.AllowExponent | NumberStyles.Number);
                CurrentInt64 = null;
            }
            else if (ValidInt.IsMatch(stringValue))
            {
                CurrentInt64 = long.Parse(stringValue, NumberStyles.AllowExponent | NumberStyles.Number);
                CurrentDouble = null;
            }
            else
                return JsonLexicalType.Error;

            return JsonLexicalType.Number;
        }

        private JsonLexicalType ScanFalse()
        {
            return ScanChars(FalseChars) ? JsonLexicalType.False : JsonLexicalType.Error;
        }

        private JsonLexicalType ScanTrue()
        {
            return ScanChars(TrueChars) ? JsonLexicalType.True : JsonLexicalType.Error;
        }

        private JsonLexicalType ScanString()
        {
            var stringBuilder = new StringBuilder();

            while (true)
            {
                int next;
                char ch;

                ReadNextChar(out next, out ch);

                if (next == -1)
                    return JsonLexicalType.EndOfJson;

                switch (ch)
                {
                    case '\\':
                        var lexType = ScanEscapedCharacter(stringBuilder);
                        if (lexType != JsonLexicalType.String)
                            return lexType;
                        break;
                    case '\"':
                        CurrentString = stringBuilder.ToString();
                        return JsonLexicalType.String;
                    default:
                        stringBuilder.Append(ch.ToString());
                        break;
                }
            }
        }

        private JsonLexicalType ScanEscapedCharacter(StringBuilder stringBuilder)
        {
            int next;
            char ch;

            ReadNextChar(out next, out ch);

            if (next == -1)
                return JsonLexicalType.EndOfJson;

            if (ValidStringEscape.IsMatch(ch.ToString()))
                stringBuilder.AppendFormat("\\{0}", ch);
            else if (ch == 'u')
            {
                string unicodeCharacter;
                var lexType = ScanUnicodeCharacter(out unicodeCharacter);

                if (lexType == JsonLexicalType.String)
                    stringBuilder.AppendFormat(unicodeCharacter);
                else
                    return lexType;
            }
            else
                return JsonLexicalType.Error;

            return JsonLexicalType.String;
        }

        private JsonLexicalType ScanUnicodeNibble(out char ch)
        {
            int next;

            ReadNextChar(out next, out ch);

            if (next == -1)
                return JsonLexicalType.EndOfJson;

            if (!ValidNumberOrHex.IsMatch(ch.ToString()))
                return JsonLexicalType.Error;

            return JsonLexicalType.String;
        }

        /* TODO: enable support for extended characters not in the Basic Multilingual Plane (BMP)
         * See: 
         * http://www.ietf.org/rfc/rfc4627.txt?number=4627
         * http://en.wikipedia.org/wiki/Unicode
         * http://msdn.microsoft.com/en-us/library/ms776414.aspx
         */
        private JsonLexicalType ScanUnicodeCharacter(out string unicodeCharacter)
        {
            unicodeCharacter = null;
            var unicodeByte = string.Empty;
            var characterBytes = new byte[2];
            var characterBytesIndex = 0;

            for (var numNibbles = 0; numNibbles < 4; numNibbles++)
            {
                char ch;
                var lexType = ScanUnicodeNibble(out ch);

                if (lexType != JsonLexicalType.String)
                    return lexType;

                unicodeByte += ch;

                // when numNibbles is odd, parse the next byte
                if ((numNibbles & 0x01) == 0x01)
                {
                    characterBytes[characterBytesIndex] = byte.Parse(unicodeByte, NumberStyles.HexNumber);
                    characterBytesIndex++;
                    unicodeByte = string.Empty;
                }
            }

            unicodeCharacter = Encoding.BigEndianUnicode.GetString(characterBytes);
            return JsonLexicalType.String;
        }


        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    TextReader.Dispose();
                }

                _disposed = true;
            }
        }

        #endregion
    }
}
