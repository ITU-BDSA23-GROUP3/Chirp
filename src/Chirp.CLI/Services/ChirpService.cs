using System.Text;
using System.Text.Json;
using Chirp.CLI.Interfaces;
using Chirp.CLI.Types;

namespace Chirp.CLI.Services;

public class ChirpService : IService<ChirpRecord, ChirpMessage>
{
    private readonly HttpClient _client;
    public ChirpService(HttpClient client)
    {
        _client = client;
    }

    public async Task<List<ChirpRecord>?> GetAllEntities()
    {
        var chirps = await _client.GetAsync("/Chirp");
        var jsonResult = await chirps.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<ChirpRecord>>(jsonResult);
    }

    public async Task<bool> StoreAllEntities(List<ChirpMessage> messages)
    {
        using StringContent serialized = new(JsonSerializer.Serialize(messages), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/Chirp", serialized);
        return response.IsSuccessStatusCode;
    }
    
    public async Task<bool> StoreEntity(ChirpMessage message)
    {
        return await StoreAllEntities(new(){message});
    }
}
