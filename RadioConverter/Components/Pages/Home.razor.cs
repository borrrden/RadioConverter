using BlazorBootstrap;

using CsvHelper;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

using RadioConverter.Logic;

using System.Globalization;

namespace RadioConverter.Components.Pages
{
    public partial class Home
    {
        private IBrowserFile? _inputFile;
        private string _outputFormat = "chirp";
        private Button _submitButton = default!;
        private Guid _inputFileKey = Guid.NewGuid();

        [Inject]
        private IJSRuntime JSRuntime { get; set; } = default!;

        private bool SubmitDisabled => _inputFile == null;

        private static readonly IReadOnlyList<string> OutputFormats = new List<string>
        {
            "Chirp", "IC-9700"
        };

        private void OnInputChosen(InputFileChangeEventArgs e)
        {
            _inputFile = e.File;
        }

        private Task<Stream> Convert(FileStream input)
        {            
            input.Seek(0, SeekOrigin.Begin);
            return Task.Factory.StartNew<Stream>(() =>
            {
                using var yaml = File.OpenRead(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "outputs", $"output_format_{_outputFormat}.yaml"));
                var transformParser = new TransformParser(yaml);
                var transforms = transformParser.CreateTransforms();

                var parser = new RepeaterDirectoryParser();
                var results = parser.Read(input, true);

                var retVal = new List<Dictionary<string, string>>();

                foreach (var result in results)
                {
                    var nextResult = new Dictionary<string, string>();
                    foreach (var transform in transforms)
                    {
                        nextResult[transform.Key] = transform.Value.Execute(result);
                    }

                    retVal.Add(nextResult);
                }

                var fout = new MemoryStream();
                using var writer = new StreamWriter(fout, leaveOpen: true);
                using var csvOut = new CsvWriter(writer, CultureInfo.InvariantCulture);

                var keys = transformParser.Keys();
                foreach (var key in keys)
                {
                    csvOut.WriteField(key);
                }

                csvOut.NextRecord();
                foreach (var output in retVal)
                {
                    foreach (var key in keys)
                    {
                        csvOut.WriteField(output[key]);
                    }

                    csvOut.NextRecord();
                }

                csvOut.Flush();
                fout.Seek(0, SeekOrigin.Begin);
                return fout;
            });
            
        }


        private async Task OnSubmit(EventArgs e)
        {
            _inputFileKey = Guid.NewGuid();
            _submitButton.ShowLoading("Converting..");
            var stream = _inputFile!.OpenReadStream();
            var tmpPath = Path.GetTempFileName();
            var fs = File.Create(tmpPath);
            _inputFile = null;
            await stream.CopyToAsync(fs);

            using var outStream = await Convert(fs);
            var fileName = "output.csv";

            using var streamRef = new DotNetStreamReference(stream: outStream);
            await JSRuntime.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
            _submitButton.HideLoading();
        }
    }
}
