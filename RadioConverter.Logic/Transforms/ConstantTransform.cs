namespace RadioConverter.Logic.Transforms;

/// <summary>
/// A simple transform that merely outputs a constant value for each entry.  The YAML
/// for this transform must contain an <c>inputs</c> key.
/// </summary>
public sealed class ConstantTransform : ITransform
{
    private readonly string _value;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="inputs">The required constructor parameter for <see cref="ITransform"/></param>
    /// <exception cref="ApplicationException">The provided dictionary is missing the <c>inputs</c> key</exception>
    public ConstantTransform(IReadOnlyDictionary<string, object> inputs)
    {
        if (!inputs.ContainsKey("value"))
        {
            throw new ApplicationException("Constant missing value");
        }

        _value = (string)inputs["value"];
    }

    /// <inheritdoc />
    public string Execute(RepeaterDirectoryEntry entry)
    {
        return _value;
    }
}
