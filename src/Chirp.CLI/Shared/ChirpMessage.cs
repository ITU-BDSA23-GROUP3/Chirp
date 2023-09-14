using Chirp.CLI.Types;

namespace Chirp.CLI.Shared;
public class ChirpMessage
{
    public static ChirpRecord CreateChirpRecord(string author, string message, long timestamp)
    {
        var chirp = new ChirpRecord(author, message, timestamp); 
        return chirp;
    }
}
