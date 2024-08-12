namespace RadioConverter.Logic.Transforms;

/// <summary>
/// A transform which starts at a given integral value and increments it for each row
/// </summary>
public sealed class IncrementTransform : ITransform
{
    private int _current;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="inputs">The required constructor parameter for <see cref="ITransform"/></param>
    public IncrementTransform(IReadOnlyDictionary<string, object> inputs)
    {
        if (inputs.ContainsKey("start"))
        {
            _current = int.Parse((string)inputs["start"]);
        }
    }

    /// <inheritdoc />
    public string Execute(RepeaterDirectoryEntry entry)
    {
        return _current++.ToString();
    }
}
