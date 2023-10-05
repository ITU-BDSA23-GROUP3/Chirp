using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using var db = new ChirpDBContext();

// Note: This sample requires the database to be created before running.
Console.WriteLine($"Database path: {db.DbPath}.");

// Create
DbInitializer.SeedDatabase(db);
db.SaveChanges();

// Read
Console.WriteLine("Querying for a blog");
var cheep = db.Cheeps
    .Include(c => c.Author)
    .OrderBy(c => c.CheepId)
    .First();

// Update
Console.WriteLine(cheep);
//blog.Posts.Add(
//    new Post { Title = "Hello World", Content = "I wrote an app using EF Core!" });
db.SaveChanges();

// Delete
Console.WriteLine("Delete the blog");
//db.Remove(blog);
db.SaveChanges();