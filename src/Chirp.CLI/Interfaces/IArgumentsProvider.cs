using DocoptNet;

namespace Chirp.CLI.Interfaces;

public interface IArgumentsProvider
{
    public IDictionary<string, ValueObject>? Arguments { get; }
    public string[] ProgramArgs { get; }
}