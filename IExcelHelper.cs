namespace TournamentScraper
{
    public interface IExcelHelper<TExcelObject, TId> : IDisposable
        where TExcelObject : class, IExcelObject<TId>, new()
        where TId : notnull
    {
        IReadOnlyDictionary<TId, TExcelObject> GetExistingRecords();
        void AppendToExcel(TExcelObject excelObject);
    }
}
