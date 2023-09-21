namespace Chirp.CLI.Interfaces;

public interface IHttpServiceProvider
{
    public HttpClient Client { get; }
}
