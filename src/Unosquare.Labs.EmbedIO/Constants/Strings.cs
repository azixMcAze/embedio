﻿namespace Unosquare.Labs.EmbedIO.Constants
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Defines assembly-wide constants.
    /// </summary>
    internal static class Strings
    {
        internal const string WebSocketVersion = "13";

        /// <summary>
        ///  Default Browser time format.
        /// </summary>
        internal const string BrowserTimeFormat = "ddd, dd MMM yyyy HH:mm:ss 'GMT'";

        /// <summary>
        /// Default CORS rule.
        /// </summary>
        internal const string CorsWildcard = "*";

        /// <summary>
        /// The comma split character for String.Split method calls.
        /// </summary>
        internal static readonly char[] CommaSplitChar = { ',' };

        /// <summary>
        /// The cookie split chars for String.Split method calls.
        /// </summary>
        internal static readonly char[] CookieSplitChars = {';', ','};

        /// <summary>
        /// The format culture used for header outputs.
        /// </summary>
        internal static CultureInfo StandardCultureInfo { get; } = new CultureInfo("en-US");

        /// <summary>
        /// The standard string comparer.
        /// </summary>
        internal static StringComparer StandardStringComparer { get; } = StringComparer.InvariantCultureIgnoreCase;
    }
}
