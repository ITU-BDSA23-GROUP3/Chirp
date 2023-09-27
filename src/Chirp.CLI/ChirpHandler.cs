using Chirp.CLI.Interfaces;
using Chirp.CLI.Types;
using DocoptNet;
using IService = Chirp.CLI.Interfaces.IService<Chirp.CLI.Types.ChirpRecord, Chirp.CLI.Types.ChirpMessage>;

namespace Chirp.CLI;

public class ChirpHandler : IChirpHandler
{
    private readonly IArgumentsProvider _argumentsProvider;
    private readonly IUserInterface _userInterface;
    private readonly IService _service;
    
    public ChirpHandler(IServiceProvider<ChirpRecord, ChirpMessage> serviceProvider, IArgumentsProvider argumentsProvider, IUserInterface userInterface)
    {
        _argumentsProvider = argumentsProvider;
        _userInterface = userInterface;
        _service = serviceProvider.Service;
    }

    public async Task<int> HandleInput()
    {
        var parser = _argumentsProvider.Parser;
        var args = _argumentsProvider.ProgramArgs;
        return parser.Parse(args) switch
        {
            IArgumentsResult<IDictionary<string, ArgValue>> { Arguments: var arguments } => await HandleCustomArgs(arguments),
            IHelpResult => _userInterface.Help(),
            IVersionResult { Version: var version } => _userInterface.Help(),
            IInputErrorResult { Usage: var usage } => _userInterface.Help(),
            var result => throw new System.Runtime.CompilerServices.SwitchExpressionException(result)
        };

    }
    
    public async Task<int> HandleCustomArgs(IDictionary<string, ArgValue> argValues)
    {
        if (argValues["cheep"].IsTrue)
        {
            var user = Environment.UserName;
            var message = _argumentsProvider.ProgramArgs[1];
            await _service.StoreEntity(new(user, message));
        }
        else if(argValues["read"].IsTrue)
        {
            var result = await _service.GetAllEntities();
            _userInterface.Read(result??new());
        }
        else if (argValues["--help"].IsTrue)
        {
            _userInterface.Help();
        }

        return 0;
    }
}
