using NLua;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadioConverter.Logic.Transforms
{
    public sealed class LuaTransform : ITransform
    {
        private readonly LuaEngine _lua = new();
        private readonly string _script;
        private readonly IReadOnlyList<ITransform> _inputs;

        public LuaTransform(IReadOnlyDictionary<string, object> inputs)
        {
            if (!inputs.ContainsKey("script"))
            {
                throw new ArgumentException("Lua missing script");
            }

            if (!inputs.ContainsKey("inputs"))
            {
                throw new ArgumentException("Lua missing inputs");
            }

            _script = (string)inputs["script"];
            _inputs = inputs.ConvertUntypedListOfDict("inputs").Select(x => TransformParser.GetTransform(x)).ToList();
            _lua.DoString(_script);
        }

        public string Execute(RepeaterDirectoryEntry entry)
        {
            var inputs = _inputs.Select(x => x.Execute(entry)).ToArray();
            var func = _lua["Calculate"] as LuaFunction;
            try
            {
                var result = func!.Call(inputs)[0]!.ToString();
                return result ?? "";
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
