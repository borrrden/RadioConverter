using CsvHelper;

using System.Globalization;

namespace RadioConverter.Logic
{
    public sealed class CsvOutputWriter : ITransformOutputWriter
    {
        public async Task WriteToAsync(IReadOnlyList<string> keys, IReadOnlyList<IReadOnlyDictionary<string, string>> values, Stream output)
        {
            using var writer = new StreamWriter(output, leaveOpen: true);
            using var csvOut = new CsvWriter(writer, CultureInfo.InvariantCulture);

            foreach (var key in keys)
            {
                csvOut.WriteField(key);
            }

            await csvOut.NextRecordAsync();
            foreach (var val in values)
            {
                foreach (var key in keys)
                {
                    csvOut.WriteField(val[key]);
                }

                await csvOut.NextRecordAsync();
            }

            await csvOut.FlushAsync();

            if (output.CanSeek)
            {
                output.Seek(0, SeekOrigin.Begin);
            }
        }
    }
}
