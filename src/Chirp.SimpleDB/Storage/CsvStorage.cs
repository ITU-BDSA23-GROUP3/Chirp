using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace Chirp.SimpleDB.Storage;

public class CsvStorage<T> : IStorage<T>
{
    private readonly string _path;
    private readonly CsvConfiguration _config;
    
    // This loads the entire file into memory, which is fine for now but in the future we should consider using yield with an IEnumerable 
    public List<T> Records { get; private set; }
    
    public CsvStorage(string path)
    {
        _config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
        };
        _path = path;
    }
    
    public void StoreEntity(T entity)
    {
        using var stream = File.Open(_path, FileMode.Append);
        using var writer = new StreamWriter(stream);
        using var csv = new CsvWriter(writer, _config);
        csv.WriteRecords(new List<T>{entity});
    }

    public void StoreEntities(List<T> entities)
    {
        using var stream = File.Open(_path, FileMode.Append);
        using var writer = new StreamWriter(stream);
        using var csv = new CsvWriter(writer, _config);
        csv.WriteRecords(entities);
    }

    public T GetEntity()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<T> GetEntities()
    {
        Records = new List<T>();
        using var reader = new StreamReader(_path);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        foreach (var data in csv.GetRecords<T>())
        {
            Records.Add(data);
        }

        return Records;
    }
}