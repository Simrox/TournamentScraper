using TournamentScraper;

internal class Program
{
    private const string WebsiteUrl = "https://turnering.skak.dk/TournamentActive/AllInvitations";
    private const string BaseUrl = "https://turnering.skak.dk";
    private const string ExcelFilePath = "skak_tournaments.xlsx";

    /// <summary>
    /// The entry point of the application.
    /// </summary>
    public static void Main(string[] args)
    {
        var logger = new ConsoleLogger();
        var excelHelper = new ExcelHelper(ExcelFilePath);
        var scraper = new ScraperHelper(logger);

        var existingLinks = excelHelper.GetExistingLinks();
        var newTournamentsCount = 0;
        foreach (var tournament in scraper.ScrapeTournaments(WebsiteUrl, BaseUrl, existingLinks))
        {
            excelHelper.AppendToExcel(tournament);
            logger.Log(
                $"✅ Added to Excel: {tournament.Name} ({tournament.Date}, {tournament.Place})"
            );
            newTournamentsCount++;
        }

        logger.Log($"✅ Finished scraping. Added {newTournamentsCount} new tournaments");
        try
        {
            excelHelper.Dispose();
            scraper.Dispose();
            logger.Log($"📁 Excel file saved successfully.");
        }
        catch (Exception ex)
        {
            //log
            logger.Log($"❌ Error saving Excel file: {ex.Message}");
        }

        Console.WriteLine("\nPress any key to exit.");
        Console.ReadKey();
    }
}
