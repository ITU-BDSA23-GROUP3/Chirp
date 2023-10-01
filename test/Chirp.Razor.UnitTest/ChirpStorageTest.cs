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
        var chirp2 = new Cheep("baz", "bar", 0);
        AddUser(chirp1.Author, sut);
        AddUser(chirp2.Author, sut);
        
        // Act
        sut.StoreCheeps(new List<Cheep> {chirp1, chirp2});
        
        // Assert
        sut.GetCheepsFromAuthor(0, chirp1.Author).ToList().Should().BeEquivalentTo(new List<Cheep> {chirp1});
    }
    
    [Fact]
    public void StoreChirp_ValueIsStored_GetValueAsPage()
    {
        _fixture.ClearDb();
        var sut = _fixture.Storage;

        // Arrange
        var chirp1 = new Cheep("foo", "bar", 0);
        var chirp2 = new Cheep("baz", "bar", 0);
        AddUser(chirp1.Author, sut);
        AddUser(chirp2.Author, sut);
        
        // Act
        sut.StoreCheeps(new List<Cheep> {chirp1, chirp2});
        
        // Assert
        sut.GetCheepsPerPage(0, 2).ToList().Should().BeEquivalentTo(new List<Cheep> {chirp1, chirp2});
    }

    public void AddUser(string name, ChirpStorage sut)
    {
        var sqlQuery =
            """
            INSERT INTO user (username, email)
            VALUES (@author, 'random@mail.com');
            """;
        using var connection = sut.GetConnection();
        connection.Open();
        using var command = new SqliteCommand(sqlQuery, connection);
        command.Parameters.AddWithValue("@author", name);
        command.ExecuteNonQuery();
    }
}
