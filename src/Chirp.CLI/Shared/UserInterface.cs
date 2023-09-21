using Chirp.CLI.Interfaces;
using Chirp.CLI.Types;

namespace Chirp.CLI.Shared;

/// <summary>
/// UserInterface is responsible for writing chirps to the screen, printing usage, general ui prints, etc.
/// </summary>
public class UserInterface : IUserInterface
{
    private static string? _usage;

    /// <inheritdoc cref="UserInterface"/>
    /// 
    /// <param name="argumentsProvider"></param>
    public UserInterface(IArgumentsProvider argumentsProvider)
    {
        _usage = argumentsProvider.Usage;
    }

    /// <summary>
    /// Read a Chirp record
    /// </summary>
    /// <param name="records"></param>
    /// <returns>Status</returns>
    public int Read(IEnumerable<ChirpRecord> records)
    {
        foreach (var chirpRecord in records)
        {
            Console.WriteLine(chirpRecord.ToString());
        }

        return 0;
    }

    /// <summary>
    /// Write usage
    /// </summary>
    /// <returns>Status</returns>
    public int Help()
    {
        Console.WriteLine(_usage);
        return 0;
    }
}

