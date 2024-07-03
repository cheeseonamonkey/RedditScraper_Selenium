using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using static System.Console;

//extending ChromeDriver methods to be less annoying 


static class ChromeDriverExtensions
{


    public static int DEFAULT_TIMEOUT = 8_000;



    public static Int64 AjaxCount(this ChromeDriver driver)
    {
        var a = (Int64)(driver as IJavaScriptExecutor).ExecuteScript("return jQuery.active;");
        if (a == null)
            throw new NullReferenceException("Error: null reference when reading ajax count from driver!");
        return a;
    }




    // another life-saver
    // (optionally, returns whether there were any pending requests)
    public static bool WaitForAjax(this ChromeDriver cd)
    {


        bool oneMoreTryThenBreak = false;

        if (!cd.IsAnyRequestPending())
        {


            /* 
               Note that this tiny pre-wait here is NOT a hard wait; instead, it accommodates the slight PROCESSING delay that may occur on the client BEFORE any request is ever dispatched!

               For example, lets say that when you click 'Submit' on a hypothetical form:
                1. A few mathamatical calculations occur.
                2. Some validation-checking (or error-handling) is performed.
                3. Some data is stored locally (as a cookie).
                4. The request is built, using the form inputs.
                5. The request is finally sent.

               If steps 1-3 take too long, we will "miss" the request, and it will not be waited for.

               Trust me that this fixed a LOT of issues and is important to make this function perform as expected 100% of the time!

           */


            string? _verUrl = cd.Url;

            for (int i = 0; i < 10; i++)
            {
                cd.WaitForMs(1); // 15ms total
                if (cd.IsAnyRequestPending()) // request NOW pending...
                    break;
                if (_verUrl.ToString() != cd.Url.ToString()) // URL has changed...
                    break;
                _verUrl = cd.Url;
            }
        }

        if (!cd.IsAnyRequestPending())
        { // no requests - confirmed
            ConsoleColor.DarkYellow.WriteInColor("  (no pending requests)\n");
            return false;
        }
        else
        {
            ConsoleColor.DarkYellow.WriteInColor("  Waiting for requests...  remaining: ");

            int msElapsed = 0; // will not follow timeout from settings.ini
            int timeoutSetting = DEFAULT_TIMEOUT * 1000;

            while (true)
            {
                if (msElapsed >= timeoutSetting)
                {
                    throw new Exception("Error - timed out waiting for Ajax");
                    break;
                }

                try
                {
                    var ajaxNum = cd.AjaxCount();

                    ConsoleColor.DarkYellow.WriteInColor($"{ajaxNum}..");
                    if (ajaxNum == 0)
                    {
                        ConsoleColor.DarkYellow.WriteInColor(" - Completed.\n");

                        // break from while
                        break;
                    }

                    Thread.Sleep(55);
                    msElapsed += 55;
                }
                catch (Exception exc)
                {
                    if (oneMoreTryThenBreak)
                    {
                        // after a second error (see ELSE), we throw the exception up call stack
                        ConsoleColor.Red.WriteLnInColor($"\n\n-----\n error in WaitForAjax!   (a delayed retry did not help)    -  \n\t\texception msg:{exc.Message}\n-----\n\n");
                        throw exc;
                    }
                    else
                    {
                        // before we start throwing exceptions around, try one more time (after a hard delay)...
                        var damount = 2500;

                        ConsoleColor.Red.WriteLnInColor($"\n\n-----\n error in WaitForAjax!" +
                                                        $"\n-----\n   will try again after a ({(Double)damount / 1000}s) delay... -  \n\t\texception msg:{exc.Message}\n-----\n\n");
                        oneMoreTryThenBreak = true;
                        Thread.Sleep(damount);

                        // continue looping while
                    }
                }
            }

            return true;
        }
    }


    public static void SetZoom(this ChromeDriver cd, byte pctg) => cd.ExecuteScript($"document.body.style.zoom='{pctg}%'");

    public static bool IsAnyRequestPending(this ChromeDriver driver) => (driver.AjaxCount() > 0);

    //public static bool WaitForAjax(this ChromeDriver cd)



