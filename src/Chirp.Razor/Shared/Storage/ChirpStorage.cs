using System.Data;
using Chirp.Razor.Shared.StorageReaders;
using Chirp.Razor.Storage;
using Chirp.Razor.Storage.Types;
using Microsoft.Data.Sqlite;

namespace Chirp.Razor.Shared.Storage;

public class ChirpStorage : IChirpStorage
{
    private readonly string _path;
    

    public ChirpStorage(string path = "./data/chirp.db")
    {
        _path = path;
    }
    
    public int Count()
    {
        const string sqlQuery = 
            """
            SELECT COUNT(*) FROM (
                SELECT * FROM message
            )
            """;

        using var connection = GetConnection();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = sqlQuery;

        using var reader = command.ExecuteReader();

        var ret = 0;
        
        while (reader.Read())
        {
            var dataRecord = (IDataRecord)reader;
            ret = int.Parse(dataRecord[0].ToString());
        }
        
        connection.Close();
        return ret;
    }

    
    public void StoreCheep(Cheep entity)
    {
        StoreCheeps(new List<Cheep>{entity});
    }

    public void StoreCheeps(List<Cheep> entities)
    {
        var sqlQuery =
            """
            INSERT INTO message (author_id, text, pub_date)
            SELECT user_id, @text, @pubDate FROM user
            LEFT JOIN message on user_id = author_id
            WHERE username = @author
            LIMIT 1;
            """;
        using var connection = GetConnection();
        connection.Open();
        using var command = new SqliteCommand(sqlQuery, connection);
        command.Parameters.Add("@author", SqliteType.Text);
        command.Parameters.Add("@text", SqliteType.Text);
        command.Parameters.Add("@pubDate", SqliteType.Integer);
        
        entities.ForEach(cheep =>
        {
            command.Parameters[0].Value = cheep.Author;
            command.Parameters[1].Value = cheep.Message;
            command.Parameters[2].Value = cheep.Timestamp;
            if (command.ExecuteNonQuery() != 1)
            {
                throw new InvalidDataException($"The cheep provided {cheep} was not valid for insertion, check if the user exists");
            }
        });
    }

    public List<Cheep> GetCheepsFromAuthor(int pageNumber, string author)
    {
        var sqlQuery = 
            """
            SELECT user.username, message.text, message.pub_date
            FROM message JOIN user ON user.user_id = message.author_id
            WHERE user.username = @author
            ORDER by message.pub_date desc
            """;
        
        using var connection = GetConnection();
        connection.Open();
        using var command = new SqliteCommand(sqlQuery, connection);
        command.Parameters.AddWithValue("@author", author);

        using var reader = command.ExecuteReader();
        var cheepReader = new CheepReader(reader);
        
        return cheepReader.Cheeps;
    }

    public IEnumerable<Cheep> GetCheepsPerPage(int pageNumber, int amount)
    {
        if (pageNumber == 0)
        {
            throw new ArgumentException("Page number can't be zero");
        }

        var sqlQuery = 
            """
           SELECT user.username, message.text, message.pub_date
           FROM message JOIN user ON user.user_id = message.author_id
           ORDER by message.pub_date desc
           LIMIT @cheepsPerPage
           OFFSET @pageNumber * @cheepsPerPage - @cheepsPerPage
           """;

        using var connection = GetConnection();
        connection.Open();
        using var command = new SqliteCommand(sqlQuery, connection);
        command.Parameters.AddWithValue("@cheepsPerPage", amount);
        command.Parameters.AddWithValue("@pageNumber", pageNumber);
        
        using var reader = command.ExecuteReader();
        var cheepReader = new CheepReader(reader);
        
        return cheepReader.Cheeps;
    }

    public SqliteConnection GetConnection()
    {
        var connection = new SqliteConnection($"DataSource={_path}");
        return connection;
    }
}
