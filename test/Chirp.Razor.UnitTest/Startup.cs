using Chirp.Razor.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Chirp.Razor.UnitTest;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddTransient<ChirpStorageFixture>();
    }
}
