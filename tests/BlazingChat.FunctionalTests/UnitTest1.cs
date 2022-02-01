using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading;

namespace BlazingChat.FunctionalTests
{
    public class Tests
    {
        private WebDriver WebDriver { get; set; } = null!;

        [SetUp]
        public void Setup()
        {
            WebDriver = GetChromeDriver();
        }

        [TearDown]
        public void TearDown()
        {
            WebDriver.Quit();
        }

        [Test]
        public void IsPageTitleBlazingChat()
        {
            var webAppUrl = "https://www.blazingchat.com";

            WebDriver.Navigate().GoToUrl(webAppUrl);
            Assert.AreEqual("BlazingChat", WebDriver.Title);
        }

        [Test]
        public void LoginTest()
        {
            var webAppUrl = "https://www.blazingchat.com";
            WebDriver.Navigate().GoToUrl(webAppUrl);

            Thread.Sleep(10000);

            var userName = "julius.caesar@gmail.com";
            var password = "julius.caesar";

            var input = WebDriver.FindElement(By.Id("user_name"));
            input.Clear();
            input.SendKeys(userName);

            Thread.Sleep(2000);

            input = WebDriver.FindElement(By.Id("password"));
            input.Clear();
            input.SendKeys(password);

            Thread.Sleep(2000);
            
            input = WebDriver.FindElement(By.Id("password"));


            // Click on the login button
            Assert.Pass();
        }

        private WebDriver GetChromeDriver()
        {
            //var path = Environment.GetEnvironmentVariable("ChromeWebDriver");
            var path = @"C:\Data\CuriousDrive\WebDrivers\Chrome";
            
            var options = new ChromeOptions();
            options.AddArguments("--no-sandbox");
            //options.AddArguments("--headless");

            if (!string.IsNullOrWhiteSpace(path))
            {
                return new ChromeDriver(path, options, TimeSpan.FromSeconds(300));
            }
            else
            {
                return new ChromeDriver(options);
            }
        }
    }
}