namespace Chirp.Razor;
using System.Data;
using Microsoft.Data.Sqlite;

public record CheepViewModel(string Author, string Message, string Timestamp);

public interface ICheepService
{
    public List<CheepViewModel> GetCheeps();
    public List<CheepViewModel> GetCheepsFromAuthor(string author);
}

public class CheepService : ICheepService
{
    // These would normally be loaded from a database for example
    private static readonly List<CheepViewModel> _cheeps = new List<CheepViewModel>();

    public List<CheepViewModel> GetCheeps()
    {
        var sqlDBFilePath = "../../data/chirp.db";
        var sqlQuery = @"SELECT user.username, message.text, message.pub_date FROM message JOIN user ON user.user_id = message.author_id ORDER by message.pub_date desc";

        using (var connection = new SqliteConnection($"Data Source={sqlDBFilePath}"))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = sqlQuery;

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var dataRecord = (IDataRecord)reader;
                
                var author = dataRecord[0].ToString();
                var message = dataRecord[1].ToString();
                var timestamp = UnixTimeStampToDateTimeString(double.Parse(dataRecord[2].ToString()));

                _cheeps.Add(new CheepViewModel(author,message,timestamp));
            }
        }
        return _cheeps;
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author)
    {
        // filter by the provided author name
        return _cheeps.Where(x => x.Author == author).ToList();
    }

    private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }

}
