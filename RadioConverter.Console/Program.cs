using RadioConverter.Console;

using Spectre.Console.Cli;
using System.Diagnostics.CodeAnalysis;

internal class Program
{
    private static async Task<int> Main(string[] args)
    {
        var app = new CommandApp<RootCommand>();
        app.Configure(c =>
        {
            c.AddCommand<DumpFunctionsCommand>("dump-funcs");
        });

        return await app.RunAsync(args);
    }
}