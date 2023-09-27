using Chirp.CLI.Interfaces;
using Chirp.CLI.Services;
using Chirp.CLI.Types;

namespace Chirp.CLI.Providers;

public class ChirpServiceProvider : IServiceProvider<ChirpRecord, ChirpMessage>
{
    private const string Conn = "http://localhost:5235";
    private const string AzureConn = "https://bdsagroup3chirpremotedb.azurewebsites.net";
    public IService<ChirpRecord, ChirpMessage> Service { get; } = CreateService();

    private static IService<ChirpRecord, ChirpMessage> CreateService()
    {
        var client = new HttpClient();
        client.BaseAddress = new Uri(AzureConn);
        return new ChirpService(client);
    }
}
