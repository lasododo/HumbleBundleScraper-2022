using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HtmlAgilityPack;
using System.Net;

namespace HumbleBundleScraper.Mirrors
{
    internal class LibgenGSMirror : BookMirror
    {
        public override string DownloadLink { get; set; }

        public override string DownloaderName { get; set; } = "Libgen.gs";

        private Task<HttpResponseMessage> GetDownloadResponseAsync(string downloadLink)
        {
            var uri = new Uri(downloadLink);
            var client = new HttpClient(new HttpClientHandler { AllowAutoRedirect = true });
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/101.0.4951.64 Safari/537.36");
            return client.GetAsync(uri);
        }

        public override async Task Download(IWebDriver driver, Book book)
        {
            // driver.Url = DownloadLink;
            // var downloadLink = driver.FindElement(By.XPath("//*[@id=\"main\"]/tbody/tr[1]/td[2]/a")).GetAttribute("href");
            string result = "";

            using (var client = new HttpClient())
            {
                using HttpResponseMessage response = await client.GetAsync(DownloadLink);
                using HttpContent content = response.Content;
                result = await content.ReadAsStringAsync();
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(result);
            var link = doc.DocumentNode.SelectNodes("//*[@id=\"main\"]/tr[1]/td[2]/a")
                .Where(node => node.Attributes.Contains("href"))
                .Select(node => node.Attributes["href"].Value)
                .Where(href => !string.IsNullOrEmpty(href))
                .First();   // it would be better to use FirstOrDefault, however in this case, I would throw an exception anyways if it was not there

            var url = new Uri(new Uri(DownloadLink), link);
            await Downloader(url.AbsoluteUri, book, GetDownloadResponseAsync);
        }
    }
}
