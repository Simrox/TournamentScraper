namespace TournamentScraper
{
    public class TournamentDetails : ExcelObjectBase<Uri>
    {
        public override Uri Id => Uri;

        [Excel("Tournament URL", 1)]
        public Uri Uri { get; private set; }

        [Excel("Name", 2)]
        public string? Name { get; private set; }

        [Excel("Start Date", 3)]
        public DateOnly StartDate { get; private set; }

        [Excel("End Date", 4)]
        public DateOnly EndDate { get; private set; }

        [Excel("Number of Participants", 5)]
        public int NoParticipants { get; private set; }

        [Excel("Rated", 6)]
        public bool IsRated { get; private set; }

        [Excel("FIDE Rated", 7)]
        public bool IsFideRated { get; private set; }

        [Excel("Wheelchair Accessible", 8)]
        public bool WheelchairAccessible { get; private set; }

        [Excel("Description", 9)]
        public string? Description { get; private set; }

        [Excel("Date and Time Series", 10)]
        public string? DateAndTimeSeries { get; private set; }

        public bool IsEmpty { get; private set; }

        public TournamentDetails()
        {
            Uri = new Uri("about:blank");
            IsEmpty = true;
        }

        public TournamentDetails(
            Uri uri,
            string? name,
            DateOnly startDate,
            DateOnly endDate,
            int noParticipants,
            bool isRated,
            bool isFideRated,
            bool wheelchairAccessible,
            string? description,
            string? dateAndTimeSeries
        )
        {
            Uri = uri;
            Name = name;
            StartDate = startDate;
            EndDate = endDate;
            NoParticipants = noParticipants;
            IsRated = isRated;
            IsFideRated = isFideRated;
            WheelchairAccessible = wheelchairAccessible;
            Description = description;
            DateAndTimeSeries = dateAndTimeSeries;
            IsEmpty = false;
        }
    }
}
