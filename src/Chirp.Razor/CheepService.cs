using System.Data;
using Microsoft.Data.Sqlite;

public record CheepViewModel(string Author, string Message, string Timestamp);

public interface ICheepService
{
    public int GetNumOfCheeps();
    public int GetCheepsPerPage();
    public List<CheepViewModel> GetCheeps(int pageNumber);
    public List<CheepViewModel> GetCheepsFromAuthor(int pageNumber, string author);
}

public class CheepService : ICheepService
{
    static readonly string sqlDBFilePath = "../../data/chirp.db";


    // These would normally be loaded from a database for example
    private static readonly List<CheepViewModel> _cheeps = new List<CheepViewModel>();

    public static readonly int cheepsPerPage = 5;

    public int GetNumOfCheeps()
    {
        var sqlQuery = @"
            SELECT COUNT(*) FROM (
                SELECT * FROM message
            )
        ";

        using (var connection = new SqliteConnection($"Data Source={sqlDBFilePath}"))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = sqlQuery;

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var dataRecord = (IDataRecord)reader;
                return int.Parse(dataRecord[0].ToString());
            }
        }
        return 0;
    }

    public int GetCheepsPerPage()
    {
        return 5;
    }

    public List<CheepViewModel> GetCheeps(int pageNumber)
    {
        var sqlQuery = @"
            SELECT user.username, message.text, message.pub_date 
            FROM message JOIN user ON user.user_id = message.author_id 
            ORDER by message.pub_date desc 
            LIMIT @cheepsPerPage 
            OFFSET @pageNumber * @cheepsPerPage - @cheepsPerPage
        ";

        _cheeps.Clear();

        using (var connection = new SqliteConnection($"Data Source={sqlDBFilePath}"))
        {
            connection.Open();

            using (SqliteCommand command = new SqliteCommand(sqlQuery, connection))
            {
                command.Parameters.AddWithValue("@cheepsPerPage", cheepsPerPage);
                command.Parameters.AddWithValue("@pageNumber", pageNumber);

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var dataRecord = (IDataRecord)reader;

                    var author = dataRecord[0].ToString();
                    var message = dataRecord[1].ToString();
                    var timestamp = UnixTimeStampToDateTimeString(double.Parse(dataRecord[2].ToString()));

                    _cheeps.Add(new CheepViewModel(author, message, timestamp));
                }
            }
        }
        return _cheeps;
    }

    public List<CheepViewModel> GetCheepsFromAuthor(int pageNumber, string author)
    {
        var sqlQuery = @"
            SELECT message.text, message.pub_date 
            FROM message JOIN user ON user.user_id = message.author_id 
            WHERE user.username = @author 
            ORDER by message.pub_date desc
        ";

        _cheeps.Clear();

        using (var connection = new SqliteConnection($"Data Source={sqlDBFilePath}"))
        {
            connection.Open();

            using (SqliteCommand command = new SqliteCommand(sqlQuery, connection))
            {
                command.Parameters.AddWithValue("@author", author);

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var dataRecord = (IDataRecord)reader;
                    var message = dataRecord[0].ToString();
                    var timestamp = UnixTimeStampToDateTimeString(double.Parse(dataRecord[1].ToString()));

                    _cheeps.Add(new CheepViewModel(author, message, timestamp));
                }
            }
        }
        return _cheeps;
    }

    private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }

}
