using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HumbleBundleScraper;
using OpenQA.Selenium;

namespace HumbleBundleScraper.Mirrors
{
    public interface IMirror
    {
        string DownloadLink { get; set; }
        string DownloaderName { get; set; }
        Task Download(IWebDriver driver, Book book);

    }
}
