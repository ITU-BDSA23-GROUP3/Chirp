using Chirp.SimpleDB.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Chirp.SimpleDB.Types;

namespace Chirp.SimpleDB.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class ChirpController
{
    private readonly ILogger<ChirpController> _logger;
    private readonly IStorage<StorageChirpRecord> _storage;
    public ChirpController(ILogger<ChirpController> logger, [FromServices] IStorageProvider<StorageChirpRecord> storageProvider)
    {
        _logger = logger;
        _storage = storageProvider.Storage;
    }
    
    [HttpPost]
    public void Post(ChirpMessage message)
    {
        
        _storage.StoreEntity(message.ToChirpRecord());
    }
    
    [HttpGet]
    public List<StorageChirpRecord> Get()
    {
        return _storage.GetEntities().ToList();
    }
}
