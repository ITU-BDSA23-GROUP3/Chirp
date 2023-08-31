// See https://aka.ms/new-console-template for more information

Console.WriteLine("Hello, World!");

var start = DateTime.Now - new DateTime(1970, 1, 1);
var end = (long)start.TotalSeconds;