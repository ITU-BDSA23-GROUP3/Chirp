using Xunit;
using Chirp.CLI.Interfaces;
using Chirp.CLI.Types;
using DocoptNet;
using NSubstitute;

namespace Chirp.CLI.UnitTest;

public class ChirpHandlerTest
{
    private readonly IHttpServiceProvider _service = Substitute.For<IHttpServiceProvider>();
    private readonly HttpClient _client = Substitute.For<HttpClient>();
    private readonly IArgumentsProvider _argsProvider = Substitute.For<IArgumentsProvider>();
    private readonly IUserInterface _ui = Substitute.For<IUserInterface>();
    private readonly ChirpHandler _sut;

    public ChirpHandlerTest()
    {
        _sut = new ChirpHandler(_service, _argsProvider,_ui);
        _client.BaseAddress = new Uri("http://haha");
    }

    [Fact]
    public void ChirpHandler_ProvidesArgument_ArgumentsReadAndExecuted()
    {
        // Arrange
        _service.Client.Returns(_client);
        var argDict = new Dictionary<string, ArgValue>
        {
            { "read", ArgValue.True },
            { "cheep", ArgValue.False },
            { "--help", ArgValue.False }
        };
        
        // Act
        _sut.HandleCustomArgs(argDict);
        
        // Assert
        _client.Received().GetAsync("/Chirp");
    }
}
