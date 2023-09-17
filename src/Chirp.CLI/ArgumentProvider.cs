using Chirp.CLI.Interfaces;
using DocoptNet;

namespace Chirp.CLI;

public class ArgumentProvider : IArgumentsProvider
{
    public IDictionary<string, ValueObject>? Arguments { get; }
    public string[] ProgramArgs { get; }

    public ArgumentProvider(string[] args, string usage)
    {
        Arguments = new Docopt().Apply(usage, args, exit: true);
        ProgramArgs = args;
    }
}