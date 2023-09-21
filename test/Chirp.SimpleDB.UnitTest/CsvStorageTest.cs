using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Chirp.SimpleDB.Storage;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Chirp.SimpleDB.UnitTest;

public class CsvStorageTest
{

    [Theory]
    [MemberData(nameof(GetFileSystem))]
    public void CsvStorageTest_FileReadAndParsed_ExpectDataToBeRead(IFileSystem fileSystem)
    {
        var storage = new CsvStorage<int>(@"c:\data\chirp_cli_db.csv", fileSystem);
        storage.GetEntities().Should().BeEquivalentTo(new List<int>{1, 2});
    }
    
    [Theory]
    [MemberData(nameof(GetFileSystem))]
    public void CsvStorageTest_FileReadAndParsed_ExpectDataToBeWritten(IFileSystem fileSystem)
    {
        var storage = new CsvStorage<int>(@"c:\data\chirp_cli_db.csv", fileSystem);
        storage.StoreEntity(3);
        storage.GetEntities().Should().BeEquivalentTo(new List<int>{1, 2, 3});
    }

    public static IEnumerable<object[]> GetFileSystem()
    {
        yield return new object[]
        {
            new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {@"c:\data\chirp_cli_db.csv", new MockFileData("""
                                                               Number
                                                               1
                                                               2

                                                               """)}
            })
        };
    }
}
