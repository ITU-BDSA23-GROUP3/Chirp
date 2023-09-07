using Chirp.Shared;
using Chirp.Storage;
using Chirp.Types;

namespace Chirp;
public static class Program
{
    private static IStorage<ChirpMessage>? _csvStorage;
    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("I need arguments!!!!");
            return;
        }
        
        _csvStorage = CsvStorageProvider<ChirpMessage>.Storage;
        

        switch (args[0])
        {
            case "cheep":
                var message = new ChirpMessage(
                    Environment.UserName,
                    args[1],
                    DateTimeHelper.DateTimeToEpoch(DateTime.Now));
                _csvStorage.StoreEntity(message);
                break;
            case "read":
                var records = _csvStorage.GetEntities();
                foreach (var chirpMessage in records)
                {
                    Console.WriteLine(chirpMessage.ToString());
                }
                break;
                
        }
    }
}
