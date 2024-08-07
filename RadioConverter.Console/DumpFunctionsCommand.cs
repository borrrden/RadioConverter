using RadioConverter.Logic;

using Spectre.Console.Cli;

using System.ComponentModel;

namespace RadioConverter.Console
{
    using Console = System.Console;

    [Description("Prints out all the built-in Lua functions for use when writing new lua type transforms")]
    public sealed class DumpFunctionsCommand : Command<DumpFunctionsCommand.Settings>
    {
        public override int Execute(CommandContext context, Settings settings)
        {
            using var outStream = settings.Output != null ? new StreamWriter(File.Open(settings.Output, FileMode.Truncate)) : Console.Out;
            var lua = new LuaEngine();
            lua.WriteFunctionDefinitions(outStream);
            return 0;
        }

        public sealed class Settings : CommandSettings
        {
            [CommandOption("-o|--output")]
            [Description("The output file to write to")]
            public string? Output { get; set; }
        }
    }
}
