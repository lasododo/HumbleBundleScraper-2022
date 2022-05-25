using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumbleBundleScraper
{
    public record Book(string Title, string Author, List<(string, string)> LibGenResultsUrls, List<string> DownloadLinks, string BundleName)
    {
        public string DownloadLink { get; set; } = null;

        public string DownloadFolder { get; } = $"{Settings.DownloadFolder}{$"{BundleName}{Settings._sep}".Replace(' ', '-')}";
        
        // cannot reference DownloadFolder here, because it is not static
        public string DownloadPath { get; } = $"{Settings.DownloadFolder}{$"{BundleName}{Settings._sep}{Title}".Replace(' ', '-')}";
    }
}
