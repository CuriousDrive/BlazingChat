using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading;

namespace BlazingChat.FunctionalTests
{
    public class AuthenticationTests
    {
        private WebDriver WebDriver { get; set; } = null!;
        private string DriverPath { get; set; } = @"C:\Data\CuriousDrive\WebDrivers\Chrome";
        private string BaseUrl { get; set; } = "https://www.blazingchat.com";
        
        [SetUp]
        public void Setup()
        {
            WebDriver = GetChromeDriver();
            WebDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(120);
        }

        [TearDown]
        public void TearDown()
        {
            WebDriver.Quit();
        }

        [Test]
        public void LoginTest()
        {
            // Navigate to login page
            WebDriver.Navigate().GoToUrl(BaseUrl);

            // Enter EmailAddress
            //Thread.Sleep(5000);
            var input = WebDriver.FindElement(By.Id("input_emailaddress"));
            input.Clear();
            input.SendKeys("julius.caesar@gmail.com");

            // Enter Password
            //Thread.Sleep(5000);
            input = WebDriver.FindElement(By.Id("input_password"));
            input.Clear();
            input.SendKeys("julius.caesar");

            // Click on Login button
            //Thread.Sleep(5000);
            input = WebDriver.FindElement(By.Id("button_login"));
            input.Click();

            // Validate login message
            var startTimestamp = DateTime.Now.Millisecond;
            var endTimstamp = startTimestamp + 15000;

            while (true)
            {
                try
                {
                    input = WebDriver.FindElement(By.Id("p_welcome_message"));
                    Assert.AreEqual("Hello, julius.caesar@gmail.com", input.Text);
                    break;
                }
                catch
                {
                    var currentTimestamp = DateTime.Now.Millisecond;
                    if (currentTimestamp > endTimstamp)
                    {
                        throw;
                    }
                    Thread.Sleep(2000);
                }
            }
        }

        private WebDriver GetChromeDriver()
        {
            var options = new ChromeOptions();
            //options.AddArguments("--headless");

            return new ChromeDriver(DriverPath, options, TimeSpan.FromSeconds(300));
        }
    }
}