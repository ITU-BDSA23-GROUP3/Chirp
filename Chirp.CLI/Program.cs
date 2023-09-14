﻿using Chirp.CLI.Shared;
using Chirp.CLI.Types;
using Chirp.SimpleDB.Storage;
using DocoptNet;


namespace Chirp.CLI;
public static class Program
{
    private static IStorage<ChirpMessage>? _csvStorage;
    public static void Main(string[] args)
    {

        const string usage = @"
Usage:
    Chirp.CLI read
    Chirp.CLI cheep <message>

Options:
    -h --help     Show this screen.
";

        var arguments = new Docopt().Apply(usage, args, exit: true);

        _csvStorage = CsvStorageProvider<ChirpMessage>.Storage;
        
        if (arguments["cheep"].IsTrue)
        {
            var author = Environment.UserName;
            var message = args[1];
            var timestamp = DateTimeHelper.DateTimeToEpoch(DateTime.Now);

            var chirp = new ChirpMessage(author, message, timestamp); 

            _csvStorage.StoreEntity(chirp);
        }
        else if(arguments["read"].IsTrue)
        {
            var records = _csvStorage.GetEntities();

            foreach (var chirpMessage in records)
            {
                Console.WriteLine(chirpMessage.ToString());
            }
        }
        else if (arguments["--help"].IsTrue)
        {
            Console.WriteLine(usage);
        }
    }
}
