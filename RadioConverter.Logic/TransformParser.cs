using System.Data.Common;
using System.Reflection;

using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace RadioConverter.Logic;

public sealed class TransformParser
{
    private readonly IDeserializer _deserializer;
    private readonly Stream _source;
    private static readonly List<Type> _transformTypes = new();

    static TransformParser()
    {
        ScanAssembly(Assembly.GetExecutingAssembly());
    }

    public TransformParser(Stream source)
    {
        _deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        _source = source;
    }

    public static void ScanAssembly(Assembly assembly)
    {
        _transformTypes.AddRange(assembly.GetExportedTypes().Where(x => x.IsClass && !x.IsAbstract && x.GetInterface(typeof(ITransform).FullName!) != null));
    }

    public IReadOnlyDictionary<string, ITransform> CreateTransforms()
    {
        var retVal = new Dictionary<string, ITransform>();
        using var reader = new StreamReader(_source, leaveOpen: true);
        var inputData = _deserializer.Deserialize<YamlFormat>(reader);

        foreach (var column in inputData.Output.Columns)
        {
            retVal[(string)column["name"]] = GetTransform(column);
        }

        return retVal;
    }

    public IReadOnlyList<string> Keys()
    {
        var retVal = new List<string>();
        _source.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(_source, leaveOpen: true);
        var inputData = _deserializer.Deserialize<YamlFormat>(reader);
        return inputData.Output.Columns.Select(x => x["name"]).Cast<string>().ToList();
    }

    public static ITransform GetTransform(IReadOnlyDictionary<string, object> input)
    {
        if (!input.ContainsKey("type"))
        {
            throw new ArgumentException("Type not found in input");
        }

        var transformType = _transformTypes.FirstOrDefault(x => x.Name.ToLowerInvariant().StartsWith((string)input["type"]));
        if (transformType == null)
        {
            throw new InvalidOperationException($"Unknown transform '{input["type"]}'");
        }

        return (Activator.CreateInstance(transformType, input) as ITransform)!;
    }
}

internal readonly record struct YamlFormat(YamlOutputFormat Output);

internal readonly record struct YamlOutputFormat(List<Dictionary<string, object>> Columns);