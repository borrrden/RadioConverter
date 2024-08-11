using Spectre.Console.Cli;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RadioConverter.Logic;
using CsvHelper;
using System.Globalization;
using System.ComponentModel.DataAnnotations;

namespace RadioConverter.Console
{
    using Console = System.Console;

    public sealed class RootCommand : AsyncCommand<RootCommand.Settings>
    {
        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
        {
            using var filterYaml = File.OpenRead("C:\\Development\\RadioConverter\\RadioConverter.Console\\filter.yaml");
            var filterParser = new FilterParser(filterYaml);

            using var yaml = File.OpenRead("C:\\Development\\RadioConverter\\RadioConverter.Console\\output_format.yaml");
            var transformParser = new TransformParser(yaml);
            var transforms = transformParser.CreateTransforms();

            var parser = new RepeaterDirectoryParser();
            var results = parser.Read(File.OpenRead(settings.Input), settings.NoDigital);

            var outputData = TransformParser.Apply(results, transforms);

            var keys = transformParser.Keys();
            using var fout = File.Open(settings.Output, FileMode.Truncate, FileAccess.Write, FileShare.Read);
            var outputWriter = new CsvOutputWriter();
            await outputWriter.WriteToAsync(keys, outputData, fout);

            return 0;
        }

        public sealed class Settings : CommandSettings
        {
            [CommandArgument(0, "<input file>")]
            [Description("The input file to operate on")]
            public string Input { get; set; } = "";

            [CommandArgument(1, "<output file>")]
            [Description("The output file to write to")]
            public string Output { get; set; } = "";

            [CommandOption("-a|--no-digital")]
            [Description("Stop reading the input file once digital radio entries are reached")]
            [DefaultValue(false)]
            public bool NoDigital { get; set; }
        }
    }
}
