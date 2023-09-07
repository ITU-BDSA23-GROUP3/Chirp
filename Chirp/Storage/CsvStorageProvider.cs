using Chirp.Types;

namespace Chirp.Storage;

public abstract class CsvStorageProvider : IStorageProvider<ChirpMessage>
{
    public static IStorage<ChirpMessage> Storage => new CsvStorage("../Data/chirp_cli_db.csv");
}