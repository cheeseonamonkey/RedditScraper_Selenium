public class CommentsPage
{
    private ChromeDriver _driver;

    public CommentsPage(ChromeDriver driver, string url)
    {
        _driver = driver;
        Url = url;

        try
        {
            driver.Navigate().GoToUrl(url);
            driver.WaitForAjax();
        }
        catch (WebDriverException ex)
        {
            Console.WriteLine($"Navigation error: {ex.Message}");
            // Implement retry logic if necessary
        }
    }

    public string Url { get; }

    public List<string> GetAllComments()
    {
        List<string> comments = new List<string>();

        try
        {
            var allLinks = _driver.FindElements(By.ClassName("usertext-body")).ToList();
            allLinks.RemoveRange(0, 2);
            foreach (var link in allLinks)
                comments.Add(link.Text);
        }
        catch (NoSuchElementException ex)
        {
            Console.WriteLine($"Element not found: {ex.Message}");
        }
        catch (WebDriverException ex)
        {
            Console.WriteLine($"WebDriver error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
        }

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

        try
        {
            var allLinks = _driver.FindElements(By.CssSelector("a[href*='/comments/']"));

            foreach (var link in allLinks)
            {
                links.Add(link.GetAttribute("href"));
            }
        }
        catch (NoSuchElementException ex)
        {
            Console.WriteLine($"Element not found: {ex.Message}");
        }
        catch (WebDriverException ex)
        {
            Console.WriteLine($"WebDriver error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
        }

        return links;
    }

    public void NextPage()
    {
        try
        {
            _driver.FindElement(By.LinkText("next ›")).Click();
            _driver.WaitForAjax();
        }
        catch (NoSuchElementException ex)
        {
            Console.WriteLine($"Element not found: {ex.Message}");
        }
        catch (WebDriverException ex)
        {
            Console.WriteLine($"WebDriver error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
        }
    }
}
