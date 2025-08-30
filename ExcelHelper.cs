using OfficeOpenXml;

namespace TournamentScraper
{
    internal class ExcelHelper : IExcelHelper
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

        public HashSet<string> GetExistingLinks()
        {
            var worksheet = InitializeExcel(WorkSheetName);
            return GetExistingLinks(worksheet);
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
        /// Appends the extracted tournament details as a new row in the Excel worksheet.
        /// </summary>
        /// <param name="details">The TournamentDetails object to append.</param>
        public void AppendToExcel(TournamentDetails details)
        {
            var worksheet = _excelPackage.Workbook.Worksheets.First();

            int newRow = worksheet.Dimension?.Rows + 1 ?? 1;
            if (newRow == 1)
            {
                AddExcelHeaders(worksheet);
                newRow++;
            }

            worksheet.Cells[newRow, 1].Value = details.Url;
            worksheet.Cells[newRow, 2].Value = details.Name;
            worksheet.Cells[newRow, 3].Value = details.Date;
            worksheet.Cells[newRow, 4].Value = details.Time;
            worksheet.Cells[newRow, 5].Value = details.Place;
        }

        /// <summary>
        /// Initializes the Excel file, creates it if it doesn't exist,
        /// and reads any existing tournament URLs into the _existingLinks HashSet.
        /// </summary>
        private ExcelWorksheet InitializeExcel(string worksheet)
        {
            if (_excelFile.Exists)
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
        /// Read existing links from the Excel file to populate _existingLinks
        /// This prevents re-processing tournaments already recorded.
        /// </summary>
        /// <param name="worksheet"></param>
        private static HashSet<string> GetExistingLinks(ExcelWorksheet worksheet)
        {
            var existingLinks = new HashSet<string>();
            if (worksheet.Dimension != null && worksheet.Dimension.Rows > 1)
            {
                for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                {
                    string link = worksheet.Cells[row, 1].Text;
                    if (!string.IsNullOrEmpty(link))
                    {
                        existingLinks.Add(link);
                    }
                }
            }
            return existingLinks;
        }

        /// <summary>
        /// Create a new Excel package and worksheet if the file doesn't exist
        /// </summary>
        /// <param name="excelFile"></param>
        private ExcelWorksheet CreateExcel(ExcelPackage excelPackage)
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
            excelWorksheet.Cells[1, 1].Value = "URL";
            excelWorksheet.Cells[1, 2].Value = "Tournament Name";
            excelWorksheet.Cells[1, 3].Value = "Date";
            excelWorksheet.Cells[1, 4].Value = "Time";
            excelWorksheet.Cells[1, 5].Value = "Place";
            excelWorksheet.Cells[1, 1, 1, 5].Style.Font.Bold = true;
        }
    }
}
