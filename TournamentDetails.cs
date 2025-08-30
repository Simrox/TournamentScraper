namespace TournamentScraper
{
    /// <summary>
    /// A simple class to hold the extracted tournament details.
    /// </summary>
    /// <param name="Url"></param>
    /// <param name="Name"></param>
    /// <param name="Date"></param>
    /// <param name="Time"></param>
    /// <param name="Place"></param>
    public record TournamentDetails(
        string Url,
        string Name,
        string Date,
        string Time,
        string Place
    );
}
