using Chirp.CLI.Interfaces;
using Chirp.CLI.Shared;
using Chirp.CLI.Types;
using Chirp.SimpleDB.Storage;
using DocoptNet;

namespace Chirp.CLI;

public class ChirpHandler : IChirpHandler
{
    private readonly IStorage<ChirpRecord> _csvStorage;
    private readonly IDictionary<string, ValueObject> _arguments;
    private readonly string[] _args;
    private readonly IUserInterface _userInterface;
    public ChirpHandler(IStorageProvider<ChirpRecord> csvStorage, IArgumentsProvider argumentsProvider, IUserInterface userInterface)
    {
        _csvStorage = csvStorage.Storage;
        _arguments = argumentsProvider.Arguments;
        _args = argumentsProvider.ProgramArgs;
        _userInterface = userInterface;
    }

    public void HandleInput()
    {
        if (_arguments["cheep"].IsTrue)
        {
            var author = Environment.UserName;
            var message = _args[1];
            var timestamp = DateTimeHelper.DateTimeToEpoch(DateTime.Now);

            var chirp = new ChirpRecord(author, message, timestamp); 

            _csvStorage.StoreEntity(chirp);
        }
        else if(_arguments["read"].IsTrue)
        {
            _userInterface.Read(_csvStorage.GetEntities());
        }
        else if (_arguments["--help"].IsTrue)
        {
            _userInterface.Help();
        }
    }
}