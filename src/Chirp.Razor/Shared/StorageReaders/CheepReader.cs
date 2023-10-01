using System.Data;
using Chirp.Razor.Storage.Types;
using Microsoft.Data.Sqlite;

namespace Chirp.Razor.Shared.StorageReaders;

public class CheepReader
{
    public List<Cheep> Cheeps { get; } = new();

    public CheepReader(IDataReader reader)
    {
        while (reader.Read())
        {
            var dataRecord = (IDataRecord)reader;

            var author = dataRecord[0].ToString();
            var message = dataRecord[1].ToString();
            var timestamp = dataRecord[2].ToString();
            var timestampDouble = double.Parse(timestamp??"0");

            Cheeps.Add(new Cheep(author??"", message??"", timestampDouble));
        }
    }
}
