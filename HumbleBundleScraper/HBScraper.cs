using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumbleBundleScraper
{
    public class HBScraper
    {

        private readonly IWebDriver _driver;

        public HBScraper(IWebDriver driver)
        {
            _driver = driver;
        }

        public List<Book> ScrapeBundle(string url)
        {
            _driver.Navigate().GoToUrl(url);

            var bookList = new List<Book>();
            var start = false;
            // https://stackoverflow.com/questions/1390568/how-can-i-match-on-an-attribute-that-contains-a-certain-string/1390680#1390680
            // document.querySelectorAll("div.slick-slide") <- can be used instead
            var books = _driver.FindElements(By.XPath("//div[contains(concat(' ', @class, ' '), ' slick-slide ')]"));
            var bundleName = _driver.FindElement(By.XPath("//div[contains(@class, 'basic-info-view')]/h2")).Text;
            
                                  // DEBUG
            foreach (var book in books/*.Take(3)*/)
            {
                if (!start)
                {
                    // required to open the carusel, where author can be found
                    _driver.FindElements(By.XPath("//div[contains(@class, 'tier-item-view')]/a"))[0].Click();
                    start = true;
                }
                else
                {
                    // there are 2 arrows, first is for the currently selected bundle, the second one is for the other perks.
                    // TODO: Write a more specific custom selector to remove the `First()`
                    _driver.FindElements(By.XPath("//i[contains(@class, 'hb hb-chevron-right')]")).First().Click();
                }

                // This sleep is necessery because of the JS that animates the details in
                Thread.Sleep(1000);

                var title = book.FindElement(By.XPath(".//h2[contains(@class, 'heading-medium')]")).Text;
                var author = book.FindElement(By.XPath(".//div[contains(@class, 'publishers-and-developers')]/span")).Text;
                bookList.Add(new Book(title, author, new List<(string, string)>(), new List<string>(), bundleName));
            }

            return bookList;
        }
    }
}