    public static void WaitUntilVisibleAndClickable(this By elby, ChromeDriver cdriver)
    {
        try
        {
            WebDriverWait wait = new WebDriverWait(cdriver, TimeSpan.FromSeconds(DEFAULT_TIMEOUT));
            wait.Until(ExpectedConditions.ElementIsVisible(elby));
            var v = cdriver.FindElement(elby);
            v.WaitUntilClickable(cdriver);
        }
        catch (Exception exc)
        {
            throw exc;
        }
    }

    public static void WaitUntilVisible(this By elby, ChromeDriver cdriver)
    {

        try
        {
            WebDriverWait wait = new WebDriverWait(cdriver, TimeSpan.FromSeconds(DEFAULT_TIMEOUT));
            wait.Until(ExpectedConditions.ElementIsVisible(elby));
        }
        catch (Exception exc)
        {
            throw exc;
        }
    }

    public static void WaitUntilClickable(this IWebElement el, ChromeDriver cdriver)
    {
        try
        {


            WebDriverWait wait = new WebDriverWait(cdriver, TimeSpan.FromSeconds(DEFAULT_TIMEOUT));
            wait.Until(ExpectedConditions.ElementToBeClickable(el));
        }
        catch (Exception exc)
        {
            throw exc;
        }
    }

    public static void WaitUntilInvisible(this By elementBy, ChromeDriver driver) => driver.WaitForElementInvisible(elementBy);

    public static void WaitForElementInvisible(this ChromeDriver driver, By elementBy)
    {
        try
        {

        }
        catch (Exception exc)
        {
            new WebDriverWait(driver, TimeSpan.FromMilliseconds(DEFAULT_TIMEOUT * 1000)).Until(ExpectedConditions.InvisibilityOfElementLocated(elementBy));
            throw exc;
        }
    }
    public static void WaitForMs(this ChromeDriver driver, int ms)
    {
        try
        {
            Thread.Sleep(ms);
        }
        catch (Exception exc)
        {
            throw new Exception("An error occurred while waiting.", exc);
        }
    }

    public static void WaitForAlertClear(this ChromeDriver driver)
    {
        try
        {

            new WebDriverWait(driver, TimeSpan.FromSeconds(DEFAULT_TIMEOUT)).Until(ExpectedConditions.AlertState(false));
        }
        catch (Exception exc)
        {
            throw exc;
        }
    }

    public static IWebElement FindElByXpath(this ChromeDriver driver, string xpath)
    {
        try
        {


            if (String.IsNullOrEmpty(xpath))
                throw new Exception("no xpath string");

            IWebElement el = driver.FindElement(By.XPath(xpath));
            return el;

        }
        catch (Exception exc)
        {
            throw exc;
        }
    }
    public static IWebElement FindElBySelector(this ChromeDriver driver, string querySelector)
    {
        try
        {

            if (String.IsNullOrEmpty(querySelector))
                throw new Exception("no query selector");

            IWebElement el = driver.FindElement(By.CssSelector(querySelector));
            return el;
        }
        catch (Exception exc)
        {
            throw exc;
        }
    }

    public static void NewTab(this ChromeDriver driver)
    {
        // Open a new tab
        driver.ExecuteScript("window.open();");
    }


    public static void Navigate(this ChromeDriver driver, string url)
    {
        try
        {

            if (String.IsNullOrEmpty(url))
                throw new Exception("no navigation url");
            if (!url.Contains("http"))
                throw new Exception("navigation url missing schema (i.e. https)");

            //listens for the document.readyState to be complete, then returns control from the Navigate() call
            //     (works pretty well, although feels uncomfortable without 'await')
            driver.Navigate().GoToUrl(url);

        }
        catch (Exception exc)
        {
            throw exc;
        }


    }

    public static void WaitForElementCSS(this ChromeDriver driver, string selector)
    {
        try
        {
            new WebDriverWait(driver, TimeSpan.FromSeconds(DEFAULT_TIMEOUT))
                .Until(ExpectedConditions.ElementExists(By.CssSelector(selector)));
        }
        catch (Exception exc)
        {
            throw exc;
        }
    }

    public static void WaitForElementXPath(this ChromeDriver driver, string xpath)
    {
        try
        {
            new WebDriverWait(driver, TimeSpan.FromSeconds(DEFAULT_TIMEOUT))
        .Until(ExpectedConditions.ElementExists(By.XPath(xpath)));
        }
        catch (Exception exc)
        {
            throw exc;
        }
    }

