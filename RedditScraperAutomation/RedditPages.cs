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
        }
    }

    public string Url { get; }

    public List<string> GetAllComments()
    {
        List<string> comments = new List<string>();

        try
        {
            var allLinks = _driver.FindElements(By.ClassName("usertext-body")).ToList();
            if (allLinks.Count >= 2)
                allLinks.RemoveRange(0, 2);
            foreach (var link in allLinks)
                comments.Add(link.Text);
        }
        catch (Exception ex)
        {
            Thread.Sleep(800);
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
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
            _driver.WaitForMs(650);

        }

        return links;
    }
    public void NextPage()
    {
        try
        {
            _driver.WaitForMs(65);


            var nextButton = _driver.FindElement(By.CssSelector(".next-button a"));

            if (nextButton != null)
                nextButton.Click();

            else  // delay then retry
            {

                _driver.Navigate(_driver.Url); //refresh
                _driver.WaitForMs(250);

                _driver.WaitForElementCSS(".next-button a");

                nextButton = _driver.FindElement(By.LinkText("next ›"));

                nextButton?.Click();
                _driver.WaitForAjax();

            }




            _driver.WaitForAjax();


        }
        catch (Exception ex)
        {
            // Handle other exceptions
            _driver.WaitForMs(650);
            Console.WriteLine($"Error occurred finding Next button (delaying before continuing): {ex.Message}");
        }
    }

}
