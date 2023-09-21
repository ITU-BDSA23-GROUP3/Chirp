using Chirp.CLI.Interfaces;

namespace Chirp.CLI.Providers;

public class HttpServiceprovider : IHttpServiceProvider
{
    private const string Conn = "http://localhost:5235";
    private const string AzureConn = "https://bdsagroup3chirpremotedb.azurewebsites.net";
    private HttpClient? _client;

    public HttpClient Client
    {
        get
        {
            if (_client == null)
            {
                _client = new HttpClient();
                _client.BaseAddress = new Uri(AzureConn);
            }
            return _client;
        }
    }
}
