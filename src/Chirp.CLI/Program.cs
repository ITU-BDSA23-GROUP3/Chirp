using Chirp.CLI.Interfaces;
using Chirp.CLI.Shared;
using Chirp.CLI.Storage;
using Chirp.CLI.Types;
using Chirp.SimpleDB.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Chirp.CLI;

/// <summary>
/// Main runner for the Chirp CLI project
/// </summary>
public static class Program
{
    private const string Usage = @"
Usage:
    Chirp.CLI read
    Chirp.CLI cheep <message>

Options:
    -h --help     Show this screen.
";

    public static void Main(string[] args)
    {
        var serviceProvider = new ServiceCollection()
            .AddSingleton<IArgumentsProvider>(new ArgumentProvider(args, Usage))
            .AddSingleton<IUserInterface, UserInterface>()
            .AddSingleton<IStorageProvider<ChirpRecord>, ChirpStorageProvider>()
            .AddSingleton<IChirpHandler, ChirpHandler>()
            .BuildServiceProvider();
        
        var handler = serviceProvider.GetService<IChirpHandler>();
        handler.HandleInput();
    }
}

