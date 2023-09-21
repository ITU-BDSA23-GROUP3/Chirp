using DocoptNet;

namespace Chirp.CLI.Interfaces;
using DocoptDictionary = IParser<IDictionary<string, ArgValue>>;

/// <summary>
/// Interface for Argument Provider
/// </summary>
public interface IArgumentsProvider
{
    public DocoptDictionary Parser { get; }
    public string Usage { get; }
    public string[] ProgramArgs { get; }
}
