using Chirp.Razor.Shared.Storage;
using Chirp.Razor.Storage.Types;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Xunit;

namespace Chirp.Razor.UnitTest;

public class ChirpStorageTest
{
    private readonly ChirpStorage _sut;

    public ChirpStorageTest(ChirpStorageFixture fixture)
    {
        _sut = fixture.Storage;
    }
    
    [Fact]
    public void StoreChirp_ValueIsStored_GetValueAsAuthor()
    {
        // Arrange
        var chirp = new Cheep("Hej", "med", 0);
        var sqlQuery = $"INSERT INTO user (username, email) VALUES ('{chirp.Author}', 'random@mail.com');";
        using var conn = _sut.GetConnection();
        conn.Open();
        using var command = new SqliteCommand(sqlQuery, conn);
        command.ExecuteNonQuery();
        
        // Act
        _sut.StoreCheep(chirp);
        
        // Assert
        _sut.GetCheepsFromAuthor(0, chirp.Author).ToList().Should().BeEquivalentTo(new List<Cheep> {chirp});
    }
    
    [Fact]
    public void StoreChirp_ValueIsStored_GetValueAsPage()
    {
        // Arrange
        var chirp = new Cheep("Hej", "med", 0);
        var sqlQuery = $"INSERT INTO user (username, email) VALUES ('{chirp.Author}', 'random@mail.com');";
        using var conn = _sut.GetConnection();
        conn.Open();
        using var command = new SqliteCommand(sqlQuery, conn);
        command.ExecuteNonQuery();
        
        // Act
        _sut.StoreCheep(chirp);
        
        // Assert
        _sut.GetCheepsPerPage(0, 1).ToList().Should().BeEquivalentTo(new List<Cheep> {chirp});
    }
}
