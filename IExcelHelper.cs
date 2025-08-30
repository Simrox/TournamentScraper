namespace TournamentScraper
{
    public interface IExcelHelper : IDisposable
    {
        HashSet<string> GetExistingLinks();
        void AppendToExcel(TournamentDetails tournamentDetails);
    }
}
