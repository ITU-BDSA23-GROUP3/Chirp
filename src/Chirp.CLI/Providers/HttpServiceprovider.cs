using Chirp.CLI.Interfaces;

namespace Chirp.CLI.Providers;

public class HttpServiceprovider : IHttpServiceProvider
{
    private const string Conn = "http://localhost:5235";
    private HttpClient? _client;

    public HttpClient Client
    {
        get
        {
            if (_client == null)
            {
                _client = new HttpClient();
                _client.BaseAddress = new Uri(Conn);
            }
            return _client;
        }
    }
}
