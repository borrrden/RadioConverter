namespace RadioConverter.Logic.Transforms
{
    public sealed class IncrementTransform : ITransform
    {
        private int _current;

        public IncrementTransform(IReadOnlyDictionary<string, object> inputs)
        {
            if (inputs.ContainsKey("start"))
            {
                _current = int.Parse((string)inputs["start"]);
            }
        }

        public string Execute(RepeaterDirectoryEntry entry)
        {
            return _current++.ToString();
        }
    }
}