    public static void PrintUrl(this ChromeDriver driver)
    {
        try
        {
            ConsoleColor.DarkGray.WriteLnInColor($".....DriverUrl:  {driver.Url}");
        }
        catch (Exception exc)
        {
            throw exc;
        }
    }




    public static ChromeDriver NewChromeDriver(bool headless = false)
    {




        //driver options:
        var options = new ChromeOptions();

        // prevents Selenium console spam
        options.AddExcludedArgument("enable-logging");
        options.LeaveBrowserRunning = false;

        if (headless)
            options.AddArgument("--headless=new"); // headless=new (April 2024 update to chrome driver?)

        //options.AddArgument("--window-size=1920,1080"); // Set viewport size
        options.AddArguments("--start-maximized");


        string cacheDir = "./.cache";






        //performance flags:
        options.AddUserProfilePreference("profile.default_content_setting_values.images", 2);
        options.AddUserProfilePreference("webkit.webprefs.fonts_disabled", true);
        //options.AddArgument("--disable-gpu");
        options.AddArgument("--disable-javascript"); //Reddit needs JQuery, so this isn't a good option (causes errors)
        //   options.AddArgument("--disable-background-networking");
        //  options.AddArgument("--disable-backgrounding-occluded-windows");
        //  options.AddArgument("--enable-quic");
        options.AddArgument("--disable-web-security");
        options.AddArgument("--blink-settings=imagesEnabled=false");
        options.AddArgument("--disable-logging");
        // options.AddArgument("--disable-webgl");
        //options.AddArgument("--enable-low-end-device-mode");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");

        //caching:
        System.IO.Directory.Delete(cacheDir, true);
        if (!System.IO.Directory.Exists(cacheDir))
            System.IO.Directory.CreateDirectory(cacheDir);
        options.AddArgument($"--disk-cache-dir={cacheDir}");


        //options.AddArgument("--disable-gpu-sandbox");
        // options.AddArgument("--disable-sandbox");
        options.AddArgument("--disable-clipboard");
        //options.AddArgument("--disable-network-throttling");
        options.AddArgument("--disable-background-timer-throttling");
        options.AddArgument("--disable-fonts");
        //   options.AddArgument("--disable-features=Stylesheets");
        //  options.AddArgument("--disable-animations");
        //      options.AddArgument("--prerender");
        options.AddArgument("--disable-touch-events");
        options.AddArgument("--disable-sync");
        // options.AddArgument("--disable-web-resources");       // i think one of these breaks it!
        // options.AddArgument("--enable-tcp-fast-open");        // i think one of these breaks it!

        options.AddArgument("--no-first-run");
        //options.AddArgument("--disable-resource-timing");
        options.AddArgument("--disable-extensions"); // Disable extensions
                                                     //   options.AddArgument("--disable-plugins");
                                                     // options.AddArgument("--single-process");                                                                                  

        // untested & dangerous:
        //options.PageLoadStrategy = PageLoadStrategy.Normal; // or PageLoadStrategy.Normal, PageLoadStrategy.Eager
        //options.AddArgument("--dom-distiller-enabled");
        //options.AddArgument("--enable-scheduler-preemption");
        //


        //fixes headless mode!
        options.AddArgument("--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.45 Safari/537.36");







        // downloading flags:
        //chrome_options.AddUserProfilePreference("download.default_directory", downloadDirectory);
        //chrome_options.AddUserProfilePreference("download.prompt_for_download", false);
        //chrome_options.AddUserProfilePreference("disable-popup-blocking", "true");

        // to add Network logging - look into "ChromeDriver PerformanceLoggingPreferences"

        // auto open dev tools  (gets in the way / can squish the viewport too much)
        //options.AddArgument("auto-open-devtools-for-tabs");   

        // ChromeDriver init
        ChromeDriver _initingDriver = new ChromeDriver(options);


        WriteLine("ChromeDriver init.");

        _initingDriver.WaitForMs(500);


        return _initingDriver;


    }


    //todo:
    //islogged on
    //if cookie exists:
    //.ASPXAUTH



}










