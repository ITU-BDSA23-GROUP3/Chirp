using Chirp.Razor.Shared.Storage;
using Microsoft.Data.Sqlite;

namespace Chirp.Razor.UnitTest;

public class ChirpStorageFixture
{
    private const string PathToData = "./../../../../../data";
    public ChirpStorage Storage { get; } = new("file:memdb1?mode=memory&cache=shared");
    
    public ChirpStorageFixture()
    {
        var sqlQuery = File.ReadAllText($"{PathToData}/schema.sql");
        using var conn = Storage.GetConnection();
        conn.Open();
        using var command = new SqliteCommand(sqlQuery, conn);
        command.ExecuteNonQuery();
    }
}
