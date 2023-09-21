using Chirp.CLI.Interfaces;
using DocoptNet;

namespace Chirp.CLI;
using DocoptDictionary = IParser<IDictionary<string, ArgValue>>;

/// <summary>
/// Responsible for creating an Argument Provider using Docopt
/// </summary>
public class ArgumentProvider : IArgumentsProvider
{
    public DocoptDictionary Parser { get; }
    public string Usage { get; }
    public string[] ProgramArgs { get; }

    /// <inheritdoc cref="ArgumentProvider"/>
    /// 
    /// <param name="args">String array with arguments</param>
    /// <param name="usage">Usage string</param>
    public ArgumentProvider(string[] args, string usage)
    {
        Parser = Docopt.CreateParser(usage).WithVersion("0.0");
        Usage = usage;
        ProgramArgs = args;
    }
}
