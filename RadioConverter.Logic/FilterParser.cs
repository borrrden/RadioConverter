using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization.NamingConventions;

using YamlDotNet.Serialization;
using NLua;

namespace RadioConverter.Logic
{
    internal readonly record struct FilterFormat(List<string> Inputs, string Script);

    public sealed class FilterParser
    {
        private readonly LuaEngine _lua = new();
        private readonly IReadOnlyList<ITransform> _inputs;

        public FilterParser(Stream source)
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            using var reader = new StreamReader(source, leaveOpen: true);
            var filter = deserializer.Deserialize<FilterFormat>(reader);
            _lua.DoString(filter.Script, "filterFunc");
            _inputs = filter.Inputs.Select(x =>
            {
                var data = new Dictionary<string, object>
                {
                    ["type"] = "mapping",
                    ["source"] = x
                };

                return TransformParser.GetTransform(data);
            }).ToList();
        }

        public bool ShouldInclude(RepeaterDirectoryEntry entry)
        {
            var inputs = _inputs.Select(x => x.Execute(entry)).ToArray();
            try
            {
                var result = _lua.GetFunction("Filter").Call(inputs);
                if (result[0] is bool b)
                {
                    return b;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
