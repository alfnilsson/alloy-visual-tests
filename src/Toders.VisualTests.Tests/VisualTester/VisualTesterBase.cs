using OpenQA.Selenium;

namespace Toders.VisualTests.Tests.VisualTester
{
    public abstract class VisualTesterBase
    {
        public virtual void Initialize(IWebDriver driver)
        {
        }

        public virtual void Check(IWebDriver driver, string name)
        {
        }

        public virtual void Close()
        {
        }

        public virtual void FinalizeTest()
        {
        }

        public virtual void Exception(IWebDriver driver)
        {
        }
    }
}