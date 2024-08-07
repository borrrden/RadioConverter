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

    public sealed class RootCommand : Command<RootCommand.Settings>
    {
        public override int Execute(CommandContext context, Settings settings)
        {
            using var filterYaml = File.OpenRead("C:\\Development\\RadioConverter\\RadioConverter.Console\\filter.yaml");
            var filterParser = new FilterParser(filterYaml);

            using var yaml = File.OpenRead("C:\\Development\\RadioConverter\\RadioConverter.Console\\output_format.yaml");
            var transformParser = new TransformParser(yaml);
            var transforms = transformParser.CreateTransforms();

            var parser = new RepeaterDirectoryParser();
            var results = parser.Read(File.OpenRead(settings.Input), settings.NoDigital);

            var retVal = new List<Dictionary<string, string>>();

            foreach (var result in results)
            {
                if (!filterParser.ShouldInclude(result))
                {
                    continue;
                }

                var nextResult = new Dictionary<string, string>();
                foreach (var transform in transforms)
                {
                    nextResult[transform.Key] = transform.Value.Execute(result);
                }

                retVal.Add(nextResult);
            }

            using var fout = File.Open(settings.Output, FileMode.Truncate, FileAccess.Write, FileShare.Read);
            using var writer = new StreamWriter(fout);
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
