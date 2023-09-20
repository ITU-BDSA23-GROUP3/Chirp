using System;
using System.Collections.Generic;
using System.Reflection;
using Chirp.CLI.Interfaces;
using Chirp.CLI.Types;
using Chirp.SimpleDB.Storage;
using DocoptNet;
using NSubstitute;
using Xunit.Abstractions;

namespace Chirp.CLI.Tests;

public class UnitTest1
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly IStorageProvider<ChirpRecord> _storage = Substitute.For<IStorageProvider<ChirpRecord>>();
    private readonly IStorage<ChirpRecord> _tmpStorage = Substitute.For<IStorage<ChirpRecord>>();
    private readonly IArgumentsProvider _argsProvider = Substitute.For<IArgumentsProvider>();
    private readonly IUserInterface _ui = Substitute.For<IUserInterface>();
    private readonly ChirpHandler _sut;

    public UnitTest1(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _sut = new ChirpHandler(_storage, _argsProvider,_ui);
    }

    [Fact]
    public void Test1()
    {
        _storage.Storage.Returns(_tmpStorage);
        _tmpStorage.GetEntities().Returns(new List<ChirpRecord> {new ChirpRecord("hej", "med", 123) });
        var argDict = new Dictionary<string, ArgValue>
        {
            { "read", ArgValue.True }
        };
        var parser = Substitute.For<IParser<Dictionary<string, ArgValue>>>();
        var tmp3 = Substitute.For<IArgumentsResult<Dictionary<string, ArgValue>>>();
        tmp3.Arguments.Returns(argDict);
        var result = Substitute.For<IParser<Dictionary<string, ArgValue>>.IResult>(tmp3);
        parser.Parse(Arg.Any<string[]>()).Returns(result);
        _argsProvider.ProgramArgs.Returns(new []{""});
        // _argsProvider.Arguments.Returns();
        _sut.HandleInput();
    }

}
