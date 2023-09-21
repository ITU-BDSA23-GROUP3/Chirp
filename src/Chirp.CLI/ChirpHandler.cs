using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Chirp.CLI.Interfaces;
using Chirp.CLI.Types;
using DocoptNet;

namespace Chirp.CLI;

public class ChirpHandler : IChirpHandler
{
    private readonly IParser<IDictionary<string, ArgValue>> _parser;
    private readonly string[] _args;
    private readonly IUserInterface _userInterface;
    private readonly HttpClient _client;
    
    public ChirpHandler(IHttpServiceProvider serviceProvider, IArgumentsProvider argumentsProvider, IUserInterface userInterface)
    {
        _parser = argumentsProvider.Parser;
        _args = argumentsProvider.ProgramArgs;
        _userInterface = userInterface;
        _client = serviceProvider.Client;
    }

    public async Task<int> HandleInput()
    {
        return _parser.Parse(_args) switch
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
            var author = Environment.UserName;
            var message = _args[1];
            using StringContent serialized = new(JsonSerializer.Serialize(new ChirpMessage(author, message)), Encoding.UTF8, "application/json");
            await _client.PostAsync("/Chirp", serialized);
        }
        else if(argValues["read"].IsTrue)
        {
            var chirps = await _client.GetAsync("/Chirp");
            var jsonResult = await chirps.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<ChirpRecord>>(jsonResult);
            _userInterface.Read(result!);
        }
        else if (argValues["--help"].IsTrue)
        {
            _userInterface.Help();
        }

        return 0;
    }
}
