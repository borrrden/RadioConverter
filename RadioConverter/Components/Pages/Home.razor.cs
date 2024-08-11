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
        private readonly List<ToastMessage> _messages = [];

        [Inject]
        private IJSRuntime JSRuntime { get; set; } = default!;

        private bool SubmitDisabled => _inputFile == null || _inputFile.Size == 0;

        private static readonly IReadOnlyList<string> OutputFormats = new List<string>
        {
            "Chirp", "IC-9700"
        };

        private void OnInputChosen(InputFileChangeEventArgs e)
        {
            var fileExtension = Path.GetExtension(e.File.Name);
            if (fileExtension != ".csv")
            {
                _inputFileKey = Guid.NewGuid();
                _messages.Add(new ToastMessage(ToastType.Danger, $"{fileExtension} is not a supported file type!"));
                return;
            }

            if (e.File.Size == 0)
            {
                _inputFileKey = Guid.NewGuid();
                _messages.Add(new ToastMessage(ToastType.Warning, $"Empty file selected"));
                return;
            }

            _inputFile = e.File;
        }

        private Task<Stream> Convert(FileStream input)
        {            
            input.Seek(0, SeekOrigin.Begin);
            return Task.Run(async () =>
            {
                using var yaml = File.OpenRead(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "outputs", $"output_format_{_outputFormat}.yaml"));
                var transformParser = new TransformParser(yaml);
                var transforms = transformParser.CreateTransforms();

                var parser = new RepeaterDirectoryParser();
                IReadOnlyList<RepeaterDirectoryEntry> results;
                try
                {
                    results = parser.Read(input, true);
                }
                catch (HeaderValidationException)
                {
                    _messages.Add(new ToastMessage(ToastType.Danger, "Input file didn't have proper headers"));
                    return Stream.Null;
                }
                catch (Exception ex)
                {
                    _messages.Add(new ToastMessage(ToastType.Danger, "Failed to parse input", ex.Message));
                    return Stream.Null;
                }

                var outputData = TransformParser.Apply(results, transforms);

                var keys = transformParser.Keys();
                var outStream = new MemoryStream();
                var outputWriter = new CsvOutputWriter();
                await outputWriter.WriteToAsync(keys, outputData, outStream);
                return outStream;
            });
            
        }

        private async Task InitiateOutputDownload()
        {
            var stream = _inputFile!.OpenReadStream();
            var tmpPath = Path.GetTempFileName();
            var fs = File.Create(tmpPath);
            _inputFile = null;
            await stream.CopyToAsync(fs);

            using var outStream = await Convert(fs);
            if (outStream.Length == 0)
            {
                return;
            }

            var fileName = "output.csv";

            using var streamRef = new DotNetStreamReference(stream: outStream);
            await JSRuntime.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
        }


        private async Task OnSubmit(EventArgs e)
        {
            _inputFileKey = Guid.NewGuid();
            _submitButton.ShowLoading("Converting..");
            await InitiateOutputDownload();
            _submitButton.HideLoading();
        }
    }
}
