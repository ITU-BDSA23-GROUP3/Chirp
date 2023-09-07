using System.Globalization;
using Chirp.Types;
using CsvHelper;
using CsvHelper.Configuration;

namespace Chirp.Storage;

public class CsvStorage : IStorage<ChirpMessage>
{
    private readonly string _path;
    private readonly CsvConfiguration _config;
    public List<ChirpMessage> Records { get; private set; }
    
    public CsvStorage(string path)
    {
        _config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
        };
        _path = path;
    }
    
    public void StoreEntity(ChirpMessage entity)
    {
        using var stream = File.Open(_path, FileMode.Append);
        using var writer = new StreamWriter(stream);
        using var csv = new CsvWriter(writer, _config);
        csv.WriteRecords(new List<ChirpMessage>{entity});
    }

    public void StoreEntities(List<ChirpMessage> entities)
    {
        using var stream = File.Open(_path, FileMode.Append);
        using var writer = new StreamWriter(stream);
        using var csv = new CsvWriter(writer, _config);
        csv.WriteRecords(entities);
    }

    public ChirpMessage GetEntity()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<ChirpMessage> GetEntities()
    {
        Records = new List<ChirpMessage>();
        using var reader = new StreamReader(_path);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        foreach (var chirpMessage in csv.GetRecords<ChirpMessage>())
        {
            Records.Add(chirpMessage);
        }

        return Records;
    }
}