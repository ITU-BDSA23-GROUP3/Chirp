using Chirp.CLI.Interfaces;
using Chirp.CLI.Providers;
using Chirp.CLI.Shared;
using Chirp.CLI.Types;
using Microsoft.Extensions.DependencyInjection;

namespace Chirp.CLI;
public static class Program
{
    private const string Usage = @"
Usage:
    Chirp.CLI read
    Chirp.CLI cheep <message>

Options:
    -h --help     Show this screen.
";

    public static async Task Main(string[] args)
    {
        var serviceProvider = new ServiceCollection()
            .AddSingleton<IArgumentsProvider>(new ArgumentProvider(args, Usage))
            .AddSingleton<IUserInterface, UserInterface>()
            .AddSingleton<IServiceProvider<ChirpRecord, ChirpMessage>, ChirpServiceProvider>()
            .AddSingleton<IChirpHandler, ChirpHandler>()
            .BuildServiceProvider();
        
        var handler = serviceProvider.GetService<IChirpHandler>();
        await handler.HandleInput();
    }
}

