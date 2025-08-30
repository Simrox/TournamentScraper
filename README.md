# Copilot Instructions for TournamentScraper

## Project Overview
- This is a C#/.NET console application for scraping chess tournament data from web pages and exporting results (e.g., to Excel).
- Main scraping logic is in `TournamentScraper.cs` (class: `ScraperHelper`).
- Data model for tournaments is defined in `TournamentDetails.cs`.
- Logging is abstracted via `ILogger` and implemented in `ConsoleLogger.cs`.
- Excel export functionality is handled by `ExcelHelper.cs` and its interface `IExcelHelper.cs`.
- Regex-based parsing helpers are in `RegexHelper.cs`.

## Build & Run
- Build with: `dotnet build` from the project root (where `TournamentScraper.csproj` is located).
- The main entry point is `Program.cs`.
- Output binaries are in `bin/Debug/net8.0/`.

## Key Patterns & Conventions
- Scraping uses HtmlAgilityPack (see `TournamentScraper.cs`), with XPath selectors for robust HTML parsing.
- Regex extraction is centralized in `RegexHelper.cs` for maintainability.
- Logging is always done via the injected `ILogger` instance.
- Tournament details are only processed if not present in the `existingLinks` set (deduplication).
- Error handling is verbose and user-friendly, with emoji-prefixed log messages for clarity.
- All scraping and parsing logic is encapsulated in helper classes for testability and separation of concerns.

## External Dependencies
- HtmlAgilityPack (HTML parsing)
- EPPlus (Excel export)
- Both are referenced as DLLs in `bin/Debug/net8.0/` and should be present for runtime.

## Integration Points
- Scraper interacts with web pages (see XPath in `TournamentScraper.cs`).
- Excel export interacts with `skak_tournaments.xlsx`.

## Example Workflow
1. Build: `dotnet build`
2. Run: `dotnet run` (or execute the built binary)
3. Scraper fetches tournaments, parses details, logs progress, and exports results to Excel.

## File Reference
- `TournamentScraper.cs` — main scraping logic
- `TournamentDetails.cs` — tournament data model
- `ExcelHelper.cs` / `IExcelHelper.cs` — Excel export
- `RegexHelper.cs` — regex patterns for parsing
- `ConsoleLogger.cs` / `ILogger.cs` — logging
- `Program.cs` — entry point

---
If any section is unclear or missing important project-specific details, please provide feedback or specify which workflows or patterns need further documentation.
