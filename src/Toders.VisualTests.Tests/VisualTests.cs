using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.PhantomJS;
using Toders.VisualTests.Tests.VisualTester;

namespace Toders.VisualTests.Tests
{
    [TestClass]
    public class VisualTests
    {
        [TestMethod]
        public void TestTeasers()
        {
            VisualTesterBase visualTester;

            // Not the best way to declare which tester to use, but you get the point
#if DEBUG
            visualTester = new ScreenshotVisualTester($"c:\\temp\\visual tests\\{DateTime.Now:yyyy-MM-dd_HH-mm}");
#endif
#if (!DEBUG)
            visualTester = new EyesVisualTester();
#endif
            PerformSeleniumTests(visualTester, driver =>
            {
                driver.Url = "http://localhost:2335/en/example-pages/";
                visualTester.Check(driver, "Examples Overview");
                driver.Url = "http://localhost:2335/en/example-pages/teasers/";
                visualTester.Check(driver, "Teasers");
            });
        }

        private void PerformSeleniumTests(VisualTesterBase visualTester, Action<IWebDriver> performTests)
        {
            var driver = new PhantomJSDriver();
            driver.Manage().Window.Size = new Size(1280, 980);

            try
            {
                visualTester.Initialize(driver);

                performTests(driver);

                visualTester.Close();
            }
            catch (Exception ex)
            {
                visualTester.Exception(driver);
                Console.WriteLine(ex);
            }
            finally
            {
                driver.Quit();
                visualTester.FinalizeTest();
            }
        }
    }
}
