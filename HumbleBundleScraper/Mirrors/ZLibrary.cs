using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HtmlAgilityPack;

namespace HumbleBundleScraper.Mirrors
{
    internal class ZLibrary : BookMirror
    {
        public override string DownloadLink { get; set; }

        public override string DownloaderName { get; set; } = "Z-Library";

        private Task<HttpResponseMessage> GetDownloadResponseAsync(string downloadLink)
        {
            var uri = new Uri(downloadLink);
            var client = new HttpClient(new HttpClientHandler { AllowAutoRedirect = true });
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/101.0.4951.64 Safari/537.36");
                                                    // TODO get the correct referer
            client.DefaultRequestHeaders.Add("Referer", "https://3lib.net/book/5416472/00bb74");
            return client.GetAsync(uri);
        }

        public override Task Download(IWebDriver driver, Book book)
        {
            driver.Url = DownloadLink;

            var bookDownloadLinks = driver.FindElements(By.XPath("//h3[contains(@itemprop, 'name')]/a"));

            foreach (var bookdownloadLink in bookDownloadLinks)
            {
                bookdownloadLink.Click();

                if (driver.FindElements(By.XPath("//a[contains(@class, 'dlButton disabled')]")).Count == 0)
                {
                    var downloadLink = driver.FindElement(By.XPath("//a[contains(@class, 'addDownloadedBook')]")).GetAttribute("href");
                    return Downloader(downloadLink, book, GetDownloadResponseAsync);
                }
                driver.Navigate().Back();
            }

            throw new ArgumentException("The book could not be found");
        }
    }
}
