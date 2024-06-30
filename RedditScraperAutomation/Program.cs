
global using OpenQA.Selenium;
global using OpenQA.Selenium.Chrome;
global using SeleniumExtras.WaitHelpers;
global using System;
global using System.Collections.Generic;
global using System.IO;
global using System.Text.RegularExpressions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

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

        var driver = ChromeDriverExtensions.NewChromeDriver(headless: true);

        try
        {
            driver.Navigate().GoToUrl("https://old.reddit.com/r/all");

            var rAll = new RAllPage(driver);

            var commentLinks = new List<string>();

            for (int i = 0; i < 48; i++)
            {
                frontPagesScraped++;
                commentLinks.AddRange(rAll.GetAllCommentLinks());
                Console.WriteLine($"Perusing /r/All... ({i} of 48) (thread URLs: {commentLinks.Count})");
                rAll.NextPage();
            }

            for (int i = 0; i < commentLinks.Count; i++)
            {
                try
                {
                    string commentLink = commentLinks[i];
                    File.AppendAllText(threadUrlsPath, commentLink + "\n");

                    var commentPage = new CommentsPage(driver, commentLink);
                    var threadComments = commentPage.GetAllComments();

                    if (threadComments.Count == 0)
                    {
                        Console.WriteLine($"No comments found for thread: {commentLink}");
                        continue; // Move to the next thread
                    }

                    threadPagesScraped++;

                    var subredditMatch = Regex.Match(commentLink, @"old\.reddit\.com/r/([^/]+)/comments");
                    string subreddit = subredditMatch.Success ? subredditMatch.Groups[1].Value : "Unknown";

                    if (subredditThreadCounts.ContainsKey(subreddit))
                        subredditThreadCounts[subreddit]++;
                    else
                        subredditThreadCounts[subreddit] = 1;

                    if (subredditCommentCounts.ContainsKey(subreddit))
                        subredditCommentCounts[subreddit] += threadComments.Count;
                    else
                        subredditCommentCounts[subreddit] = threadComments.Count;

                    var allCommentsConcat = string.Join("\n", threadComments);

                    File.AppendAllText(allCommentsPath, allCommentsConcat + "\n\n");

                    string filePath = Path.Combine(commentsDirectory, $"{subreddit}_Comments.txt");
                    File.AppendAllText(filePath, allCommentsConcat + "\n\n");

                    Console.WriteLine($"Scraped {threadPagesScraped} of {commentLinks.Count} threads ({threadComments.Count} comments) ({(new FileInfo(allCommentsPath).Length / 1048576.0).ToString("0.00")} MB)");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in loop: {ex.Message}");
                }
            }

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
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }
        finally
        {
            driver.Quit();
        }
    }
}
