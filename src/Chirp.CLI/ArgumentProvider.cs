using Chirp.CLI.Interfaces;
using DocoptNet;

namespace Chirp.CLI;
using DocoptDictionary = IParser<IDictionary<string, ArgValue>>;

public class ArgumentProvider : IArgumentsProvider
{
    public DocoptDictionary Parser { get; }
    public string Usage { get; }
    public string[] ProgramArgs { get; }

    public ArgumentProvider(string[] args, string usage)
    {
        Parser = Docopt.CreateParser(usage).WithVersion("0.0");
        Usage = usage;
        ProgramArgs = args;
    }
}
