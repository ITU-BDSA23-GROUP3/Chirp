using Chirp.Razor.Shared.Storage;
using Chirp.Razor.Storage.Types;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Xunit;

namespace Chirp.Razor.UnitTest;

public class ChirpStorageTest
{
    private readonly ChirpStorageFixture _fixture;

    public ChirpStorageTest(ChirpStorageFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact]
    public void StoreChirp_ValueIsStored_GetValueAsAuthor()
    {
        _fixture.ClearDb();
        var sut = _fixture.Storage;
        
        // Arrange
        var chirp1 = new Cheep("foo", "bar", 0);
        var chirp2 = new Cheep("baz", "bar1", 0);
        var chirp3 = new Cheep("baz", "bar2", 0);
        _fixture.AddUser(chirp1.Author);
        _fixture.AddUser(chirp2.Author);
        
        // Act
        sut.StoreCheeps(new List<Cheep> {chirp1, chirp2, chirp3});
        
        // Assert
        sut.GetCheepsFromAuthor(0, chirp2.Author).ToList().Should().BeEquivalentTo(new List<Cheep> {chirp2, chirp3});
    }
    
    [Fact]
    public void StoreChirp_ValueIsStored_GetValueAsPage()
    {
        _fixture.ClearDb();
        var sut = _fixture.Storage;

        // Arrange
        var chirp1 = new Cheep("foo", "bar", 0);
        var chirp2 = new Cheep("baz", "bar1", 0);
        var chirp3 = new Cheep("baz", "bar2", 0);
        _fixture.AddUser(chirp1.Author);
        _fixture.AddUser(chirp2.Author);
        
        // Act
        sut.StoreCheeps(new List<Cheep> {chirp1, chirp2, chirp3});
        
        // Assert
        sut.GetCheepsPerPage(1, 2).ToList().Count.Should().Be(2);
        sut.GetCheepsPerPage(1, 2).ToList().Should().BeEquivalentTo(new List<Cheep> {chirp1, chirp2});
    }
}
