using Chirp.SimpleDB.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Chirp.SimpleDB.Types;

namespace Chirp.SimpleDB.Controllers;

[ApiController]
[Route("[controller]")]
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
    public void Post(List<ChirpMessage> messages)
    {
        _storage.StoreEntities(messages.Select(message => message.ToChirpRecord()).ToList());
    }
    
    [HttpGet]
    public List<StorageChirpRecord> Get()
    {
        return _storage.GetEntities().ToList();
    }
}
