using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

namespace SeleniumTests
{
    [TestFixture]
    public class UntitledTestCase
    {
        private IWebDriver driver;
        private StringBuilder verificationErrors;
        private string baseURL;
        private bool acceptNextAlert = true;

        [SetUp]
        public void SetupTest()
        {
            driver = new ChromeDriver();
            baseURL = "http://localhost:63164/Orders";
            driver.Navigate().GoToUrl("http://localhost:63164/Orders");
            verificationErrors = new StringBuilder();
        }

        [TearDown]
        public void TeardownTest()
        {
            try
            {
                driver.Quit();
            }
            catch (Exception)
            {
                // Ignore errors if unable to close the browser
            }
            Assert.AreEqual("", verificationErrors.ToString());
        }

        [Test]
        public void TheUntitledTestCaseTest()
        {
            // ERROR: Caught exception [unknown command []]
            driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='FilterClear'])[1]/following::button[1]")).Click();
            // ERROR: Caught exception [ERROR: Unsupported command [selectWindow | win_ser_1 | ]]
            driver.FindElement(By.XPath("//input[@type='number']")).Click();
            driver.FindElement(By.XPath("//input[@type='number']")).Clear();
            driver.FindElement(By.XPath("//input[@type='number']")).SendKeys("34");
            driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='ShipCountry'])[1]/following::select[1]")).Click();
            new SelectElement(driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='ShipCountry'])[1]/following::select[1]"))).SelectByText("Austria");
            driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='ShipCountry'])[1]/following::select[1]")).Click();
            driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='ShipCity'])[1]/following::select[1]")).Click();
            new SelectElement(driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='ShipCity'])[1]/following::select[1]"))).SelectByText("Salzburg");
            driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='ShipCity'])[1]/following::select[1]")).Click();
            driver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='OrderDate'])[1]/following::button[1]")).Click();
            driver.Close();
            // ERROR: Caught exception [ERROR: Unsupported command [selectWindow | win_ser_local | ]]
            driver.Close();
        }
        private bool IsElementPresent(By by)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        private bool IsAlertPresent()
        {
            try
            {
                driver.SwitchTo().Alert();
                return true;
            }
            catch (NoAlertPresentException)
            {
                return false;
            }
        }

        private string CloseAlertAndGetItsText()
        {
            try
            {
                IAlert alert = driver.SwitchTo().Alert();
                string alertText = alert.Text;
                if (acceptNextAlert)
                {
                    alert.Accept();
                }
                else
                {
                    alert.Dismiss();
                }
                return alertText;
            }
            finally
            {
                acceptNextAlert = true;
            }
        }
    }
}
