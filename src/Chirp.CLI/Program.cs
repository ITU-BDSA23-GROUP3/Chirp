using System.Reflection.Metadata;
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
        var arguments = new Docopt().Apply(UserInterface.USAGE, args, exit: true);

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
            UserInterface.Read(_csvStorage.GetEntities());
        }
        else if (arguments["--help"].IsTrue)
        {
            UserInterface.Help();
        }
    }
}
