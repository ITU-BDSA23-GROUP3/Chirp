using Chirp.CLI.Interfaces;
using Chirp.CLI.Shared;
using Chirp.CLI.Types;
using Chirp.SimpleDB.Storage;
using DocoptNet;

namespace Chirp.CLI;
using DocoptDictionary = IParser<IDictionary<string, ArgValue>>;
public class ChirpHandler : IChirpHandler
{
    private readonly IStorage<ChirpRecord> _csvStorage;
    private readonly DocoptDictionary parser;
    private readonly string[] _args;
    private readonly IUserInterface _userInterface;
    public ChirpHandler(IStorageProvider<ChirpRecord> csvStorage, IArgumentsProvider argumentsProvider, IUserInterface userInterface)
    {
        _csvStorage = csvStorage.Storage;
        parser = argumentsProvider.Parser;
        _args = argumentsProvider.ProgramArgs;
        _userInterface = userInterface;
    }

    public int HandleInput()
    {
        return parser.Parse(_args) switch
        {
            IArgumentsResult<IDictionary<string, ArgValue>> { Arguments: var arguments } => HandleCustomArgs(arguments),
            IHelpResult => _userInterface.Help(),
            IVersionResult { Version: var version } => _userInterface.Help(),
            IInputErrorResult { Usage: var usage } => _userInterface.Help(),
            var result => throw new System.Runtime.CompilerServices.SwitchExpressionException(result)
        };

    }

    public int HandleCustomArgs(IDictionary<string, ArgValue> argValues)
    {
        if (argValues["cheep"].IsTrue)
        {
            var author = Environment.UserName;
            var message = _args[1];
            var timestamp = DateTimeHelper.DateTimeToEpoch(DateTime.Now);

            var chirp = new ChirpRecord(author, message, timestamp); 

            _csvStorage.StoreEntity(chirp);
        }
        else if(argValues["read"].IsTrue)
        {
            _userInterface.Read(_csvStorage.GetEntities());
        }
        else if (argValues["--help"].IsTrue)
        {
            _userInterface.Help();
        }

        return 0;
    }
}
