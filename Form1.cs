namespace Tool_2M
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using OtpNet;
    using RestSharp;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Web;
    using System.Windows.Forms;
    using Newtonsoft.Json;

    public partial class Form1 : Form
    {
        private const string M_BASIC_URL = "https://mbasic.facebook.com";
        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void login_Click(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                int numberThread = getNumberThread();

                int count = dgv.Rows.Count;
                Parallel.For(0, count, new ParallelOptions { MaxDegreeOfParallelism = numberThread < count ? numberThread : count }, i =>
                {
                    DataGridViewRow dataGridViewRow = dgv.Rows[i];
                    String uid = dataGridViewRow.Cells["uid"].Value.ToString();
                    String pass = dataGridViewRow.Cells["pass"].Value.ToString();
                    String code2fa = dataGridViewRow.Cells["haifa"].Value.ToString();
                    String profileFloderUid = "C:\\tmp\\chromeprofiles\\profile" + uid;

                    ChromeDriver driver = createChromeByUid(uid);
                    try
                    {
                        driver.Navigate().GoToUrl(M_BASIC_URL);
                        Thread.Sleep(2000);
                        if (IsElementPresent(driver, By.Id("m_login_email")))
                        {
                            LoginPage(driver, uid, pass);

                            NhapCodeXacNhan(driver, code2fa);
                            if (driver.Url.Contains(M_BASIC_URL + "/checkpoint/1501092823525282"))
                            {
                                Console.WriteLine("Checkpoint 282");
                                dataGridViewRow.Cells["status"].Value = "Die 282";
                                return;
                            }

                            while (driver.Url.Contains(M_BASIC_URL + "/login/checkpoint"))
                            {
                                Thread.Sleep(2000);
                                driver.FindElement(By.Id("checkpointSubmitButton-actual-button")).Click();
                            }

                        }
                        if (driver.PageSource.Contains("/home.php"))
                        {
                            string resultToken = GoToBusinessLocation(driver, dataGridViewRow);
                            if (resultToken != null)
                            {
                                dataGridViewRow.Cells["token"].Value = resultToken;
                                dataGridViewRow.Cells["status"].Value = "Thành Công";
                                //dataGridViewRow.Cells["cookie"].Value = getCookie(driver);
                                return;
                            }

                        }

                        Console.WriteLine(driver.Url);
                        dataGridViewRow.Cells["status"].Value = "Lỗi";

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        dataGridViewRow.Cells["status"].Value = "Lỗi";
                    }
                    finally
                    {
                        dataGridViewRow.Cells["cookie"].Value = getCookie(driver);
                        driver.Close();
                    }

                });
            }).Start();

        }

        private void LoginPage(ChromeDriver driver, String uid, String pass)
        {
            Console.WriteLine("uid: " + uid + "\n pass: " + pass);
            driver.FindElement(By.Id("m_login_email")).SendKeys(uid);
            Thread.Sleep(2000);
            driver.FindElement(By.XPath("/html/body/div/div/div[2]/div/table/tbody/tr/td/div[2]/div/div[2]/form/ul/li[2]/section/input")).SendKeys(pass);
            Thread.Sleep(2000);
            driver.FindElement(By.Name("login")).Click();
        }

        private String convertCode2Fa(String haiFa)
        {
            try
            {
                byte[] bytes = Base32Encoding.ToBytes(haiFa);
                Totp totp = new Totp(bytes);
                return totp.ComputeTotp();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "";
            }
        }

        private void NhapCodeXacNhan(ChromeDriver driver, String code2Fa)
        {
            code2Fa = convertCode2Fa(code2Fa);
            Console.WriteLine(code2Fa);
            try
            {
                Thread.Sleep(2000);
                driver.FindElement(By.Id("approvals_code")).SendKeys(code2Fa); //nhap code 2fa
                Thread.Sleep(2000);
                driver.FindElement(By.Id("checkpointSubmitButton-actual-button")).Click(); // click tiep tuc 2fa
            }
            catch (Exception)
            {

                throw;
            }
        }

        private String GoToBusinessLocation(ChromeDriver driver, DataGridViewRow dataGridViewRow)
        {
            try
            {
                driver.Navigate().GoToUrl("https://business.facebook.com/business_locations");
                Thread.Sleep(3000);
                if (driver.Url.Contains("https://business.facebook.com/security/twofactor/reauth"))
                {
                    String code2fa = convertCode2Fa(dataGridViewRow.Cells["haifa"].Value.ToString());

                    driver.FindElement(By.XPath("/html/body/div[1]/div[1]/div/div[2]/div/div/div/div/div/div[2]/div[1]/div[4]/span/span/div/div[2]/div/div/div/div[1]/div[2]/div/div/input")).SendKeys(code2fa); // nhap code 2fa
                    Thread.Sleep(3000);
                    driver.FindElement(By.XPath("/html/body/div[1]/div[1]/div/div[2]/div/div/div/div/div/div[2]/div[1]/div[4]/span/span/div/div[2]/div/div/div/div[1]/div[2]/div/div/input")).SendKeys(OpenQA.Selenium.Keys.Enter);
                    Thread.Sleep(3000);
                }
                String value = driver.PageSource;
                int vitriEaag = value.IndexOf("EAAG");
                value = value.Remove(0, vitriEaag);
                string[] strings
                    = value.Split('\"');
                string tokenEaag = strings[0];
                //MessageBox.Show(strings[0]);
                return tokenEaag;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }

        private void dgv_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == System.Windows.Forms.Keys.V)
            {
                string value = Clipboard.GetText(TextDataFormat.Text).Trim();
                string[] rows = value.Split('\n');
                for (int i = 0; i < rows.Length; i++)
                {

                    String data = (i + 1) + "|" + rows[i];
                    string[] cell = data.Split('|');
                    dgv.Rows.Add(cell);
                }

            }
        }

        private void dgv_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btn_request_Click(object sender, EventArgs e)
        {
            int numberThread = getNumberThread();
            int count = dgv.Rows.Count;
            Parallel.For(0, count, new ParallelOptions { MaxDegreeOfParallelism = numberThread < count ? numberThread : count }, i =>
            {
                DataGridViewRow dataGridViewRow = dgv.Rows[i];
                String uid = dataGridViewRow.Cells["uid"].Value.ToString();
                String pass = dataGridViewRow.Cells["pass"].Value.ToString();
                String haiFA = "";
                try
                {
                    haiFA = dataGridViewRow.Cells["haifa"].Value.ToString();
                }
                catch (Exception)
                {
                    Console.WriteLine("chua co thong tin 2fa");
                }
                dataGridViewRow.Cells["status"].Value = "Dang Login ... ";

                WebRequestClient webRequestClient = new WebRequestClient();
                RestResponse restResponseMbasicPage = webRequestClient.GetMbasicPage();
                var cookie = new System.Net.CookieContainer();
                cookie.Add(restResponseMbasicPage.Cookies);

                string? contentPageReponse = restResponseMbasicPage.Content;
                String lsd = Regex.Match(contentPageReponse, @"name=""lsd"" value=""(.*?)""").Groups[1].ToString();
                String jaz = Regex.Match(contentPageReponse, @"name=""jazoest"" value=""(.*?)""").Groups[1].ToString();
                String m_ts = Regex.Match(contentPageReponse, @"name=""m_ts"" value=""(.*?)""").Groups[1].ToString();
                String li = Regex.Match(contentPageReponse, @"name=""li"" value=""(.*?)""").Groups[1].ToString();
                String fb_dtsg = Regex.Match(contentPageReponse, @"name=""fb_dtsg"" value=""(.*?)""").Groups[1].ToString();
                Console.WriteLine("lsd: " + lsd + "\n " + "jaz: " + jaz + "\n" + "m_ts: " + m_ts + "\n" + "li: " + li + "\n" + "fb_dtsg: " + fb_dtsg);
                //String data_post = $"lsd={lsd}&jazoest={jaz}&m_ts={m_ts}&li={li}&try_number=0&unrecognized_tries=0&email={uid}&pass={pass}&login=Log+In&bi_xrwh=0";
                RestResponse restResponse = webRequestClient.PostLogin(lsd, fb_dtsg, jaz, m_ts, li, uid, pass, cookie.GetAllCookies());
                //MessageBox.Show(restResponse2.Content);
                string? contentLoginResponse = restResponse.Content;
                cookie.Add(restResponse.Cookies);
                if (restResponse.ResponseUri.ToString().Contains("checkpoint"))
                {
                    if (restResponse.ResponseUri.ToString().Contains("282"))
                    {
                        dataGridViewRow.Cells["status"].Value = "DIE 282";
                        return;
                    }
                    try
                    {
                        fb_dtsg = Regex.Match(contentLoginResponse, @"name=""fb_dtsg"" value=""(.*?)""").Groups[1].ToString();
                        String nh = Regex.Match(contentLoginResponse, @"name=""nh"" value=""(.*?)""").Groups[1].ToString();
                        jaz = Regex.Match(contentLoginResponse, @"name=""jazoest"" value=""(.*?)""").Groups[1].ToString();

                        string Apprv_code = convertCode2Fa(haiFA);

                        Console.WriteLine("fb_dtsg_new: " + fb_dtsg + "\n jaz: " + jaz + "\n nh: " + nh + " \n Apprv_code: " + Apprv_code);
                        //Thread.Sleep(2000);
                        //RestResponse restResponse = null;

                        try
                        {
                            Console.WriteLine("Apprv_code check point");

                            restResponse = webRequestClient.PostCheckPoint(cookie.GetAllCookies(), fb_dtsg, jaz, nh, Apprv_code);
                            string? contentPostCheckPointPage = restResponse.Content;
                            cookie.Add(restResponse.Cookies);
                            //Console.WriteLine(contentPostCheckPointPage);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"{ex.Message}");
                        }

                        try
                        {
                            Console.WriteLine("PostSaveDevice .............");

                            restResponse = webRequestClient.PostSaveDevice(cookie.GetAllCookies(), fb_dtsg, jaz, nh, Apprv_code);
                            cookie.Add(restResponse.Cookies);
                            string? contentPostCheckPointPage = restResponse.Content;
                            //Console.WriteLine(contentPostCheckPointPage);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"{ex.Message}");

                        }
                        if (restResponse.ResponseUri.ToString().Contains("checkpoint"))
                        {

                            try
                            {
                                Console.WriteLine("PostReviewRecentLogin .............");

                                restResponse = webRequestClient.PostReviewRecentLogin(cookie.GetAllCookies(), fb_dtsg, jaz, nh, Apprv_code);
                                cookie.Add(restResponse.Cookies);
                                Console.WriteLine(restResponse.ResponseUri);

                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"{ex.Message}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Loix check point");
                        dataGridViewRow.Cells["status"].Value = "Loi Check Point ";
                        return;
                    }

                }
                if (restResponse.ResponseUri.ToString().Contains("https://mbasic.facebook.com/login/save-device"))
                {
                    Console.WriteLine("save-device .............");

                    string? content = restResponse.Content;
                    string match = Regex.Match(content, @"/login/save-device/cancel/?flow=interstitial_nux&amp;nux_source=regular_login&amp;paipv=0&amp;eav=(.*?)""").Groups[1].ToString();
                    restResponse = webRequestClient.GoToUrl($"{M_BASIC_URL}/login/save-device/cancel/?flow=interstitial_nux&amp;nux_source=regular_login&amp;paipv=0&amp;eav={match}", cookie.GetAllCookies());
                    Console.WriteLine("skip  .............");
                    Console.WriteLine(restResponse.ResponseUri);


                    content = restResponse.Content;
                    match = Regex.Match(content, @"/a/nux/wizard/nav.php?step=search&amp;skip&amp;eav=(.*?)&amp;paipv=0").Groups[1].ToString();
                    restResponse = webRequestClient.GoToUrl($"{M_BASIC_URL}/a/nux/wizard/nav.php?step=search&amp;skip&amp;eav={match}&amp;paipv=0", cookie.GetAllCookies());
                    Console.WriteLine("skip2  .............");
                    //content = restResponse.Content;
                    Console.WriteLine(restResponse.ResponseUri);

                }
                if (restResponse.ResponseUri.ToString().Contains("home.php"))
                {
                    Console.WriteLine("home  .............");

                    string? contentHomePage = restResponse.Content;
                    fb_dtsg = HttpUtility.UrlEncode(Regex.Match(contentHomePage, @"name=""fb_dtsg"" value=""(.*?)""").Groups[1].ToString());
                    string nh = HttpUtility.UrlEncode(Regex.Match(contentHomePage, @"name=""nh"" value=""(.*?)""").Groups[1].ToString());
                    jaz = HttpUtility.UrlEncode(Regex.Match(contentHomePage, @"name=""jazoest"" value=""(.*?)""").Groups[1].ToString());

                    try
                    {
                        Console.WriteLine("business_locations  .............");

                        restResponse = webRequestClient.GoToUrl("https://business.facebook.com/business_locations", cookie.GetAllCookies());
                        if (restResponse.ResponseUri.ToString().Contains("https://business.facebook.com/security/twofactor/reauth"))
                        {
                            Console.WriteLine("twofactor/reauth/enter  .............");

                            string code2Fa = webRequestClient.get2FaApi(haiFA);
                            String body = $"approvals_code={code2Fa}&save_device=false&__user={uid}&__a=1&fb_dtsg={fb_dtsg}&jazoest={jaz}";
                            RestResponse respAuth2Fa = webRequestClient.Post(cookie.GetAllCookies(), "https://business.facebook.com/security/twofactor/reauth/enter", body);
                            cookie.Add(respAuth2Fa.Cookies);

                            string? content = respAuth2Fa.Content;
                            Console.WriteLine(content);
                            Console.WriteLine("business_locations  .............");

                            restResponse = webRequestClient.GoToUrl("https://business.facebook.com/business_locations", cookie.GetAllCookies());

                        }

                        if (restResponse.ResponseUri.ToString().Contains("business_locations"))
                        {

                            Console.WriteLine("uri = " + restResponse.ResponseUri);
                            String? value = restResponse.Content;
                            int vitriEaag = value.IndexOf("EAAG");
                            value = value.Remove(0, vitriEaag);
                            string[] eaag
                                = value.Split('\"');
                            string tokenEaag = eaag[0];
                            Console.WriteLine("tokenEaag: " + tokenEaag);
                            dataGridViewRow.Cells["token"].Value = tokenEaag;
                            dataGridViewRow.Cells["cookie"].Value = saveCookie(cookie.GetAllCookies());
                            dataGridViewRow.Cells["status"].Value = "Thanh Cong";
                        }
                        else
                        {
                            //dataGridViewRow.Cells["cookie"].Value = saveCookie(cookie.GetAllCookies());
                            dataGridViewRow.Cells["status"].Value = "That Bai";

                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.StackTrace);
                        dataGridViewRow.Cells["status"].Value = "That bai";
                    }



                }

                dataGridViewRow.Cells["status"].Value = "that bai";
                return;

            });



        }
        private bool IsElementPresent(ChromeDriver chromeDriver, By by)
        {
            try
            {
                chromeDriver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        private String getCookie(ChromeDriver driver)
        {
            IReadOnlyCollection<OpenQA.Selenium.Cookie> allCookies = driver.Manage().Cookies.AllCookies;
            String strCookie = "";
            foreach (OpenQA.Selenium.Cookie cookie in allCookies)
            {
                strCookie += cookie.Name + "=" + cookie.Value + ";";
            }
            return strCookie;
        }

        private String saveCookie(CookieCollection cookieCollection)
        {
            String cookie = "";
            for (int i = 0; i < cookieCollection.Count; i++)
            {
                cookie += cookieCollection[i].Name + "=" + cookieCollection[i].Value + ";";

            }
            return cookie;

        }

        private void btn_reg_page_Click(object sender, EventArgs e)
        {
            string pageName = txt_page_name.Text;
            if (string.IsNullOrEmpty(pageName))
            {
                MessageBox.Show("chua nhap ten page");
                return;
            }
            try
            {
                int numberThread = getNumberThread();

                int count = dgv.Rows.Count;
                Parallel.For(0, count, new ParallelOptions { MaxDegreeOfParallelism = 2 }, i =>
                {
                    DataGridViewRow dataGridViewRow = dgv.Rows[i];
                    dataGridViewRow.Cells["status"].Value = "Create page ...";
                    string cookie;
                    string token;
                    string uid = dataGridViewRow.Cells["uid"].Value.ToString();
                    //try
                    //{
                    //    //cookie = dataGridViewRow.Cells["cookie"].Value.ToString();
                    //    //token = dataGridViewRow.Cells["token"].Value.ToString();
                    //    //uid = 

                    //}
                    //catch (Exception)
                    //{
                    //    dataGridViewRow.Cells["status"].Value = "uid|cookie|token not found";
                    //    //return;
                    //}

                    ChromeDriver driver = createChromeByUid(uid);
                    try
                    {
                        //Thread.Sleep(2000);
                        driver.Navigate().GoToUrl("https://www.facebook.com/pages/creation");
                        Thread.Sleep(4000);
                        //driver.FindElement(By.XPath("/html/body/div[1]/div[1]/div[1]/div/div[5]/div/div/div[3]/div[2]/div[1]/div/div[3]/div[1]/div[2]/div/div/div/div[1]/div/label/div/div/input")).SendKeys(pageName);
                        driver.FindElement(By.XPath("/html/body/div[1]/div/div[1]/div/div[5]/div/div/div[3]/div/div/div[1]/div[1]/div[1]/div/div[2]/div[1]/div[2]/div/div/div/div[1]/div/label/div/div/input")).SendKeys(pageName);

                        Thread.Sleep(2000);
                        driver.FindElement(By.XPath("/html/body/div[1]/div/div[1]/div/div[5]/div/div/div[3]/div/div/div[1]/div[1]/div[1]/div/div[2]/div[1]/div[2]/div/div/div/div[3]/div/div/div/div/label/div/div/div")).Click();// click input
                        Thread.Sleep(2000);
                        driver.FindElement(By.XPath("/html/body/div[1]/div/div[1]/div/div[5]/div/div/div[3]/div/div/div[1]/div[1]/div[1]/div/div[2]/div[1]/div[2]/div/div/div/div[3]/div/div/div/div/label/div/div/div/input")).SendKeys("a");
                        Thread.Sleep(2000);

                        driver.FindElement(By.XPath("/html/body/div[1]/div/div[1]/div/div[5]/div/div/div[3]/div/div/div[2]/div/div/div[1]/div[1]/div/div[1]/div/ul/li[1]/div/div[1]/div/div/div/div/span")).Click();
                        //driver.FindElement(By.XPath("/html/body/div[1]/div[1]/div[1]/div/div[5]/div/div/div[3]/div[2]/div[1]/div/div[3]/div[1]/div[2]/div/div/div/div[3]/div/div/div/div/label/div/div/div/input")).SendKeys(Keys.Enter);
                        Thread.Sleep(2000);

                        driver.FindElement(By.XPath("/html/body/div[1]/div/div[1]/div/div[5]/div/div/div[3]/div/div/div[1]/div[1]/div[1]/div/div[3]/div[1]/div/div/div/div[1]/div/span/span")).Click(); // di tiep
                        Thread.Sleep(7000);
                        driver.FindElement(By.XPath("/html/body/div[1]/div/div[1]/div/div[5]/div/div/div[3]/div/div/div[1]/div[1]/div[1]/div/div[3]/div[2]/div[2]/div/div")).Click(); // Finish setting up your Page
                        Thread.Sleep(3000);
                        driver.FindElement(By.XPath("/html/body/div[1]/div/div[1]/div/div[5]/div/div/div[3]/div/div/div[1]/div[1]/div[1]/div/div[3]/div[2]/div[2]/div/div")).Click(); // customer your page
                        Thread.Sleep(3000);
                        driver.FindElement(By.XPath("/html/body/div[1]/div/div[1]/div/div[5]/div/div/div[3]/div/div/div[1]/div[1]/div[1]/div/div[3]/div[2]/div[2]/div/div")).Click(); // Connect WhatsApp to your Page
                        Thread.Sleep(3000);
                        driver.FindElement(By.XPath("/html/body/div[1]/div/div[1]/div/div[5]/div/div/div[3]/div/div/div[1]/div[1]/div[1]/div/div[3]/div[2]/div[2]/div/div")).Click(); // Build your Page audience
                        Thread.Sleep(3000);
                        driver.FindElement(By.XPath("/html/body/div[1]/div/div[1]/div/div[5]/div/div/div[3]/div/div/div[1]/div[1]/div[1]/div/div[3]/div[2]/div[2]/div/div")).Click(); // done
                        Thread.Sleep(3000);
                        string url = driver.Url;
                        if (url.Contains("https://www.facebook.com/profile.php?id="))
                            dataGridViewRow.Cells["status"].Value = "Tao Page Thanh Cong - " + url.Replace("https://www.facebook.com/profile.php?id=", "");
                        else
                            dataGridViewRow.Cells["status"].Value = "that bai";


                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        dataGridViewRow.Cells["status"].Value = "that bai";
                    }
                    finally
                    {
                        driver.Close();
                    }

                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                MessageBox.Show(ex.ToString());
            }
        }

        private ChromeDriver createChromeByUid(String uid)
        {
            ChromeOptions chromeOptions = new ChromeOptions();

            String profileFloderUid = "C:\\tmp\\chromeprofiles\\profile" + uid;
            if (!Directory.Exists(profileFloderUid))
            {
                Directory.CreateDirectory(profileFloderUid);
            }

            ChromeDriverService chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = true;

            chromeOptions.AddArgument("--window-size=630,515");
            chromeOptions.AddArgument("--disable-notifications");
            chromeOptions.AddArgument("--disable-images");
            chromeOptions.AddArgument("--mute-audio");
            chromeOptions.AddArgument(@"user-data-dir=" + profileFloderUid);

            ChromeDriver driver = new ChromeDriver(chromeDriverService, chromeOptions);
            return driver;
        }

        private int getNumberThread()
        {
            return Convert.ToInt32(nb_number_thread.Value);
        }

        private void btn_reg_page_request_Click(object sender, EventArgs e)
        {
            string pageName = txt_page_name.Text;
            if (string.IsNullOrEmpty(pageName))
            {
                MessageBox.Show("chua nhap ten page");
                return;
            }
            new Thread(() =>
            {
                int numberThread = getNumberThread();

                int count = dgv.Rows.Count;
                Parallel.For(0, count, new ParallelOptions { MaxDegreeOfParallelism = numberThread < count ? numberThread : count }, j =>
                {
                    DataGridViewRow dataGridViewRow = dgv.Rows[j];
                    DataGridViewCell cellStatus = dataGridViewRow.Cells["status"];

                    try
                    {
                        String cookie;
                        String uid;

                        try
                        {
                            cookie = dataGridViewRow.Cells["cookie"].Value.ToString().Replace(" ", "");
                            uid = dataGridViewRow.Cells["uid"].Value.ToString();

                        }
                        catch (Exception)
                        {

                            cellStatus.Value = "cookie not found";
                            return;
                        }
                        cellStatus.Value = "Dang Tao Page...";

                        WebRequestClient webRequestClient = new WebRequestClient();
                        CookieCollection cookieCollection = new CookieCollection();
                        string[] strCookie
                            = cookie.Split(';');
                        for (int i = 0; i < strCookie.Length; i++)
                        {
                            string[] cookieElement
                                = strCookie[i].Split('=');
                            if (cookieElement.Length == 2)
                                cookieCollection.Add(new System.Net.Cookie(cookieElement[0], cookieElement[1]));

                        }
                        RestResponse restResponse
                            = webRequestClient.GoToUrl(M_BASIC_URL, cookieCollection);
                        string? contentPageReponse = restResponse.Content;
                        Console.WriteLine(contentPageReponse);
                        Console.WriteLine("Go to Mabsaic success ============ \n \n");

                        String lsd = Regex.Match(contentPageReponse, @"name=""lsd"" value=""(.*?)""").Groups[1].ToString();
                        String jaz = HttpUtility.UrlEncode(Regex.Match(contentPageReponse, @"name=""jazoest"" value=""(.*?)""").Groups[1].ToString());
                        String m_ts = Regex.Match(contentPageReponse, @"name=""m_ts"" value=""(.*?)""").Groups[1].ToString();
                        String li = Regex.Match(contentPageReponse, @"name=""li"" value=""(.*?)""").Groups[1].ToString();
                        String fb_dtsg = HttpUtility.UrlEncode(Regex.Match(contentPageReponse, @"name=""fb_dtsg"" value=""(.*?)""").Groups[1].ToString());
                        Console.WriteLine("lsd: " + lsd + "\n " + "jaz: " + jaz + "\n" + "m_ts: " + m_ts + "\n" + "li: " + li + "\n" + "fb_dtsg: " + fb_dtsg);
                        Console.WriteLine(restResponse.ResponseUri);

                        RestResponse restResponse2
                            = webRequestClient.GoToUrl("https://www.facebook.com/pages/creation", cookieCollection);
                        string? resPonse = restResponse2.Content;

                        Console.WriteLine(restResponse2.ResponseUri);
                        Console.WriteLine("Go to creation success ============ \n \n");

                        string variables = "{\"input\":{\"bio\":\"\",\"categories\":[\"123377808095874\"],\"creation_source\":\"comet\",\"name\":\"" + pageName + "\",\"page_referrer\":\"launch_point\",\"actor_id\":\"" + uid + "\",\"client_mutation_id\":\"1\"}}";
                        String body = $"av={uid}&__user={uid}=1&fb_dtsg={fb_dtsg}&jazoest={jaz}&fb_api_caller_class=RelayModern&fb_api_req_friendly_name=AdditionalProfilePlusCreationMutation&variables={variables}&server_timestamps=true&doc_id=5903223909690825";
                        Console.WriteLine(body);

                        RestResponse restCreatePage1
                            = webRequestClient.Post(cookieCollection, "https://www.facebook.com/api/graphql", body);
                        string? resStep1 = restCreatePage1.Content;
                        Console.WriteLine("Go to graphql 1 success ============ \n \n");

                        Console.WriteLine(resStep1);

                        dynamic jsonObject = JsonConvert.DeserializeObject<dynamic>(resStep1);

                        String profile_id = jsonObject.data.additional_profile_plus_create.additional_profile.id.ToString();
                        String page_id = jsonObject.data.additional_profile_plus_create.page.id.ToString();

                        Console.WriteLine(profile_id + " " + page_id);

                        Console.WriteLine(restCreatePage1.ResponseUri);
                        variables = "{\"input\":{\"additional_profile_plus_id\":\"" + profile_id + "\",\"creation_source\":\"comet\",\"cpn_setting\":true,\"email_notif_setting\":false,\"actor_id\":\"" + uid + "\",\"client_mutation_id\":\"1\"}}";
                        String body2 = $"av={profile_id}&__user={uid}=1&fb_dtsg={fb_dtsg}&jazoest={jaz}&fb_api_caller_class=RelayModern&fb_api_req_friendly_name=AdditionalProfilePlusCreationMutation&variables={variables}&server_timestamps=true&doc_id=6470849629597825";
                        Console.WriteLine(body2);

                        RestResponse restCreatePage2
                            = webRequestClient.Post(cookieCollection, "https://www.facebook.com/api/graphql", body2);
                        string? resStep2 = restCreatePage2.Content;
                        Console.WriteLine("Go to graphql 2 success ============ \n \n");

                        Console.WriteLine(restCreatePage2.ResponseUri);
                        Console.WriteLine(resStep2);
                        if (resStep2.Contains(profile_id))
                            cellStatus.Value = profile_id + "thanh cong";
                        else
                            cellStatus.Value = "tao page that bai";

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        cellStatus.Value = "tao page that bai";
                    }
                });

            }).Start();
        }

        private CookieCollection GetCookieCollectionFromStr(String cookie)
        {

            CookieCollection cookieCollection = new CookieCollection();
            string[] strCookie
                = cookie.Split(';');
            for (int i = 0; i < strCookie.Length; i++)
            {
                string[] cookieElement
                    = strCookie[i].Split('=');
                if (cookieElement.Length == 2)
                    cookieCollection.Add(new System.Net.Cookie(cookieElement[0], cookieElement[1]));
            }
            return cookieCollection;
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            if (txt_file_name.Text == null || txt_file_name.Text.Length == 0)
            {
                MessageBox.Show("ban chua nhap ten file");
                return;
            }
            string fileName = txt_file_name.Text + ".txt";
            String content = "";

            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                int count = dgv.Rows[i].Cells.Count;
                for (int j = 1; j < count - 1; j++)
                {
                    try
                    {
                        content += dgv.Rows[i].Cells[j].Value.ToString();

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    finally
                    {
                        content += "|";
                    }
                }
                content = content + "\n";

            }

            try
            {
                File.WriteAllText(fileName, content);
                Console.WriteLine("Text written to file successfully.");
                MessageBox.Show("Luu file thanh cong " + fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while writing to the file: " + ex.Message);
                MessageBox.Show("Luu file that bai ");

            }
        }

        private void btn_add_camp_Click(object sender, EventArgs e)
        {
            string token = "EAAGNO4a7r2wBAOznsZAi00OkZB0ompYKW5BkLBLl32iZAgeW3ZCycYufOjYhTDbsIFyOISOQq4ZByXDcr1P7gsH0RdqZCxZBadJ9xVucSbqvGNmz6F9YFz5EH3O8uN2Tsa4j5r2JpnLGy3YFDlCcZAlva17YFqXNqwXwxp9jIfQkOOWsEZBzFpCN59VCRi5BHS7stcdY0yjkM8QZDZD";
            string cookie = "presence=EDvF3EtimeF1676545024EuserFA21B89804044674A2EstateFDutF0CEchF_7bCC;wd=616x383;m_page_voice=100089804044674;fr=0tg3iSY8exN6WrR72.AWVasAUpQOZrLFPn5ms2Ewxo60M.Bj7gvr.BB.AAA.0.0.Bj7gvr.AWVWFt3qRlw;xs=50%3Afkb7AhO9d4rOMg%3A2%3A1676545004%3A-1%3A-1;c_user=100089804044674;locale=en_GB;sb=BPrtYzZttCBTXpNYyqDZQXAa;dpr=1.25;datr=BPrtY5PKYzmwpbbXucf-xtC3;";
            var client = new RestClient(M_BASIC_URL);
            var request = new RestRequest(M_BASIC_URL, Method.Get);
            client.AddDefaultHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36");
            request.AddHeader("cookie", cookie);
            RestResponse response = client.Execute(request);
            Console.WriteLine("content : " + response.Content);


            client = new RestClient($"https://graph.facebook.com/v16.0/me?access_token={token}&__cppo=1&debug=all&fields=adaccounts%7Baccount_id%7D&format=json&method=get&pretty=0&suppress_http_code=1&transport=cors");
            request = new RestRequest($"https://graph.facebook.com/v16.0/me?access_token={token}&__cppo=1&debug=all&fields=adaccounts%7Baccount_id%7D&format=json&method=get&pretty=0&suppress_http_code=1&transport=cors", Method.Get);
            request.Method = Method.Get;
            request.AddHeader("authority", "graph.facebook.com");
            request.AddHeader("accept", "*/*");
            request.AddHeader("accept-language", "en");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddHeader("cookie", cookie);
            request.AddHeader("referer", "https://developers.facebook.com/");
            //request.AddHeader("sec-ch-ua", "\"Chromium\";v=\"110\", \"Not A(Brand\";v=\"24\", \"Google Chrome\";v=\"110\"");
            //request.AddHeader("sec-ch-ua-mobile", "?0");
            //request.AddHeader("sec-ch-ua-platform", "\"Windows\"");
            //request.AddHeader("sec-fetch-dest", "empty");
            //request.AddHeader("sec-fetch-mode", "cors");
            //request.AddHeader("sec-fetch-site", "same-site");
            client.AddDefaultHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36");
            response = client.Execute(request);
            string content = response.Content;
            Console.WriteLine("content : " + content);

            dynamic json_content_act = JsonConvert.DeserializeObject<dynamic>(content);

            String act_id = json_content_act.adaccounts.data[0].id.ToString();
            Console.WriteLine("act_id : " + act_id);

            client = new RestClient($"https://graph.facebook.com/v16.0/{act_id}/campaigns?access_token={token}&__cppo=1");
            request = new RestRequest($"https://graph.facebook.com/v16.0/{act_id}/campaigns?access_token={token}&__cppo=1", Method.Post);
            request.AddHeader("accept", "*/*");
            request.AddHeader("accept-language", "en");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddHeader("cookie", cookie);
            client.AddDefaultHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36");
            request.AddParameter("debug", "all");
            request.AddParameter("format", "json");
            request.AddParameter("method", "post");
            request.AddParameter("name", HttpUtility.UrlDecode("Tet lần 1 nè ahu hí bí tí"));
            request.AddParameter("objective", "LINK_CLICKS");
            request.AddParameter("pretty", "0");
            request.AddParameter("special_ad_categories", "NONE");
            request.AddParameter("status", "ACTIVE");
            request.AddParameter("suppress_http_code", "1");
            request.AddParameter("daily_budget", "100000");
            request.AddParameter("transport", "cors");
            response = client.Execute(request);
            string content_camp = response.Content;
            dynamic json_content_camp = JsonConvert.DeserializeObject<dynamic>(content_camp);

            String id = json_content_camp.id.ToString();
            Console.WriteLine("id camp : " + json_content_camp);


            client = new RestClient($"https://graph.facebook.com/v16.0/{act_id}/adsets?access_token={token}&__cppo=1");
            request = new RestRequest($"https://graph.facebook.com/v16.0/{act_id}/adsets?access_token={token}&__cppo=1", Method.Post);
            request.AddHeader("authority", "graph.facebook.com");
            request.AddHeader("accept", "*/*");
            request.AddHeader("accept-language", "en");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddHeader("cookie", cookie);
            request.AddHeader("origin", "https://developers.facebook.com");
            request.AddHeader("referer", "https://developers.facebook.com/");
            client.AddDefaultHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36");
            request.AddParameter("bid_amount", "100");
            request.AddParameter("billing_event", "IMPRESSIONS");
            request.AddParameter("campaign_id", id);
            request.AddParameter("debug", "all");
            request.AddParameter("format", "json");
            request.AddParameter("method", "post");
            request.AddParameter("name", "My Ad Set233");
            request.AddParameter("optimization_goal", "LINK_CLICKS");
            request.AddParameter("pretty", "0");
            request.AddParameter("status", "PAUSED");
            request.AddParameter("suppress_http_code", "1");
            request.AddParameter("targeting", "{\"geo_locations\":{\"countries\":[\"US\"]},\"age_min\":18,\"age_max\":65,\"genders\":[1]}");
            request.AddParameter("transport", "cors");
            response = client.Execute(request);
            content = response.Content;
            Console.WriteLine("content : " + content);

            dynamic jsonObject = JsonConvert.DeserializeObject<dynamic>(content);

            String id_ad_set = jsonObject.id.ToString();
            Console.WriteLine("id set : " + id_ad_set);



            client = new RestClient($"https://graph.facebook.com/v16.0/{act_id}/ads?access_token={token}&__cppo=1");
            request = new RestRequest($"https://graph.facebook.com/v16.0/{act_id}/ads?access_token={token}&__cppo=1", Method.Post);
            request.AddHeader("authority", "graph.facebook.com");
            request.AddHeader("accept", "*/*");
            request.AddHeader("accept-language", "en");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddHeader("cookie", cookie);
            request.AddHeader("origin", "https://developers.facebook.com");
            request.AddHeader("referer", "https://developers.facebook.com/");
            client.AddDefaultHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36");
            request.AddParameter("adset_id", id_ad_set);
            request.AddParameter("creative", "{\"object_story_spec\":{\"page_id\":\"115510804792515\",\"link_data\":{\"link\":\"http://www.example.com\",\"message\":\"Check out this awesome website!\"}}}");
            request.AddParameter("debug", "all");
            request.AddParameter("format", "json");
            request.AddParameter("method", "post");
            request.AddParameter("name", "My Ad test c#");
            request.AddParameter("pretty", "0");
            request.AddParameter("status", "PAUSED");
            request.AddParameter("suppress_http_code", "1");
            request.AddParameter("transport", "cors");
            response = client.Execute(request);
            Console.WriteLine(response.Content);

        }
    }
}

