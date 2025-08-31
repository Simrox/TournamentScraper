namespace TournamentScraper
{
    public record ExcelProperty
    {
        public required string Header { get; set; }
        public required int Order { get; set; }
        public required string PropertyName { get; set; }
        public required Type PropertyType { get; init; }
        public string? Value { get; set; }
    }
}
