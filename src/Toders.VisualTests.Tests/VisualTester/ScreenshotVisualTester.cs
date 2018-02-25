using System.Drawing.Imaging;
using System.IO;
using OpenQA.Selenium;

namespace Toders.VisualTests.Tests.VisualTester
{
    public class ScreenshotVisualTester : VisualTesterBase
    {
        private readonly string _path;
        private int _index;

        public ScreenshotVisualTester(string path)
        {
            _path = path;
        }

        public override void Initialize(IWebDriver driver)
        {
            Directory.CreateDirectory(_path);
        }

        public override void Check(IWebDriver driver, string name)
        {
            TakeScreenshot(driver, name);
        }

        public override void Exception(IWebDriver driver)
        {
            TakeScreenshot(driver, "exception");
            ((ITakesScreenshot)driver).GetScreenshot().SaveAsFile(Path.Combine(_path, "exception.png"), ImageFormat.Png);
        }

        private void TakeScreenshot(IWebDriver driver, string name)
        {
            ((ITakesScreenshot)driver).GetScreenshot().SaveAsFile(Path.Combine(_path, $"{_index++} - {name}.png"), ImageFormat.Png);
        }
    }
}
