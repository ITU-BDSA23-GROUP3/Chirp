using Chirp.CLI.Interfaces;
using Chirp.CLI.Types;

namespace Chirp.CLI.Shared;
public class UserInterface : IUserInterface
{
    private static string? _usage;

    public UserInterface(IArgumentsProvider argumentsProvider)
    {
        _usage = argumentsProvider.Usage;
    }
    public int Read(IEnumerable<ChirpRecord> records)
    {
        foreach (var chirpRecord in records)
        {
            Console.WriteLine(chirpRecord.ToString());
        }

        return 0;
    }

    public int Help()
    {
        Console.WriteLine(_usage);
        return 0;
    }
}

