using System.Net;
using HtmlAgilityPack;

namespace TournamentScraper
{
    internal partial class ScraperHelper(ILogger logger) : IScraperHelper
    {
        private readonly ILogger _logger = logger;

        /// <summary>
        /// Main method to start the scraping process.
        /// Navigates to the main tournament list, finds links, and processes new tournaments.
        /// </summary>
        public IEnumerable<TournamentDetails> ScrapeTournaments(
            string websiteUrl,
            string baseUrl,
            IReadOnlySet<string> existingLinks
        )
        {
            _logger.Log($"🚀 Starting to scrape tournaments from: {websiteUrl}");

            var web = new HtmlWeb();
            HtmlDocument doc;

            try
            {
                doc = web.Load(websiteUrl);
                var x = doc.DocumentNode.InnerHtml;
            }
            catch (Exception ex)
            {
                _logger.Log($"❌ Error loading main website {websiteUrl}: {ex.Message}");
                yield break;
            }

            // XPath to select all <a> tags within a <tbody> of any <table>, where href starts with /TournamentActive/Details?tourId=
            // This targets the specific tournament links in the main table.
            var tournamentLinkNodes = doc.DocumentNode.SelectNodes(
                "//table//tbody//a[starts-with(@href, '/TournamentActive/Details?tourId=')]"
            );

            if (tournamentLinkNodes == null || tournamentLinkNodes.Count == 0)
            {
                _logger.Log(
                    "⚠️ No tournament links found on the main page. Check XPath selector or website structure."
                );
                yield break;
            }

            var alreadyCheckedLinks = new HashSet<string>();
            foreach (var linkNode in tournamentLinkNodes)
            {
                string relativeHref = linkNode.GetAttributeValue("href", string.Empty);
                string fullUrl = baseUrl + relativeHref;
                if (string.IsNullOrEmpty(relativeHref) || alreadyCheckedLinks.Contains(fullUrl))
                    continue;

                alreadyCheckedLinks.Add(fullUrl);

                string tournamentName = linkNode.InnerText.Trim(); // The link text usually contains the tournament name

                if (existingLinks.Contains(fullUrl))
                {
                    _logger.Log($"⏩ Skipping existing tournament: {tournamentName} ({fullUrl})");
                    continue;
                }

                _logger.Log($"✨ Processing new tournament: {tournamentName} ({fullUrl})");

                var tournamentDetails = ProcessTournamentPage(fullUrl, tournamentName);
                if (tournamentDetails is null)
                    continue;

                yield return tournamentDetails;
            }
        }

        /// <summary>
        /// Navigates to an individual tournament details page and extracts date, time, and place.
        /// </summary>
        /// <param name="tournamentUrl">The URL of the tournament details page.</param>
        /// <param name="tournamentName">The name of the tournament (from the main page link).</param>
        /// <returns>A TournamentDetails object with extracted data, or null if an error occurs.</returns>
        private TournamentDetails? ProcessTournamentPage(
            string tournamentUrl,
            string tournamentName
        )
        {
            var web = new HtmlWeb();
            HtmlDocument doc;

            try
            {
                doc = web.Load(tournamentUrl);
            }
            catch (Exception ex)
            {
                _logger.Log($"❌ Error loading tournament page {tournamentUrl}: {ex.Message}");
                return null;
            }

            // In doc2.html, the main content is inside the div with id 'tour-inv'.
            // We'll check for that first, and fall back to the original 'mainContent' for compatibility.
            var mainContentNode =
                doc.DocumentNode.SelectSingleNode("//div[@id='tour-inv']")
                ?? doc.DocumentNode.SelectSingleNode("//div[@id='mainContent']");

            if (mainContentNode == null)
            {
                _logger.Log($"⚠️ Could not find main content node on page {tournamentUrl}");
                return null;
            }

            // Initialize extracted values
            string date = "N/A";
            string time = "N/A";
            string place = "N/A";

            // Prioritize extracting structured data with XPath before falling back to Regex
            var dateNode = mainContentNode.SelectSingleNode(
                ".//th[text()='Periode']/following-sibling::td[1]"
            );
            if (dateNode != null)
            {
                date = dateNode.InnerText.Trim();
            }

            // For other details, we'll parse the full text content.
            string contentText = mainContentNode.InnerText;

            // Fallback for date if not found in the structured table
            if (
                date == "N/A"
                && RegexHelper.DateRegex().Match(contentText) is { Success: true } dateMatch
            )
            {
                date = dateMatch.Groups[1].Value.Trim();
            }

            // Time extraction
            if (RegexHelper.TimeRegex().Match(contentText) is { Success: true } timeMatch)
            {
                time = timeMatch.Groups[1].Value.Trim();
            }
            else if (
                RegexHelper.GenericTimeRegex().Match(contentText) is
                { Success: true } genericTimeMatch
            )
            {
                time = genericTimeMatch.Groups[1].Value.Trim();
            }

            // Place extraction
            if (RegexHelper.PlaceRegex().Match(contentText) is { Success: true } placeMatch)
            {
                place = placeMatch.Groups[1].Value.Trim();
            }
            else if (RegexHelper.CityRegex().Match(contentText) is { Success: true } cityMatch)
            {
                place = cityMatch.Groups[1].Value.Trim();
            }

            return new TournamentDetails(
                new Uri(tournamentUrl),
                WebUtility.HtmlDecode(tournamentName),
                DateOnly.FromDateTime(DateTime.Now),
                DateOnly.FromDateTime(DateTime.Now),
                5,
                true,
                true,
                false,
                WebUtility.HtmlDecode($"{time}, {place}"),
                WebUtility.HtmlDecode(time)
            );
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
