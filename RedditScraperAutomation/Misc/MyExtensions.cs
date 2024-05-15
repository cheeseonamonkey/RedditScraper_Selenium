


static class MyExtensions
{





    public static String AsFormattedDate(this DateTime dt)
    {
        var stro = "";
        stro += dt.ToString().Split(" ")[0];
        return stro;
    }


    public static void WriteInColor(this ConsoleColor c, string s)
    {
        Console.ForegroundColor = c;
        Console.Write(s);
        Console.ForegroundColor = ConsoleColor.White;
    }

    public static void WriteLnInColor(this ConsoleColor c, string s)
    {
        Console.ForegroundColor = c;
        Console.WriteLine(s);
        Console.ForegroundColor = ConsoleColor.White;
    }

    public static void KillRunningInstances_Windows()
    {

        String[] strCmdText = {
        @"/c taskkill /im \""chrome.exe\"" -f",
        @"/c taskkill /im \""chrome.exe\"" -f",
        @"/c taskkill /im \""conhost.exe\"" -f",
        @"/c taskkill /im \""dotnet.exe\"" -f" };


        Console.ForegroundColor = ConsoleColor.Yellow;



        foreach (String s in strCmdText)
            try
            {
                System.Diagnostics.Process.Start("cmd.exe", s);
            }
            catch (Exception exc)
            {

                Console.WriteLine($"\t   - couldn't kill {s.Replace("/c taskkill /im ", "").Replace(" -f", "")} (or was not running)");

            }

        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine("\n\tKilled disruptive processes (dotnet, chromedriver, chrome, conhost)... \n");
        Console.ForegroundColor = ConsoleColor.White;
    }

    static public String CapitalizeFirst(this String s) =>
        s[0].ToString().ToUpper() + s.Substring(1);

    static public String RemoveAll(this String s, String strToRemove)
    {
        while (s.Contains(strToRemove))
            s = s.Replace(strToRemove, "");
        return s;
    }

    static public String ObfuscatePassword(this String s, String replaceChar = "*")
    {
        string sOut = "";
        sOut += s[0];
        for (int i = 1; i < s.Length; i++)
            sOut += "*";
        return sOut;
    }

    static public String UrlFromSubDomain(this String s)
    {
        if (String.IsNullOrEmpty(s))
            throw new Exception("Error in subdomain entry");
        return "https://" + s + ".rtchex.com";
    }





}