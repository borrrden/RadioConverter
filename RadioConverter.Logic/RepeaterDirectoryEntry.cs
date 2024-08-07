using CsvHelper.Configuration.Attributes;

namespace RadioConverter.Logic
{
    public sealed class RepeaterDirectoryEntry
    {
        [Index(0)]
        public string Output { get; init; } = "";

        [Index(1)]
        public string Input { get; init; } = "";

        [Index(2)]
        public string Call { get; init; } = "";

        [Index(3)]
        [Name("mNemonic")]
        public string ShortName { get; init; } = "-";

        [Index(4)]
        public string Location { get; init; } = "";

        [Index(5)]
        [Name("Service Area")]
        public string ServiceArea { get; init; } = "";

        [Index(6)]
        public string Latitude { get; init; } = "?";

        [Index(7)]
        public string Longitude { get; init; } = "?";

        [Index(8)]
        [Name("S")]
        public string Status { get; init; } = "O";

        [Index(9)]
        [Name("ERP")]
        public string EffectivePower { get; init; } = "";

        [Index(10)]
        [Name("HASL")]
        public string Altitude { get; init; } = "";

        [Index(11)]
        [Name("T/O")]
        public string Timeout { get; init; } = "";

        [Index(12)]
        [Name("Sp")]
        public string Sponsor { get; init; } = "";

        [Index(13)]
        public string Tone { get; init; } = "";

        [Index(14)]
        public string Notes { get; init; } = "";
    }
}
