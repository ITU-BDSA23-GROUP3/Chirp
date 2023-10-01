using System.Data;
using Chirp.Razor.Storage.Types;
using Microsoft.Data.Sqlite;

namespace Chirp.Razor.Shared.StorageReaders;

public class CheepsFromAuthorReader
{
    public List<Cheep> Cheeps { get; } = new();
    
    public CheepsFromAuthorReader(IDataReader reader, string author)
    {
        while (reader.Read())
        {
            var dataRecord = (IDataRecord)reader;
            
            var message = dataRecord[0].ToString();
            var timestamp = dataRecord[1].ToString();
            var timestampDouble = double.Parse(timestamp??"0");

            Cheeps.Add(new Cheep(author, message??"", timestampDouble));
        }
    }
}
