namespace TournamentScraper
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ExcelAttribute(string header, int order) : Attribute
    {
        public string Header { get; } = header;
        public int Order { get; } = order;
    }
}
