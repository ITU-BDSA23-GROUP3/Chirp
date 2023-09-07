using Chirp.CLI.Shared;
using Chirp.CLI.Types;
using Chirp.SimpleDB.Storage;
using DocoptNet;


namespace Chirp.CLI;
public static class Program
{
    private static IStorage<ChirpMessage>? _csvStorage;
    public static void Main(string[] args)
    {

        const string USAGE = @"
Usage:
    Chirl.CLI read
    Chirl.CLI cheep <message>

Options:
    -h --help     Show this screen.
";

        var arguments = new Docopt().Apply(USAGE, args, exit: true);

        _csvStorage = CsvStorageProvider<ChirpMessage>.Storage;
        
        if (arguments["cheep"].IsTrue)
        {
            var message = new ChirpMessage(
                Environment.UserName,
                args[1],
                DateTimeHelper.DateTimeToEpoch(DateTime.Now));
            _csvStorage.StoreEntity(message);
        }
        else if(arguments["read"].IsTrue)
        {
            var records = _csvStorage.GetEntities();
            foreach (var chirpMessage in records)
            {
                Console.WriteLine(chirpMessage.ToString());
            }
        }
        else if (arguments["--help"].IsTrue)
        {
            Console.WriteLine(USAGE);
        }
    }
}
