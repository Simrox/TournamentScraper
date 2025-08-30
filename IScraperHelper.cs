namespace TournamentScraper
{
    public interface IScraperHelper : IDisposable
    {
        IEnumerable<TournamentDetails> ScrapeTournaments(
            string websiteUrl,
            string baseUrl,
            IReadOnlySet<string> existingLinks
        );
    }
}
