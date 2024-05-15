global using OpenQA.Selenium.Support.UI;
global using SeleniumExtras.WaitHelpers;
global using static System.Console;
global using OpenQA.Selenium;
global using OpenQA.Selenium.Chrome;
using System.Text.RegularExpressions;

Console.WriteLine("Hello, World!");

var options = new ChromeOptions();




// create driver (NewChromeDriver() contains all the config stuff)
var driver = ChromeDriverExtensions.NewChromeDriver();

driver.Navigate("https://old.reddit.com/r/all");

var rAll = new RAllPage(driver);

var clinks = rAll.GetAllCommentLinks();
clinks.ForEach(Console.WriteLine);






public class RAllPage
{
    public ChromeDriver driver;

    // Default constructor
    public RAllPage(ChromeDriver driver)
    {
        this.driver = driver;
    }

    public List<string> GetAllCommentLinks()
    {
        List<string> listOut = new List<string>();
        var allLinks = driver.FindElements(By.TagName("a"));

        foreach (var ln in allLinks)
        {
            if (!Regex.IsMatch(ln.Text, @"[0-9]{1,9} comments"))
                continue;

            listOut.Add(ln.GetDomProperty("href"));
        }

        return listOut;
    }
}
