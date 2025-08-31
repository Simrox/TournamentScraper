using OfficeOpenXml;

namespace TournamentScraper
{
    internal class ExcelHelper<TExcelObject, TId> : IExcelHelper<TExcelObject, TId>
        where TExcelObject : class, IExcelObject<TId>, new()
        where TId : notnull
    {
        private readonly FileInfo _excelFile;
        private readonly ExcelPackage _excelPackage;

        private const string WorkSheetName = "Tournaments";

        public ExcelHelper(string excelFilePath)
        {
            ExcelPackage.License.SetNonCommercialPersonal("Simrox");
            _excelFile = new(excelFilePath);
            _excelPackage = new ExcelPackage(_excelFile);
        }

        public IReadOnlyDictionary<TId, TExcelObject> GetExistingRecords()
        {
            var worksheet = InitializeExcel(WorkSheetName);
            return GetExistingRecords(worksheet);
        }

        /// <summary>
        /// Save the package even if no new links are found, to ensure any initial setup is saved.
        /// </summary>
        public void Dispose()
        {
            _excelPackage.Save();
            _excelPackage.Dispose();

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Appends the extracted excel record as a new row in the Excel worksheet.
        /// </summary>
        /// <param name="details">The TournamentDetails object to append.</param>
        public void AppendToExcel(TExcelObject excelObject)
        {
            var worksheet = _excelPackage.Workbook.Worksheets.First();

            int newRow = worksheet.Dimension?.Rows + 1 ?? 1;
            if (newRow == 1)
            {
                AddExcelHeaders(worksheet);
                newRow++;
            }

            var properties = excelObject.GetExcelProperties();
            int colIndex = 1;
            foreach (var prop in properties)
            {
                worksheet.Cells[newRow, colIndex].Value = prop.Value.Value;
                colIndex++;
            }
        }

        /// <summary>
        /// Initializes the Excel file, creates it if it doesn't exist,
        /// and reads any existing tournament URLs into the _existingLinks HashSet.
        /// </summary>
        private ExcelWorksheet InitializeExcel(string worksheet)
        {
            if (File.Exists(_excelFile.FullName))
            {
                try
                {
                    return _excelPackage.Workbook.Worksheets.FirstOrDefault()
                        ?? _excelPackage.Workbook.Worksheets.Add(worksheet);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error opening existing Excel file: {ex.Message}");
                    Console.WriteLine("Attempting to create a new Excel file.");
                    return CreateExcel(_excelPackage);
                }
            }
            else
            {
                return CreateExcel(_excelPackage);
            }
        }

        /// <summary>
        /// Read all records from the Excel file into a list of TExcelObject.
        /// </summary>
        /// <param name="worksheet"></param>
        private static IReadOnlyDictionary<TId, TExcelObject> GetExistingRecords(
            ExcelWorksheet worksheet
        )
        {
            var existingRecords = new Dictionary<TId, TExcelObject>();
            var excelObject = new TExcelObject();
            if (worksheet.Dimension != null && worksheet.Dimension.Rows > 1)
            {
                for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                {
                    var rowKeyValuePairs = GetRowKeyValuePairs(worksheet, row);
                    if (
                        rowKeyValuePairs.Count == 0
                        || rowKeyValuePairs.Values.All(i => string.IsNullOrEmpty(i.Value))
                    )
                    {
                        continue;
                    }
                    excelObject.SetProperties(rowKeyValuePairs);
                    existingRecords.TryAdd(excelObject.Id, excelObject);
                }
            }
            return existingRecords;
        }

        /// <summary>
        /// Create a new Excel package and worksheet if the file doesn't exist
        /// </summary>
        /// <param name="excelFile"></param>
        private static ExcelWorksheet CreateExcel(ExcelPackage excelPackage)
        {
            var worksheet = excelPackage.Workbook.Worksheets.Add(WorkSheetName);
            AddExcelHeaders(worksheet);
            return worksheet;
        }

        /// <summary>
        /// Adds header row to the Excel worksheet.
        /// </summary>
        private static void AddExcelHeaders(ExcelWorksheet excelWorksheet)
        {
            var excelObject = new TExcelObject();
            var properties = excelObject
                .GetExcelProperties()
                .OrderBy(p => p.Value.Order)
                .Select(p => p.Value.Header)
                .ToList();
            var index = 1;
            foreach (var prop in properties)
            {
                excelWorksheet.Cells[1, index].Value = prop;
                excelWorksheet.Cells[1, index].Style.Font.Bold = true;
                index++;
            }
        }

        private static IReadOnlyDictionary<string, ExcelProperty> GetRowKeyValuePairs(
            ExcelWorksheet worksheet,
            int rowIndex
        )
        {
            var result = new Dictionary<string, ExcelProperty>();
            int colCount = worksheet.Dimension.End.Column;
            var targetProperties = new TExcelObject().GetExcelProperties().Values;

            for (int col = 1; col <= colCount; col++)
            {
                var header = worksheet.Cells[1, col].Text;
                var value = worksheet.Cells[rowIndex, col].Text;

                if (string.IsNullOrEmpty(value))
                    continue;

                var targetProperty = targetProperties.FirstOrDefault(p =>
                    p.Header.Equals(header, StringComparison.OrdinalIgnoreCase)
                );

                targetProperty ??= targetProperties.FirstOrDefault(p =>
                    p.PropertyName.Equals(header, StringComparison.OrdinalIgnoreCase)
                );

                targetProperty ??= targetProperties.FirstOrDefault(p => p.Order == col);

                if (targetProperty is null)
                    continue;

                result.Add(
                    targetProperty.PropertyName,
                    new ExcelProperty
                    {
                        Header = header,
                        Order = col,
                        PropertyName = targetProperty.PropertyName,
                        PropertyType = targetProperty.PropertyType,
                        Value = value
                    }
                );
            }

            return result;
        }
    }
}
