
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
        try
        {
            List<string> comments = new List<string>();

            var allLinks = _driver.FindElements(By.ClassName("usertext-body")).ToList();
            allLinks.RemoveRange(0, 2);
            foreach (var link in allLinks)
                comments.Add(link.Text);

            return comments;
        }
        catch (Exception err)
        {
            Console.WriteLine("Error! " + err.Message);
            Console.WriteLine("\t(press eneter to continue scraping...)");
            Console.ReadLine();

            return new List<string>();
        }
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

