using System.Text.RegularExpressions;

namespace TournamentScraper
{
    internal partial class RegexHelper
    {
        /// <summary>
        /// Regex for time: e.g., "kl. 18:30" or "10:00"
        /// Tries to find "kl. HH:MM" first, then just "HH:MM"
        /// </summary>
        /// <returns></returns>
        [GeneratedRegex(@"kl\.\s*(\d{1,2}:\d{2})", RegexOptions.IgnoreCase, "da-DK")]
        internal static partial Regex TimeRegex();

        /// <summary>
        /// Regex for place: e.g., "Sted: Kulturhuset"
        /// Captures text immediately after "Sted:" until a newline or end of string.
        /// </summary>
        /// <returns></returns>
        [GeneratedRegex(
            @"Sted:\s*(.+?)(?:\r?\n|$)",
            RegexOptions.IgnoreCase | RegexOptions.Multiline,
            "da-DK"
        )]
        internal static partial Regex PlaceRegex();

        /// <summary>
        /// Fallback for time without "kl." prefix, but be careful of false positives
        /// </summary>
        /// <returns></returns>
        [GeneratedRegex(@"\b(\d{1,2}:\d{2})\b")]
        internal static partial Regex GenericTimeRegex();

        /// <summary>
        /// Regex for date: e.g., "11. - 13. oktober 2024" or "27. august 2025"
        /// This pattern handles single dates and date ranges with Danish month names.
        /// </summary>
        /// <returns></returns>
        [GeneratedRegex(
            @"(\d{1,2}\.\s*(?:-\s*\d{1,2}\.)?\s*(?:januar|februar|marts|april|maj|juni|juli|august|september|oktober|november|december)\s*\d{4})",
            RegexOptions.IgnoreCase,
            "da-DK"
        )]
        internal static partial Regex DateRegex();

        /// <summary>
        /// If "Sted:" is not found, attempt to find a city or general location.
        /// This is a more generic fallback and might need tuning.
        /// </summary>
        /// <returns></returns>
        [GeneratedRegex(
            @"i\s+([A-ZÆØÅ][a-zæøå\s-]+(?:Kommune|By)?)\b",
            RegexOptions.IgnoreCase,
            "da-DK"
        )]
        internal static partial Regex CityRegex();
    }
}
