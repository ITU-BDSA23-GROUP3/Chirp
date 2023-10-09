// namespace Chirp.Razor.Storage.Types;
// 
// public record Cheep(string Author, string Message, double Timestamp)
// {
//     public string GetSerializedTimeStamp()
//     {
//         // Unix timestamp is seconds past epoch
//         DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
//         dateTime = dateTime.AddSeconds(Timestamp);
//         return $"{dateTime:MM/dd/yy H:mm:ss}";
//     }
// }
