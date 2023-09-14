using Chirp.CLI.Shared;
using Chirp.CLI.Types;
using System.Collections.Generic;
using Chirp.SimpleDB.Storage;
using DocoptNet;


namespace Chirp.CLI;
public static class Program
{
    private static IStorage<ChirpMessage>? _csvStorage;
    public static void Main(string[] args)
    {

        const string usage = @"
Usage:
    Chirp.CLI read
    Chirp.CLI cheep <message>

Options:
    -h --help     Show this screen.
";

        var arguments = new Docopt().Apply(usage, args, exit: true);

        _csvStorage = CsvStorageProvider<ChirpMessage>.Storage;
        
        if (arguments["cheep"].IsTrue)
        {
            var message = CreateChirpMessage(Environment.UserName, args[1]);

            _csvStorage.StoreEntity(message);
        }
        else if(arguments["read"].IsTrue)
        {
            var records = _csvStorage.GetEntities();

            WriteOutChirpMessages(records);
        }
        else if (arguments["--help"].IsTrue)
        {
            Console.WriteLine(usage);
        }
    }

    private static ChirpMessage CreateChirpMessage(string author, string message)
    {
        var chirp = new ChirpMessage(
            author,
            message,
            DateTimeHelper.DateTimeToEpoch(DateTime.Now));

        return chirp;
    }

    private static void WriteOutChirpMessages(IEnumerable<ChirpMessage> records)
    {
        foreach (var chirpMessage in records)
        {
            Console.WriteLine(chirpMessage.ToString());
        }
    }
}
