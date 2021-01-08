using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace StreamBooster
{
    class Program
    {
        static ChromeOptions Options()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments(new List<string>() { "start-maximized", "enable-automation", "--log-level=3", "--disable-browser-side-navigation", "--headless", "--disable-gpu", "--disable-infobars", "--disable-dev-shm-usage", "--no-sandbox", "mute-audio" });
            chromeOptions.AddArgument(@"--user-data-dir=C:\Users\dimad\AppData\Local\Google\Chrome Dev\User Data");
            chromeOptions.AddArgument("--profile-directory=Profile 1");
            chromeOptions.AddArguments("window-size=1800x900");
            chromeOptions.AddArgument("--start-maximized");
            return chromeOptions;
        }
        private static IWebDriver driver = new ChromeDriver(@"C:\Users\dimad\source\repos\twitch\StreamBooster", Options(), TimeSpan.FromSeconds(180));
        static void Main(string[] args)
        {
            Console.Title = "StreamBooster";
            driver.Url = "https://streambooster.ru";
            if (Login() == true || OneMore() == true) 
            {
                for(; ; )
                {
                    OpenTranslation();
                    while (CheckTranslation() == true)
                    {
                        Console.WriteLine("Translaion alive  " + DateTime.Now);
                        new System.Threading.ManualResetEvent(false).WaitOne(300000);
                    }
                }
            }
            else
            {
                if (Login() == true || OneMore() == true)
                {
                    driver.Quit();
                    var fileName = Assembly.GetExecutingAssembly().Location;
                    System.Diagnostics.Process.Start(fileName);
                    return;
                }
                else
                {
                    Console.WriteLine("!!!!!!!!!!!!!!!!!!!!CRITICAL LOGIN ERROR " + DateTime.Now + " !!!!!!!!!!!!!!!!!!!");
                    Console.ReadLine();
                }
            }
        }
        static public void OpenTranslation()
        {
            try
            {
                Console.WriteLine("Switch translation " + DateTime.Now + " ");
                driver.Url = "https://streambooster.ru";
                new System.Threading.ManualResetEvent(false).WaitOne(100000);
                driver.Url = "https://streambooster.ru/dashboard/watch";
            }
            catch
            {
                Console.WriteLine("!!!!!!!!!!!!!!!CRITICAL OPEN TRANSLATION ERROR " + DateTime.Now + " !!!!!!!!!!!!!!!!!!!!!!!");
            }
        } 
        public static bool CheckTranslation()
        {
            try
            {
                driver.FindElement(By.XPath("/html/body/div[1]/div[1]/div[2]/div[1]/div[2]/ul/li[1]/span/a")).Click();
                Console.WriteLine("Button 1 tapped");
                return true;
            }
            catch { }
            try
            {
                driver.FindElement(By.XPath("/html/body/div[2]/div[1]/div[2]/div[1]/div[2]/ul/li[1]/span/a")).Click();
                Console.WriteLine("Button 2 tapped");
                return true;
            }
            catch { }
            try
            {
                if (driver.FindElement(By.XPath("//span[@id='credits-per-minute']")).Text != "0.00") return true;
                else return false; 
            }
            catch
            {
                Console.WriteLine("!!!!!!!!!!!!!!!CRITICAL CHECK TRANSLATION ERROR " + DateTime.Now + " !!!!!!!!!!!!!!!!!!!!!!!");
                OneMore();
                return false;
            }
        }
        public static bool LoginSBNewTab()
        {
            try
            {
                var chromeOptions = new ChromeOptions();
                chromeOptions.AddArgument(@"--user-data-dir=C:\Users\dimad\AppData\Local\Google\Chrome Dev\User Data\Default\");
                chromeOptions.AddArgument("--profile-directory=Profile 1");
                chromeOptions.AddArgument("--log-level=3");
                chromeOptions.AddArgument("window-size=1400x900");
                IWebDriver driver2 = new ChromeDriver(@"C:\Users\dimad\source\repos\twitch\TwitchAdds", chromeOptions, TimeSpan.FromSeconds(180));
                try
                {
                    driver2.Url = "https://streambooster.ru/login";
                    new System.Threading.ManualResetEvent(false).WaitOne(1500);
                    driver2.FindElement(By.XPath("//a[@id='oauth-trigger']")).Click();
                    new System.Threading.ManualResetEvent(false).WaitOne(5000);
                    if (driver2.FindElement(By.XPath("/html/body/div/div[1]/div[1]/div/div[2]/div[1]/div[1]/a")).Text == "GRO08Y")
                    {
                        string newSesId = driver2.Manage().Cookies.GetCookieNamed("PHPSESSID").Value;
                        using (StreamWriter sw = new StreamWriter(@"C:\Users\dimad\source\repos\twitch\StreamBooster\sesid.txt", false, System.Text.Encoding.Default))
                        {
                            sw.WriteLine(newSesId);
                        }
                        Console.WriteLine(newSesId);
                        Console.WriteLine("Аккаунт был подключен в новой вкладке");
                        driver2.Quit();
                        driver.Quit();
                        driver = new ChromeDriver(@"C:\Users\dimad\source\repos\twitch\StreamBooster", Options(), TimeSpan.FromSeconds(180));
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("!!!!Ошибка подключения в новой вкладке №1!!!!");
                        driver2.Quit();
                        return false;
                    }
                }
                catch
                {
                    if (driver2.FindElement(By.XPath("/html/body/div/div[1]/div[1]/div/div[2]/div[1]/div[1]/a")).Text == "GRO08Y")
                    {
                        Console.WriteLine("Аккаунт был подключен");
                        driver2.Quit();
                        driver.Quit();
                        driver = new ChromeDriver(@"C:\Users\dimad\source\repos\twitch\StreamBooster", Options(), TimeSpan.FromSeconds(180));
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("!!!!Ошибка подключения в новой вкладке №2!!!!");
                        driver2.Quit();
                        return false;
                    }
                }
            }
            catch
            {
                Console.WriteLine("!!!Глобальная ошибка входа в новой вкладке!!!");
                return false;
            }
        }
        public static bool OneMore()
        {
            if (LoginSBNewTab() == true | Login() == true) return true;
            else return false;
        }
        public static bool Login()
        {
            try
            {
                string[] sesId = File.ReadAllLines(@"C:\Users\dimad\source\repos\twitch\StreamBooster\sesid.txt");
                driver.Url = "https://streambooster.ru/";
                driver.Manage().Cookies.AddCookie(new Cookie("PHPSESSID", sesId[0]));
                Console.WriteLine(sesId[0]);
                driver.Manage().Cookies.AddCookie(new Cookie("_ga", "GA1.2.403533739.1606816484"));
                driver.Manage().Cookies.AddCookie(new Cookie("_gat_gtag_UA_7160809_4", "1"));
                driver.Manage().Cookies.AddCookie(new Cookie("_gid", "GA1.2.1705362827.1608254732"));
                driver.Manage().Cookies.AddCookie(new Cookie("_ym_d", "1606816484"));
                driver.Manage().Cookies.AddCookie(new Cookie("_ym_uid", "160681648492267251"));
                driver.Manage().Cookies.GetCookieNamed("PHPSESSID");
                driver.Manage().Cookies.GetCookieNamed("_ga");
                driver.Manage().Cookies.GetCookieNamed("_gat_gtag_UA_7160809_4");
                driver.Manage().Cookies.GetCookieNamed("_gid");
                driver.Manage().Cookies.GetCookieNamed("_ym_d");
                driver.Manage().Cookies.GetCookieNamed("_ym_uid");
                driver.Url = "https://streambooster.ru/";
                new System.Threading.ManualResetEvent(false).WaitOne(1000);
                if (driver.FindElement(By.XPath("/html/body/div/div[1]/div[1]/div/div[2]/div[1]/div[1]/a")).Text == "GRO08Y")
                {
                    Console.WriteLine("~~~~~~~~~~~~~~~~Was logined " + DateTime.Now + " ~~~~~~~~~~~~~~~");
                    return true;
                }
            }
            catch { }
            try
            {
                driver.Url = "https://streambooster.ru/login";
                new System.Threading.ManualResetEvent(false).WaitOne(1500);
                driver.FindElement(By.XPath("//a[@id='oauth-trigger']")).Click();
                new System.Threading.ManualResetEvent(false).WaitOne(5000);
            }
            catch
            {
                Console.WriteLine("!!!!Error login step 1 " + DateTime.Now + " !!!!");
                return false;
            }
            try
            {
                new System.Threading.ManualResetEvent(false).WaitOne(1500);
                if (driver.FindElement(By.XPath("/html/body/div/div[1]/div[1]/div/div[2]/div[1]/div[1]/a")).Text == "GRO08Y")
                {
                    Console.WriteLine("~~~~~~~~~~~~~~~~Login succes " + DateTime.Now + " ~~~~~~~~~~~~~~~");
                    return true;
                }
                else return false;
            }
            catch
            {
                Console.WriteLine("!!!!Error login step 2! " + DateTime.Now + " !!!");
                return false;
            }
        }
    }
}
