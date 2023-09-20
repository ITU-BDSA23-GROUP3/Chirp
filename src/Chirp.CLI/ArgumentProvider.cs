using Chirp.CLI.Interfaces;
using DocoptNet;

namespace Chirp.CLI;

public class ArgumentProvider : IArgumentsProvider
{
    public IDictionary<string, ValueObject>? Arguments { get; }
    public string Usage { get; }
    public string[] ProgramArgs { get; }

    public ArgumentProvider(string[] args, string usage)
    {
        Usage = usage;
        Arguments = new Docopt().Apply(Usage, args, exit: true);
        ProgramArgs = args;
    }
}