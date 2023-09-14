using Chirp.CLI.Interfaces;
using Chirp.CLI.Shared;
using Chirp.CLI.Types;
using Chirp.SimpleDB.Storage;
using DocoptNet;

namespace Chirp.CLI;

public class ChirpHandler : IChirpHandler
{
    private readonly IStorage<ChirpRecord> _csvStorage;
    public ChirpHandler(IStorageProvider<ChirpRecord> csvStorage)
    {
        _csvStorage = csvStorage.Storage;
    }

    public void HandleInput(string[] args)
    {
        var arguments = new Docopt().Apply(UserInterface.USAGE, args, exit: true);
        
        if (arguments["cheep"].IsTrue)
        {
            var author = Environment.UserName;
            var message = args[1];
            var timestamp = DateTimeHelper.DateTimeToEpoch(DateTime.Now);

            var chirp = new ChirpRecord(author, message, timestamp); 

            _csvStorage.StoreEntity(chirp);
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