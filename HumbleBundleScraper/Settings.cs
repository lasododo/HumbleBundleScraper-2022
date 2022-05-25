using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;


namespace HumbleBundleScraper
{
    /*
    public enum RunType
    {
        FullyAutomated,
        ManualChoices
    }
    */

    internal class Settings
    {
        #region Directory Settings (Modifications should not be necessery)

        public static readonly string _sep = Path.DirectorySeparatorChar.ToString();

        private static readonly string _baseDir = Path.GetFullPath($"{AppDomain.CurrentDomain.BaseDirectory}{_sep}..{_sep}..{_sep}..{_sep}");

        private static readonly string _driverPath = Path.GetFullPath($@"{_baseDir}..{_sep}SeleniumDrivers{_sep}");

        private static readonly List<string> _chromeOptions = new List<string>()
        {
            "--start-maximized"
        };

        public static readonly string LibGenURL = "http://libgen.rs/";

        #endregion

        #region Downloader Settings (Modify the content, not the types!)

        public static readonly string DownloadFolder = Path.GetFullPath($@"{_baseDir}..{_sep}HBDownloaded{_sep}");

        // public static readonly RunType runType = RunType.FullyAutomated;

        public static readonly List<string> HumbleBundleURLs = new List<string>()
        {
            // "https://www.humblebundle.com/books/coding-cookbooks-oreilly-books?hmb_source=&hmb_medium=product_tile&hmb_campaign=mosaic_section_1_layout_index_1_layout_type_threes_tile_index_3_c_codingcookbooksoreilly_bookbundle",
            "https://www.humblebundle.com/books/cybersecurity-cyber-warfare-packt-books?hmb_source=&hmb_medium=product_tile&hmb_campaign=mosaic_section_1_layout_index_1_layout_type_threes_tile_index_1_c_cybersecuritycyberwarfarepackt_bookbundle"
        };

        #endregion

        public static IWebDriver CreateChromeDriver()
        {
            var options = new ChromeOptions();

            foreach (var item in _chromeOptions)
                options.AddArgument(item);

            return new ChromeDriver(_driverPath, options);
        }
    }
}
