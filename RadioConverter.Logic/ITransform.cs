namespace RadioConverter.Logic;

/// <summary>
/// An interface describing a transformation operation from an input file entry
/// </summary>
public interface ITransform
{
    /// <summary>
    /// Performs the transform operation on the provided entry
    /// </summary>
    /// <param name="entry">The entry to apply the transformation to</param>
    /// <returns>The result of the transformation</returns>
    /// <remarks>This interface requires a constructor that takes a <see cref="IReadOnlyDictionary{TKey, TValue}" />
    /// with types <see cref="string"/> and <see cref="object"/>
    /// </remarks>
    string Execute(RepeaterDirectoryEntry entry);
}