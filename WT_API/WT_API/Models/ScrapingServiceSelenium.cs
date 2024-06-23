using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Threading.Tasks;

namespace WT_API.Models
{   

    public class ScrapingServiceSelenium
    {
        public async Task<bool> CheckLinkAtXPathAsync(string url, string xpath)
        {
            var options = new ChromeOptions();
            options.AddArgument("--headless");

            using (var driver = new ChromeDriver(options))
            {
                driver.Navigate().GoToUrl(url);
                try
                {
                    var element = driver.FindElement(By.XPath(xpath));
                    return element != null;
                }
                catch (NoSuchElementException)
                {
                    return false;
                }
                finally
                {
                    driver.Quit();
                }
            }
        }
    }

}
