using Xunit;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using FluentAssertions;

namespace Chirp.Infrastructure.UnitTest
{ 
    public class SeleniumTester : IDisposable
    {
        private readonly IWebDriver _driver;
        private const string BaseUrl = "http://localhost:1339";
        public SeleniumTester()
        {
            var options = new ChromeOptions();
            _driver = new RemoteWebDriver(new Uri("http://localhost:4444/wd/hub"), options);

            ResetTables();
            Login();
        }

        private void ResetTables()
        {
            Login();
            _driver.Navigate().GoToUrl($"{BaseUrl}/reset");
            _driver.FindElement(By.Id("reset-tables-btn")).Click();
            _driver.SwitchTo().Alert().Accept();
        }

        private void Logout()
        {
            _driver.Navigate().GoToUrl($"{BaseUrl}");
            _driver.FindElement(By.Id("logout")).Click();
        }

        private void Login()
        {
            var username = "chirptest";
            var email = "chirptest@bdsa.com";

            _driver.Navigate().GoToUrl($"{BaseUrl}/devlogin");
            _driver.FindElement(By.Id("username")).SendKeys(username);
            _driver.FindElement(By.Id("email")).SendKeys(email);

            _driver.FindElement(By.Id("login-btn")).Click();
        }

        [Fact]
        public void FrontPageContainsHeadingCheep()
        {
            // Arrange
            _driver.Navigate().GoToUrl($"{BaseUrl}");

            // Act
            var header = _driver.FindElement(By.TagName("h1")).Text;

            // Assert
            header.Should().BeEquivalentTo("Chirp!");
        }

        [Fact]
        public void IsLoggedIn()
        {
            // Arrange
            _driver.Navigate().GoToUrl($"{BaseUrl}");

            // Act
            var header = _driver.FindElement(By.Id("logout")).Text;

            // Assert
            header.Should().BeEquivalentTo("logout [chirptest]");
        }

        [Fact]
        public void PostCheep()
        {
            // Arrange
            _driver.Navigate().GoToUrl($"{BaseUrl}");

            // Act
            var message  = Guid.NewGuid().ToString();
            _driver.FindElement(By.Id("cheep-text-area")).SendKeys(message);
            _driver.FindElement(By.Id("cheep-btn")).Click();

            // Assert
            var pageSource = _driver.PageSource;
            pageSource.Contains(message).Should().BeTrue();
        }

        [Fact]
        public void CheepCharacterCount()
        {
            // Arrange
            _driver.Navigate().GoToUrl($"{BaseUrl}");

            // Act
            var message  = Guid.NewGuid().ToString();
            _driver.FindElement(By.Id("cheep-text-area")).SendKeys(message);

            // Assert
            var cheepCount = _driver.FindElement(By.Id("cheep-count"));
            cheepCount.Should().NotBeNull();
            cheepCount.Text.Should().Be($"{message.Length}/160");
        }

        [Fact]
        public void CanFollowUser()
        {
            // Arrange
            _driver.Navigate().GoToUrl($"{BaseUrl}");

            // Act
            _driver.FindElement(By.Id("follow-button")).Click();

            // Assert
            _driver.FindElement(By.Id("timeline")).Click();
            var followingText = _driver.FindElement(By.CssSelector("p.follows")).Text;
            followingText.Should().NotBeNull();
            followingText.Should().Be("Following: 1");
        }

        [Fact]
        public void CanUnfollowUser()
        {
            // Arrange
            _driver.Navigate().GoToUrl($"{BaseUrl}");
            _driver.FindElement(By.Id("follow-button")).Click();

            // Act
            _driver.FindElement(By.Id("unfollow-button"));

            // Assert
            _driver.FindElement(By.Id("timeline")).Click();
            var followingElement = _driver.FindElement(By.CssSelector("p.follows"));
            var followingText = followingElement.Text;
            followingText.Should().NotBeNull();
            followingText.Should().Be("Following: 0");
        }

        [Fact]
        public void CanLikeCheeps()
        {
            // Arrange
            _driver.Navigate().GoToUrl($"{BaseUrl}");
            var likedCount = 5;

            // Act
            for (int i = 0; i < likedCount; i++)
            {
                _driver.FindElement(By.Id("like-button-not-liked")).Click();
            }

            // Assert
            _driver.Navigate().GoToUrl($"{BaseUrl}/user-information");
            var table = _driver.FindElement(By.Id("liked-cheeps"));
            var rows = table.FindElements(By.CssSelector("tbody tr"));

            rows.Should().NotBeNull();
            likedCount.Should().Be(rows.Count - 1);
        }

        [Fact]
        public void CanDislikeCheeps()
        {
            // Arrange
            _driver.Navigate().GoToUrl($"{BaseUrl}");
            var likedCount = 5;

            for (int i = 0; i < likedCount; i++)
            {
                _driver.FindElement(By.Id("like-button-not-liked")).Click();
            }

            // Act
            for (int i = 0; i < likedCount; i++)
            {
                _driver.FindElement(By.Id("like-button-liked")).Click();
            }

            // Assert
            _driver.Navigate().GoToUrl($"{BaseUrl}/user-information");
            var table = _driver.FindElement(By.Id("liked-cheeps"));
            var rows = table.FindElements(By.CssSelector("tbody tr"));

            rows.Should().NotBeNull();
            
            // One due to the header of the table
            const int emptyTableSize = 1;
            
            emptyTableSize.Should().Be(rows.Count);
        }

        [Fact]
        public void CanForgetUser()
        {
            // Arrange
            _driver.Navigate().GoToUrl($"{BaseUrl}");
            var message  = Guid.NewGuid().ToString();
            _driver.FindElement(By.Id("cheep-text-area")).SendKeys(message);
            _driver.FindElement(By.Id("cheep-btn")).Click();

            // Act
            _driver.Navigate().GoToUrl($"{BaseUrl}/user-information");
            _driver.FindElement(By.Id("forget-btn")).Click();
            _driver.SwitchTo().Alert().Accept();

            // Assert
            _driver.Navigate().GoToUrl($"{BaseUrl}");
            var pageSource = _driver.PageSource;
            pageSource.Should().NotBeNull();
            pageSource.Should().NotContain(message);
        }

        public void Dispose()
        {
            ResetTables();
            _driver.Quit();
        }
    }
}
