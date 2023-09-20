using DocoptNet;

namespace Chirp.CLI.Interfaces;
using DocoptDictionary = IParser<IDictionary<string, ArgValue>>;

public interface IArgumentsProvider
{
    public DocoptDictionary Parser { get; }
    public string Usage { get; }
    public string[] ProgramArgs { get; }
}