namespace TournamentScraper
{
    internal class ConsoleLogger : ILogger
    {
        public void Log(string text)
        {
            Console.WriteLine(text);
        }
    }
}
