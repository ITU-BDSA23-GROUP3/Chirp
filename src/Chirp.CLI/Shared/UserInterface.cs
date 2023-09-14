using Chirp.CLI.Types;

namespace Chirp.CLI.Shared;
public static class UserInterface
{
    public static string USAGE { get; } = @"
Usage:
    Chirp.CLI read
    Chirp.CLI cheep <message>

Options:
    -h --help     Show this screen.
";

    public static void Read(IEnumerable<ChirpMessage> records)
    {
        foreach (var chirpMessage in records)
        {
            Console.WriteLine(chirpMessage.ToString());
        }
    }

    public static void Help(){
        Console.WriteLine(USAGE);
    }
}