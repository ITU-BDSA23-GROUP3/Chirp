using Chirp.Razor.Shared.Storage;
using Microsoft.Data.Sqlite;

namespace Chirp.Razor.UnitTest;

public class ChirpStorageFixture
{
    private const string PathToData = "./../../../../../data";
    public ChirpStorage Storage { get; } = new("file:memdb1?mode=memory&cache=shared");
    // public ChirpStorage Storage { get; } = new($"{PathToData}/test_db.db");
    
    public ChirpStorageFixture()
    {
        ClearDb();
    }

    public void ClearDb()
    {
        var sqlQuery = File.ReadAllText($"{PathToData}/schema.sql");
        using var conn = Storage.GetConnection();
        conn.Open();
        using var command = new SqliteCommand(sqlQuery, conn);
        command.ExecuteNonQuery();
    }

    public void AddUser(string name)
    {
        var sqlQuery =
            """
            INSERT INTO user (username, email)
            VALUES (@author, 'random@mail.com');
            """;
        using var connection = Storage.GetConnection();
        connection.Open();
        using var command = new SqliteCommand(sqlQuery, connection);
        command.Parameters.AddWithValue("@author", name);
        command.ExecuteNonQuery();
    }
}
