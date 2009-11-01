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
using log4net;

#endregion

namespace YukiYume.Logging
{
    /// <summary>
    /// log4net extension methods
    /// </summary>
    public static class Log4NetExtensions
    {
        public static void Info(this ILog log, string format, params object[] args)
        {
            log.InfoFormat(format, args);
        }

        public static void Debug(this ILog log, string format, params object[] args)
        {
            log.DebugFormat(format, args);
        }

        public static void Fatal(this ILog log, string format, params object[] args)
        {
            log.FatalFormat(format, args);
        }

        public static void Error(this ILog log, string format, params object[] args)
        {
            log.ErrorFormat(format, args);
        }

        public static void Warn(this ILog log, string format, params object[] args)
        {
            log.WarnFormat(format, args);
        }
    }
}
