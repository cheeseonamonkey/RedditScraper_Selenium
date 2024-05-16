global using OpenQA.Selenium;
global using OpenQA.Selenium.Chrome;
global using SeleniumExtras.WaitHelpers;
global using System;
global using System.Collections.Generic;
global using System.IO;
global using System.Text.RegularExpressions;


class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!\n");

        string outputDirectory = "Output";

        if (Directory.Exists(outputDirectory))
            Directory.Delete(outputDirectory, true);
        Directory.CreateDirectory(outputDirectory);

        string threadUrlsPath = Path.Combine(outputDirectory, "ThreadUrls.csv");
        string allCommentsPath = Path.Combine(outputDirectory, "AllCommentsConcat.txt");
        string statsPath = Path.Combine(outputDirectory, "stats.csv");
        string commentsDirectory = Path.Combine(outputDirectory, "SubredditComments");
        Directory.CreateDirectory(commentsDirectory);

        int frontPagesScraped = 0;
        int threadPagesScraped = 0;
        Dictionary<string, int> subredditThreadCounts = new Dictionary<string, int>();
        Dictionary<string, int> subredditCommentCounts = new Dictionary<string, int>();

        var driver = ChromeDriverExtensions.NewChromeDriver(headless: false);

        driver.Navigate().GoToUrl("https://old.reddit.com/r/all");

        var rAll = new RAllPage(driver);

        var commentLinks = new List<string>();

        for (int i = 0; i < 5; i++)
        {
            frontPagesScraped++;
            commentLinks.AddRange(rAll.GetAllCommentLinks());
            rAll.NextPage();
        }

        commentLinks.ForEach(commentLink =>
        {
            File.AppendAllText(threadUrlsPath, commentLink + "\n");

            var commentPage = new CommentsPage(driver, commentLink);
            var threadComments = commentPage.GetAllComments();
            threadPagesScraped++;

            var subreddit = Regex.Match(commentLink, @"reddit.com/r/([^/]+)/").Groups[1].Value;
            subredditThreadCounts[subreddit] = subredditThreadCounts.ContainsKey(subreddit) ? subredditThreadCounts[subreddit] + 1 : 1;
            subredditCommentCounts[subreddit] = subredditCommentCounts.ContainsKey(subreddit) ? subredditCommentCounts[subreddit] + threadComments.Count : threadComments.Count;

            var allCommentsConcat = string.Join("\n", threadComments);

            File.AppendAllText(allCommentsPath, allCommentsConcat + "\n\n");

            string filePath = Path.Combine(commentsDirectory, $"{subreddit}_Comments.txt");
            File.AppendAllText(filePath, allCommentsConcat + "\n\n");

            Console.WriteLine($"Scraped {threadPagesScraped} of {commentLinks.Count} threads ({threadComments.Count} comments) ({(new FileInfo(allCommentsPath).Length / 1048576.0).ToString("0.00")} MB)");
        });

        using (StreamWriter writer = new StreamWriter(statsPath))
        {
            writer.WriteLine($"Front pages scraped,{frontPagesScraped}");
            writer.WriteLine($"Threads scraped,{threadPagesScraped}");

            foreach (var kvp in subredditThreadCounts)
            {
                writer.WriteLine($"{kvp.Key} Threads,{kvp.Value}");
                writer.WriteLine($"{kvp.Key} Comments,{subredditCommentCounts[kvp.Key]}");
            }
        }

        Console.WriteLine("\nMetadata written to: stats.csv!");
    }
}

public class CommentsPage
{
    private ChromeDriver _driver;

    public CommentsPage(ChromeDriver driver, string url)
    {
        _driver = driver;
        Url = url;

        driver.Navigate().GoToUrl(url);
        driver.WaitForAjax();
    }

    public string Url { get; }

    public List<string> GetAllComments()
    {
        List<string> comments = new List<string>();

        var allLinks = _driver.FindElements(By.ClassName("usertext-body")).ToList();
        allLinks.RemoveRange(0, 2);
        foreach (var link in allLinks)
            comments.Add(link.Text);

        return comments;
    }
}

public class RAllPage
{
    private ChromeDriver _driver;

    public RAllPage(ChromeDriver driver)
    {
        _driver = driver;
    }

    public List<string> GetAllCommentLinks()
    {
        List<string> links = new List<string>();

        var allLinks = _driver.FindElements(By.CssSelector("a[href*='/comments/']"));

        foreach (var link in allLinks)
        {
            links.Add(link.GetAttribute("href"));
        }

        return links;
    }

    public void NextPage()
    {
        _driver.FindElement(By.LinkText("next ›")).Click();
        _driver.WaitForAjax();
    }
}
