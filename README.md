# Humble Bundle Book Downloader

## Disclaimer:
-   The following project is just for educational purposes, I do not ecourage by any means piracy of any kind!
- The project was created for my personal educational use to practice the work with web client, file downloading etc.

## Requirements:
- `chromedriver`
    - (please put it in the `SeleniumDrivers` folder)
- `.NET 6`

## Current state of the project:
-   Currently, the project is not finished and I am continuesly working on it.

## TODO:
- Fix and polish the download of content using `Z-Library`
- Eliminate the 500 status codes comming from `Libgen (this mirror)`
    - Browser downloads the book without the problem
    - Client receives the 500 status code
- Make the run more modifiable
    - Add possibility to enter max number of links to visit on libgen (currently it is a single page)
    - Add support for multi-page scrape on libgen
- (would be nice) Remove selenium dependency