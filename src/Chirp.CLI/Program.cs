using Chirp.CLI.Interfaces;
using Chirp.CLI.Storage;
using Chirp.CLI.Types;
using Chirp.SimpleDB.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Chirp.CLI;
public static class Program
{
    public static void Main(string[] args)
    {
        var serviceProvider = new ServiceCollection()
            .AddSingleton<IStorageProvider<ChirpRecord>, ChirpStorageProvider>()
            .AddSingleton<IChirpHandler, ChirpHandler>()
            .BuildServiceProvider();
        
        var handler = serviceProvider.GetService<IChirpHandler>();
        handler.HandleInput(args);
    }
}

