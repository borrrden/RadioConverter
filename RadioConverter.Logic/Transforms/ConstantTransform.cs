namespace RadioConverter.Logic.Transforms
{
    public sealed class ConstantTransform : ITransform
    {
        private readonly string _value;

        public ConstantTransform(IReadOnlyDictionary<string, object> inputs)
        {
            if (!inputs.ContainsKey("value"))
            {
                throw new ApplicationException("Constant missing value");
            }

            _value = (string)inputs["value"];
        }

        public string Execute(RepeaterDirectoryEntry entry)
        {
            return _value;
        }
    }
}
