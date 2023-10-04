using Chirp.Razor.Shared.Storage;
using Microsoft.Data.Sqlite;

namespace Chirp.Razor.UnitTest;

public class ChirpStorageFixture
{
    private const string PathToData = "../../../../../data";
    public ChirpStorage Storage { get; } = new(new StoragePathHandler("file:memdb1?mode=memory&cache=shared", PathToData));
    
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
            INSERT INTO user (username, email, pw_hash)
            VALUES (@author, 'random@mail.com', '123');
            """;
        using var connection = Storage.GetConnection();
        connection.Open();
        using var command = new SqliteCommand(sqlQuery, connection);
        command.Parameters.AddWithValue("@author", name);
        command.ExecuteNonQuery();
    }
}
