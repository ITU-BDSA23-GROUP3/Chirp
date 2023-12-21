using Xunit;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using FluentAssertions;
using System;
using OpenQA.Selenium.DevTools.V117.CacheStorage;
using Chirp.Web.Pages;
using OpenQA.Selenium.Support.UI;

namespace Chirp.Infrastructure.UnitTest
{ 
    public class SeleniumTester : IDisposable
    {
        readonly IWebDriver driver;
        private const string baseUrl = "http://localhost:1339";
        public SeleniumTester()
        {
            var options = new ChromeOptions();
            driver = new RemoteWebDriver(new Uri("http://localhost:4444/wd/hub"), options);

            ResetTables();
            Login();
        }

        public void ResetTables()
        {
            Login();
            driver.Navigate().GoToUrl($"{baseUrl}/reset");
            driver.FindElement(By.Id("reset-tables-btn")).Click();
            driver.SwitchTo().Alert().Accept();
        }

        public void Logout()
        {
            driver.Navigate().GoToUrl($"{baseUrl}");
            driver.FindElement(By.Id("logout")).Click();
        }
        public void Login()
        {
            var username = "chirptest";
            var email = "chirptest@bdsa.com";

            driver.Navigate().GoToUrl($"{baseUrl}/devlogin");
            driver.FindElement(By.Id("username")).SendKeys(username);
            driver.FindElement(By.Id("email")).SendKeys(email);

            driver.FindElement(By.Id("login-btn")).Click();
        }

        [Fact]
        public void FrontPageContainsHeadingCheep()
        {
            // Arrange
            driver.Navigate().GoToUrl($"{baseUrl}");

            // Act
            var header = driver.FindElement(By.TagName("h1")).Text;

            // Assert
            header.Should().BeEquivalentTo("Chirp!");
        }

        [Fact]
        public void IsLoggedIn()
        {
            // Arrange
            driver.Navigate().GoToUrl($"{baseUrl}");

            // Act
            var header = driver.FindElement(By.Id("logout")).Text;

            // Assert
            header.Should().BeEquivalentTo("logout [chirptest]");
        }

        [Fact]
        public void PostCheep()
        {
            // Arrange
            driver.Navigate().GoToUrl($"{baseUrl}");

            // Act
            var message  = Guid.NewGuid().ToString();
            driver.FindElement(By.Id("cheep-text-area")).SendKeys(message);
            driver.FindElement(By.Id("cheep-btn")).Click();

            // Assert
            var pageSource = driver.PageSource;
            pageSource.Contains(message).Should().BeTrue();
        }

        [Fact]
        public void CheepCharacterCount()
        {
            // Arrange
            driver.Navigate().GoToUrl($"{baseUrl}");

            // Act
            var message  = Guid.NewGuid().ToString();
            driver.FindElement(By.Id("cheep-text-area")).SendKeys(message);

            // Assert
            var cheepCount = driver.FindElement(By.Id("cheep-count"));
            cheepCount.Equals($"{message.Length}/160");
        }

        [Fact]
        public void CanFollowUser()
        {
            // Arrange
            driver.Navigate().GoToUrl($"{baseUrl}");

            // Act
            driver.FindElement(By.Id("follow-button")).Click();

            // Assert
            driver.FindElement(By.Id("timeline")).Click();
            var followingText = driver.FindElement(By.CssSelector("p.follows")).Text;

            followingText.Equals("Following: 1");
        }

        [Fact]
        public void CanUnfollowUser()
        {
            // Arrange
            driver.Navigate().GoToUrl($"{baseUrl}");
            driver.FindElement(By.Id("follow-button")).Click();

            // Act
            driver.FindElement(By.Id("unfollow-button"));

            // Assert
            driver.FindElement(By.Id("timeline")).Click();
            var followingElement = driver.FindElement(By.CssSelector("p.follows"));
            var followingText = followingElement.Text;

            followingText.Equals("Following: 0");
        }

        [Fact]
        public void CanLikeCheeps()
        {
            // Arrange
            driver.Navigate().GoToUrl($"{baseUrl}");
            var likedCount = 5;

            // Act
            for (int i = 0; i < likedCount; i++)
            {
                driver.FindElement(By.Id("like-button-not-liked")).Click();
            }

            // Assert
            driver.Navigate().GoToUrl($"{baseUrl}/user-information");
            var table = driver.FindElement(By.Id("liked-cheeps"));
            var rows = table.FindElements(By.CssSelector("tbody tr"));

            likedCount.Equals(rows.Count - 1);
        }

        [Fact]
        public void CanDislikeCheeps()
        {
            // Arrange
            driver.Navigate().GoToUrl($"{baseUrl}");
            var likedCount = 5;

            for (int i = 0; i < likedCount; i++)
            {
                driver.FindElement(By.Id("like-button-not-liked")).Click();
            }

            // Act
            for (int i = 0; i < likedCount; i++)
            {
                driver.FindElement(By.Id("like-button-liked")).Click();
            }

            // Assert
            driver.Navigate().GoToUrl($"{baseUrl}/user-information");
            var table = driver.FindElement(By.Id("liked-cheeps"));
            var rows = table.FindElements(By.CssSelector("tbody tr"));

            // One due to the header of the table
            var emptyTableSize = 1;
            emptyTableSize.Equals(rows.Count);
        }

        [Fact]
        public void CanForgetUser()
        {
            // Arrange
            driver.Navigate().GoToUrl($"{baseUrl}");
            var message  = Guid.NewGuid().ToString();
            driver.FindElement(By.Id("cheep-text-area")).SendKeys(message);
            driver.FindElement(By.Id("cheep-btn")).Click();

            // Act
            driver.Navigate().GoToUrl($"{baseUrl}/user-information");
            driver.FindElement(By.Id("forget-btn")).Click();
            driver.SwitchTo().Alert().Accept();

            // Assert
            driver.Navigate().GoToUrl($"{baseUrl}");
            var pageSource = driver.PageSource;
            pageSource.Contains(message).Should().BeFalse();
        }

        public void Dispose()
        {
            ResetTables();
            driver.Quit();
        }
    }
}
