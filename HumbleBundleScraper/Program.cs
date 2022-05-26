// See https://aka.ms/new-console-template for more information
using OpenQA.Selenium;
using HumbleBundleScraper;
using HumbleBundleScraper.Mirrors;

var driver = Settings.CreateChromeDriver();

var books = new List<Book>();
var hbScraper = new HBScraper(driver);

foreach (var url in Settings.HumbleBundleURLs)
{
    var scrapedBooks = hbScraper.ScrapeBundle(url);
    books.AddRange(scrapedBooks);
}

var libgenScraper = new LibgenScraper(driver, books);
libgenScraper.ScrapeDownloadables();

var tasks = new List<Task>();
var failedToDownload = new List<Book>();
foreach (var book in books)
{
    driver.Url = book.DownloadLink;
    var libgenDownloadLinks = driver.FindElements(By.XPath("/html/body/table/tbody/tr[18]/td[2]/table/tbody/tr/td"));

    var mirrorLinks = new List<IMirror>();

    // Take(2) -> only 2 are implemented
    foreach (var libgenDownloadLink in libgenDownloadLinks.Take(2))
    {     
        var aTag = libgenDownloadLink.FindElement(By.TagName("a"));
        var link = aTag.GetAttribute("href");
        IMirror bookMirror = BookMirror.GetCorrectMirrorDownloader(aTag.Text);
        bookMirror.DownloadLink = link;
        mirrorLinks.Add(bookMirror);
    }
    
    // mirrorLinks.Reverse();

    tasks.Add(Task.Run(async () =>
    {
        // try 5 times
        for (int i = 0; i < 5; i++)
        { 
            foreach (var bookMirror in mirrorLinks)
            {
                Console.WriteLine($"Using the {bookMirror.DownloaderName} mirror to download the book");

                try
                {
                    await bookMirror.Download(driver, book);
                    return;
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Failed to download the book using {bookMirror.DownloaderName} -> {e.Message}");
                }
                catch (Exception e)   // when could be used, but for now this made sense to me
                {
                    Console.WriteLine($"Failed to download the book. {bookMirror.DownloaderName}. " +
                        $"Please, try again to use a different mirror. -> {e.Message}");
                }
            }
            await Task.Delay(1000); // to avoid sending too many requests at once
        }
        failedToDownload.Add(book);
    }));
}
driver.Close();
await Task.WhenAll(tasks);
Console.WriteLine("-----------------------------------");
Console.WriteLine("-----------------------------------");
Console.WriteLine("-----------------------------------");
Console.WriteLine("Books finished downloading");

foreach (var book in failedToDownload)
    Console.WriteLine($"{book.Title} by {book.Author} was not downloaded");

