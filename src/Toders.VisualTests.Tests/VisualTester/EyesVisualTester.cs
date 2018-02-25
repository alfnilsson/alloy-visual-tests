using System.Drawing;
using Applitools.Selenium;
using OpenQA.Selenium;

namespace Toders.VisualTests.Tests.VisualTester
{
    public class EyesVisualTester : VisualTesterBase
    {
        private readonly Eyes _eyes;

        public EyesVisualTester()
        {
            _eyes = new Eyes
            {
                ApiKey = "YOUR_API_KEY"
            };
        }

        public override void Initialize(IWebDriver driver)
        {
            _eyes.Open(driver, "Toders", "Testing Alloy Templates", new Size(1200, 980));
        }

        public override void Check(IWebDriver driver, string name)
        {
            _eyes.CheckWindow(name);
        }

        public override void Close()
        {
            _eyes.Close();
        }

        public override void FinalizeTest()
        {
            _eyes.AbortIfNotClosed();
        }

        public override void Exception(IWebDriver driver)
        {
            _eyes.CheckWindow("Exception!");
        }
    }
}
