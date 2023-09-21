using Xunit;
using Chirp.CLI.Interfaces;
using Chirp.CLI.Services;
using Chirp.CLI.Types;
using DocoptNet;
using NSubstitute;

namespace Chirp.CLI.UnitTest;

public class ChirpHandlerTest
{
    private readonly IServiceProvider<ChirpRecord, ChirpMessage> _serviceProvider = Substitute.For<IServiceProvider<ChirpRecord, ChirpMessage>>();
    private readonly IService<ChirpRecord, ChirpMessage> _service = Substitute.For<IService<ChirpRecord, ChirpMessage>>();
    private readonly IArgumentsProvider _argsProvider = Substitute.For<IArgumentsProvider>();
    private readonly IUserInterface _ui = Substitute.For<IUserInterface>();
    private readonly ChirpHandler _sut;

    public ChirpHandlerTest()
    {
        _serviceProvider.Service.Returns(_service);
        _sut = new ChirpHandler(_serviceProvider, _argsProvider,_ui);
    }

    [Fact]
    public async Task ChirpHandler_ProvidesArgument_ArgumentsReadAndExecuted()
    {
        // Arrange
        var argDict = new Dictionary<string, ArgValue>
        {
            { "read", ArgValue.True },
            { "cheep", ArgValue.False },
            { "--help", ArgValue.False }
        };
        
        // Act
        await _sut.HandleCustomArgs(argDict);
        
        // Assert
        await _service.Received().GetAllEntities();
    }
}
