using Chirp.Parsers;
using Chirp.Shared;
using Chirp.Types;
using Chirp.Writers;

namespace Chirp;
public static class Program
{
    private const string Path = "../Data/chirp_cli_db.csv";
    private static IFileWriter? _fileWriter;
    private static IFileReader? _fileReader;
    private static ChirpCsvParser? _chirpCsvParser;
    public static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("I need arguments!!!!");
            return;
        }

        _fileWriter = new FileWriter(Path);
        _fileReader = new FileReader(Path);
        _chirpCsvParser = new ChirpCsvParser(_fileReader);
        

        switch (args[0])
        {
            case "cheep":
                var message = new ChirpMessage(
                    Environment.UserName,
                    args[1],
                    DateTimeHelper.DateTimeToEpoch(DateTime.Now));
                _fileWriter.AddLine(message);
                await _fileWriter.WriteAsync(CancellationToken.None);
                break;
            case "read":
                var chirps = await _chirpCsvParser.ParseAsync(CancellationToken.None);
                foreach (var chirpMessage in chirps)
                {
                    Console.WriteLine(chirpMessage.ToString());
                }
                break;
        }
    }
}
