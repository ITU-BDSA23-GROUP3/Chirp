using Chirp.CLI.Interfaces;
using Chirp.CLI.Services;
using Chirp.CLI.Types;
// using IServiceProvider = Chirp.CLI.Interfaces.IServiceProvider<Chirp.CLI.Types.ChirpRecord, Chirp.CLI.Types.ChirpMessage>;

namespace Chirp.CLI.Providers;

public class ChirpServiceProvider : IServiceProvider<ChirpRecord, ChirpMessage>
{
    private const string Conn = "http://localhost:5235";
    private const string AzureConn = "https://bdsagroup3chirpremotedb.azurewebsites.net";
    private HttpClient? _client;
    private ChirpService? _service;

    public IService<ChirpRecord, ChirpMessage> Service
    {
        get
        {
            if (_client == null || _service == null)
            {
                _client = new HttpClient();
                _client.BaseAddress = new Uri(AzureConn);
                _service = new ChirpService(_client);
            }
            return _service;
        }
    }
}
