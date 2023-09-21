using Chirp.CLI.Shared;

namespace Chirp.SimpleDB.Types;

public abstract record ChirpMessage(string Author, string Message)
{
    public StorageChirpRecord ToChirpRecord()
    {
        var timestamp = DateTimeHelper.DateTimeToEpoch(DateTime.Now);
        return new StorageChirpRecord(Author, Message, timestamp);
    }
}
