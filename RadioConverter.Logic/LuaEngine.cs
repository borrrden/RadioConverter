using NLua;

using System.Text;

namespace RadioConverter.Logic
{
    public sealed class LuaEngine : Lua
    {
        private const string RoundFunc = """
            function Round(val, places)
              local multiplier = 10 ^ places
              return math.floor(val * multiplier + 0.5) / multiplier
            end
            """;

        private const string Is10mFunc = """
            function Is10mBand(freq)
                return freq >= 28.0 and freq <= 29.7
            end
            """;

        private const string Is6mFunc = """
            function Is6mBand(freq)
                return freq >= 50.0 and freq <= 54.0
            end
            """;

        private const string Is2mFunc = """
            function Is2mBand(freq)
                return freq >= 144.0 and freq <= 148.0
            end
            """;

        private const string Is70cmFunc = """
            function Is70cmBand(freq)
                return freq >= 420.0 and freq <= 450.0
            end
            """;

        private const string Is23cmFunc = """
            function Is23cmBand(freq)
                return freq >= 1240.0 and freq <= 1300.0
            end
            """;

        public LuaEngine() {
            State.Encoding = Encoding.UTF8;
            DoString(RoundFunc, "roundFunc");
            DoString(Is10mFunc, "is10mFunc");
            DoString(Is6mFunc, "is6mFunc");
            DoString(Is2mFunc, "is2MFunc");
            DoString(Is70cmFunc, "is70cmFunc");
            DoString(Is23cmFunc, "is23cmFunc");
        }

        public void WriteFunctionDefinitions(TextWriter outStream)
        {
            var allFunctions = new List<string> { RoundFunc, Is10mFunc, Is6mFunc, Is2mFunc, Is70cmFunc, Is23cmFunc };
            outStream.WriteLine();
            foreach (var func in allFunctions)
            {
                outStream.WriteLine(func);
                outStream.WriteLine();
            }
        }
    }
}
