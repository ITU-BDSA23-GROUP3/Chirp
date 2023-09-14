namespace Chirp.SimpleDB.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        //Assert
        int a = 1;

        //Act
        a++;

        //Assert
        Assert.True(a == 2);
    }
}