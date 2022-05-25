using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumbleBundleScraper
{
    public class LibgenScraper
    {
        private readonly IWebDriver _driver;

        private List<Book> Books { get; set; }

        private List<Book> NotFoundBooks { get; set; } = new List<Book>();

        public LibgenScraper(IWebDriver driver, List<Book> books)
        {
            _driver = driver;
            Books = books;
        }

        public void ScrapeDownloadables()
        {
            AssignDownloadUrls();
            RemoveNotDownloadable();
            SelectDownloadLinks();
        }

        private void SearchUpTheBook(Book book)
        {
            var serachBar = _driver.FindElement(By.XPath("//*[@id=\"searchform\"]"));
            serachBar.SendKeys($"{book.Title} {book.Author}");
            serachBar.Submit();
        }

        private void AssignDownloadUrls()
        {
            foreach (var book in Books)
            {
                Console.WriteLine(book.Title);
                _driver.Navigate().GoToUrl(Settings.LibGenURL);

                SearchUpTheBook(book);

                // select the book
                var tableRowSelector = "/html/body/table[3]/tbody/tr";
                var seachResults = _driver.FindElements(By.XPath(tableRowSelector));

                // [1] is skipped/omitted due to being the header
                for (int i = 1; i < seachResults.Count; i++)
                {
                    if (i > 2)
                        break;
                    //  the title selector  -> /html/body/table[3]/tbody/tr[2]/td[3]/a
                    var result = _driver.FindElement(By.XPath($"{tableRowSelector}[{i + 1}]/td[3]/a"));
                    var libgenTitle = result.Text;
                    result.Click();
                    book.LibGenResultsUrls.Add((libgenTitle, _driver.Url));
                    _driver.Navigate().Back();
                }
            }
        }

        private void RemoveNotDownloadable()
        {
            NotFoundBooks = Books
                .Where(x => x.LibGenResultsUrls.Count < 1)
                .ToList();

            Books.RemoveAll(x => x.LibGenResultsUrls.Count < 1);
        }

        private static void GetUsersSelection(Book book)
        {
            while (true)
            {
                if (!int.TryParse(Console.ReadLine().Trim(), out int choice))
                {
                    Console.WriteLine("Failed to get the input");
                    continue;
                }

                if (choice < 0 || choice >= book.LibGenResultsUrls.Count)
                {
                    Console.WriteLine($"Failed to identifie choice 0 >= {choice} < {book.LibGenResultsUrls.Count}");
                    continue;
                }

                book.DownloadLink = book.LibGenResultsUrls[choice].Item2;
                break;
            }
        }

        private static void ConsoleClean()
        {
            Console.Clear();
            Console.WriteLine("==============================================");
            Console.WriteLine("==============================================");
            Console.WriteLine("==============================================");
        }

        private void SelectDownloadLinks()
        {
            ConsoleClean();

            foreach (var book in Books)
            {
                Console.WriteLine($"The following book {book.Title} with author {book.Author} has {book.LibGenResultsUrls.Count} occurences");
                if (book.LibGenResultsUrls.Count < 2)
                {
                    book.DownloadLink = book.LibGenResultsUrls.FirstOrDefault().Item2;
                    continue;
                }

                Console.WriteLine("Please choose from the selected books which one would you like to download:");
                
                for (int i = 0; i < book.LibGenResultsUrls.Count; i++)
                {
                    // there are ISBNs as part of the text, so rather than erasing them or putting them on the same line
                    // as the name, I have decided to pad them to right, so that the numbers for choice are more visible
                    var bookName = book.LibGenResultsUrls[i].Item1.Replace("\n", "\n\t");
                    Console.WriteLine($"{i}.\t{bookName}");
                }

                book.DownloadLink = book.LibGenResultsUrls.First().Item2;
                // GetUsersSelection(book);
                ConsoleClean();
            }
        }
    }
}
