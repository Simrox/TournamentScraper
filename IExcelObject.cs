namespace TournamentScraper
{
    public interface IExcelObject<T>
        where T : notnull
    {
        public T Id { get; }
        void SetProperties(IReadOnlyDictionary<string, ExcelProperty> properties);
        IReadOnlyDictionary<string, ExcelProperty> GetExcelProperties();
    }
}
