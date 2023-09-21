using System.Globalization;
using System.IO.Abstractions;
using CsvHelper;
using CsvHelper.Configuration;

namespace Chirp.SimpleDB.Storage;

public class CsvStorage<T> : IStorage<T>
{
    private readonly IFileSystem _fileSystem;
    private readonly string _path;
    private readonly CsvConfiguration _config;
    
    // This loads the entire file into memory, which is fine for now but in the future we should consider using yield with an IEnumerable 
    public List<T> Records { get; private set; }

    /// <summary>
    /// Create a CSV storage
    /// </summary>
    /// <param name="path">Path to CSV storage</param>
    /// <param name="fileSystem">Filesystem</param>
    public CsvStorage(string path, IFileSystem? fileSystem = null)
    {
        _config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
        };
        _fileSystem = fileSystem ?? new FileSystem();
        _path = path;
    }
    
    /// <summary>
    /// Store an entity of a generic type
    /// </summary>
    /// <param name="entity">Entity to store</param>
    public void StoreEntity(T entity)
    {
        StoreEntities(new List<T>{entity});
    }

    /// <summary>
    /// Open file, and write to the file using the CSV format
    /// </summary>
    /// <param name="entities">List of entities</param>
    public void StoreEntities(List<T> entities)
    {
        ValidateFileOrCreate();
        using var stream = _fileSystem.File.Open(_path, FileMode.Append);
        using var writer = new StreamWriter(stream);
        using var csv = new CsvWriter(writer, _config);
        csv.WriteRecords(entities);
    }

    /// <summary>
    /// Not implemented
    /// </summary>
    /// <returns>Will return generic typed entity</returns>
    /// <exception cref="NotImplementedException">Not implemented yet, throws exception</exception>
    public T GetEntity()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get all the entities in the CSV database
    /// </summary>
    /// <returns>All entities in the CSV database</returns>
    public IEnumerable<T> GetEntities()
    {
        ValidateFileOrCreate();
        Records = new List<T>();
        using var reader = _fileSystem.File.OpenText(_path);;
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        foreach (var data in csv.GetRecords<T>())
        {
            Records.Add(data);
        }

        return Records;
    }

    /// <summary>
    /// Will check if the file exists, if not it will create the file
    /// </summary>
    private void ValidateFileOrCreate()
    {
        if (!_fileSystem.File.Exists(_path))
        {
            using (var sw = _fileSystem.File.CreateText(_path))
            using (var csv = new CsvWriter(sw, _config))
            {
                csv.WriteHeader<T>();
            }
            using (var stream = _fileSystem.File.Open(_path, FileMode.Append))
            {
                using var sw = new StreamWriter(stream);
                sw.Write(Environment.NewLine);
            }
        }
    }
}
