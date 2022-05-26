using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumbleBundleScraper.Mirrors
{
    public abstract class BookMirror : IMirror
    {
        public abstract string DownloadLink { get; set; }

        public abstract string DownloaderName { get; set; }

        public abstract Task Download(IWebDriver driver, Book book);

        public static void EnsurePathsCreated(Book book)
        {
            if (!Directory.Exists(Settings.DownloadFolder))
                Directory.CreateDirectory(Settings.DownloadFolder);

            if (!Directory.Exists(book.DownloadFolder))
                Directory.CreateDirectory(book.DownloadFolder);
        }

        public static BookMirror GetCorrectMirrorDownloader(string tagText)
        {
            switch (tagText)
            {
                case "this mirror":
                    return new LibgenMirror();
                case "Libgen.gs":
                    return new LibgenGSMirror();
                case "Z-Library":
                    // return new ZLibrary();   // commented out, because it is not implemented correctly for async 
                default:
                    throw new NotImplementedException("Other download links are not currently implemented, please continue manually!");
            }
        }

        internal static async Task Downloader(string downloadLink, Book book, Func<string, Task<HttpResponseMessage>> getDownloadResponseAsync)
        {
            var response = await getDownloadResponseAsync(downloadLink);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                throw new ArgumentException($"Server has not responsed with 200, but it did with {response.StatusCode} ({(int) response.StatusCode}). Link -> {downloadLink}");

            EnsurePathsCreated(book);
            var ext = Path.GetExtension(response.Content.Headers.ContentDisposition.FileName.Replace("\"", ""));
            var filePath = $"{book.DownloadPath}.{ext}";

            using (FileStream DestinationStream = File.Create(filePath))
            {
                Console.WriteLine($"Saving the {book.Title} by {book.Author} here: {filePath}");
                await response.Content.CopyToAsync(DestinationStream);
                Console.WriteLine($"Book was downloaded ({book.Title} by {book.Author}) and can be found here: {filePath}");
            }
        }
    }
}
