using CsvHelper.Configuration.Attributes;

using System.Reflection;

namespace RadioConverter.Logic.Transforms
{
    public sealed class MappingTransform : ITransform
    {
        public string Source { get; }

        public MappingTransform(IReadOnlyDictionary<string, object> inputs)
        {
            if (!inputs.ContainsKey("source"))
            {
                throw new ArgumentException("Missing source parameter");
            }

            Source = (string)inputs["source"];
        }

        public string Execute(RepeaterDirectoryEntry entry)
        {

            foreach (var property in typeof(RepeaterDirectoryEntry).GetProperties())
            {
                var attribute = property.GetCustomAttribute<NameAttribute>();
                var name = attribute?.Names?.FirstOrDefault() ?? property.Name;
                if (name == Source)
                {
                    return property.GetValue(entry)?.ToString() ?? throw new ApplicationException($"Failure to map '{Source}' from input (null)");
                }
            }

            throw new InvalidDataException($"'{Source}' not found in source input!");
        }
    }
}
