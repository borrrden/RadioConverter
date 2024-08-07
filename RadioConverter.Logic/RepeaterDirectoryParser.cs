using CsvHelper;

using System.Globalization;

namespace RadioConverter.Logic
{
    public sealed class RepeaterDirectoryParser
    {
        public IReadOnlyList<RepeaterDirectoryEntry> Read(Stream source, bool noDigital = false)
        {
            using var reader = new StreamReader(source, leaveOpen: true);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            if (noDigital)
            {
                return csv.GetRecords<RepeaterDirectoryEntry>()
                    .TakeWhile(x => !x.Notes.StartsWith("{{*"))
                    .Where(x => x.Output != "")
                    .ToList();
            }

            return csv.GetRecords<RepeaterDirectoryEntry>().Where(x => x.Output != "").ToList();
        }
    }
}
