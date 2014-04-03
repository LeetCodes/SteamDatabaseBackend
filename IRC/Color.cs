/*
 * Copyright (c) 2013, SteamDB. All rights reserved.
 * Use of this source code is governed by a BSD-style license that can be
 * found in the LICENSE file.
 */
using System.Text.RegularExpressions;
using Meebey.SmartIrc4net;

namespace SteamDatabaseBackend
{
    public static class Colors
    {
        // To keep our colors somewhat consistent, only used colors are uncommented
        public const char NORMAL = IrcConstants.IrcNormal;
        //public static readonly string WHITE = IrcConstants.IrcColor + "00"; // white is evil
        //public static readonly string BLACK = IrcConstants.IrcColor + "01"; // black is evi, too
        public static readonly string DARK_BLUE = IrcConstants.IrcColor + "02";
        //public static readonly string DARK_GREEN = IrcConstants.IrcColor + "03";
        public static readonly string RED = IrcConstants.IrcColor + "04";
        //public static readonly string BROWN = IrcConstants.IrcColor + "05";
        //public static readonly string PURPLE = IrcConstants.IrcColor + "06";
        public static readonly string OLIVE = IrcConstants.IrcColor + "07";
        //public static readonly string YELLOW = IrcConstants.IrcColor + "08";
        public static readonly string GREEN = IrcConstants.IrcColor + "09";
        //public static readonly string TEAL = IrcConstants.IrcColor + "10";
        //public static readonly string CYAN = IrcConstants.IrcColor + "11";
        //public static readonly string BLUE = IrcConstants.IrcColor + "12";
        //public static readonly string MAGENTA = IrcConstants.IrcColor + "13";
        public static readonly string DARK_GRAY = IrcConstants.IrcColor + "14";
        public static readonly string LIGHT_GRAY = IrcConstants.IrcColor + "15";

        public static string StripColors(string input)
        {
            return Regex.Replace(input, @"\x15|\x03(\d\d)?", string.Empty);
        }
    }
}
