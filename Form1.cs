namespace Tool_2M
{
    using Newtonsoft.Json.Linq;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using OtpNet;
    using RestSharp;
    using SeleniumProxyAuth;
    using SeleniumProxyAuth.Models;
    using System;
    using System.Net;
    using System.Reflection.Metadata;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Windows.Forms;
    using static System.Windows.Forms.AxHost;
    using static System.Windows.Forms.Design.AxImporter;

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
            var proxyServer = new SeleniumProxyServer();

            List<ProxyAuth> List = new List<ProxyAuth>();
            List.Add(new ProxyAuth("166.0.128.179", 39179, "user49179", "feu6vcjH3t"));
            List.Add(new ProxyAuth("157.254.222.141", 39141, "user49141", "Dyz75MvqHE"));
            List.Add(new ProxyAuth("217.20.240.138", 39138, "user49138", "8BuMGBA6x9"));
            List.Add(new ProxyAuth("157.254.222.95", 39095, "user49095", "tvRtxuVnGp"));
            List.Add(new ProxyAuth("206.206.64.113", 39113, "user49113", "QSdnBBkaMn"));

            int parallelismSize = 4;
            Parallel.For(0, dgv.Rows.Count, new ParallelOptions { MaxDegreeOfParallelism = parallelismSize }, i =>
            {
                var localPort = proxyServer.AddEndpoint(List[i]);


                DataGridViewRow dataGridViewRow = dgv.Rows[i];

                String uid = dataGridViewRow.Cells["uid"].Value.ToString();
                String pass = dataGridViewRow.Cells["pass"].Value.ToString();
                String code2fa = dataGridViewRow.Cells["haifa"].Value.ToString();


                String profileFloder = "C:\\Users\\" + Environment.UserName + "\\AppData\\Local\\Google\\Chrome\\User Data";
                String profileFloderUid = "C:\\Users\\" + Environment.UserName + "\\AppData\\Local\\Google\\Chrome\\User Data\\" + uid;
                bool existsProfile = false;
                if (Directory.Exists(profileFloderUid))
                {
                    existsProfile = true;
                }
                else
                {
                    Directory.CreateDirectory(profileFloderUid);
                }
                ChromeOptions chromeOptions = new ChromeOptions();
                ChromeDriverService chromeDriverService = ChromeDriverService.CreateDefaultService();
                chromeDriverService.HideCommandPromptWindow = true;

                chromeOptions.AddArgument("--window-size=630,515");
                chromeOptions.AddArgument("--disable-notifications");
                chromeOptions.AddArgument("--disable-images");
                chromeOptions.AddArgument("--mute-audio");
                chromeOptions.AddArgument(@"user-data-dir=" + profileFloderUid);
                Console.WriteLine(localPort.ToString());
                chromeOptions.AddArgument($"--proxy-server=127.0.0.1:{localPort}");



                ChromeDriver driver = new ChromeDriver(chromeDriverService, chromeOptions);
                driver.Navigate().GoToUrl("https://www.whatismyip.com");
                Thread.Sleep(20000);

                driver.Navigate().GoToUrl("https://mbasic.facebook.com");
                Thread.Sleep(2000);

                if (!existsProfile && driver.FindElement(By.Id("m_login_email")) == null)
                {
                    LoginPage(driver, uid, pass);

                    NhapCodeXacNhan(driver, code2fa);

                    while (driver.Url.Contains("https://mbasic.facebook.com/login/checkpoint"))
                    {
                        Thread.Sleep(2000);
                        driver.FindElement(By.Id("checkpointSubmitButton-actual-button")).Click();

                        //Thread.Sleep(2000);
                        //driver.FindElement(By.XPath("checkpointSubmitButton-actual-button")).Click();

                        //Thread.Sleep(2000);
                        //driver.FindElement(By.Id("checkpointSubmitButton-actual-button")).Click(); // review recent login

                        //Thread.Sleep(2000);
                        //driver.FindElement(By.Id("checkpointSubmitButton-actual-button")).Click();
                    }

                }
                if (driver.PageSource.Contains("/home.php"))
                {
                    string resultToken = GoToBusinessLocation(driver, dataGridViewRow);
                    if (resultToken != null)
                    {
                        dataGridViewRow.Cells[4].Value = resultToken;
                        dataGridViewRow.Cells[5].Value = "Thành Công";
                        driver.Close();
                        return;
                    }

                }

                Console.WriteLine(driver.Url);
                dataGridViewRow.Cells[5].Value = "Lỗi";
                driver.Close();



            });


        }

        private void LoginPage(ChromeDriver driver, String uid, String pass)
        {
            driver.FindElement(By.Id("m_login_email")).SendKeys(uid);
            driver.FindElement(By.XPath("/html/body/div/div/div[2]/div/table/tbody/tr/td/div[2]/div/div[2]/form/ul/li[2]/section/input")).SendKeys(pass);
            Thread.Sleep(2000);
            driver.FindElement(By.Name("login")).Click();
        }

        private String convertCode2Fa(String haiFa)
        {
            byte[] bytes = Base32Encoding.ToBytes(haiFa);
            Totp totp = new Totp(bytes);
            return totp.ComputeTotp();
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
                    //var client = new RestClient("https://2fa.live");
                    //var request = new RestRequest("https://2fa.live/tok/" + dataGridViewRow.Cells["haifa"].Value.ToString(), Method.Get);
                    //RestResponse queryResult = client.Execute(request);
                    //string? content = queryResult.Content;
                    //Console.WriteLine(content);

                    //JObject jsonObject = JObject.Parse(content);
                    //Console.WriteLine($"{jsonObject.ToString()}");
                    //string twoHaiFaValue = jsonObject["token"].ToString();
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
                    string[] cell = rows[i].Split('|');
                    if (cell.Length > 0)
                    {
                        int count = dgv.Rows.Count + 1;
                        dgv.Rows.Add(count, cell[0], cell[1], cell[2]);
                    }
                }

            }
        }

        private void dgv_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btn_request_Click(object sender, EventArgs e)
        {
            String data = "100089506756030|Bbpbslxp149bp|VQOODUJXLBFC373BVD75ONQQ4FV4GRGS";
            string[] strings = data.Split('|');
            String uid = strings[0];
            String pass = strings[1];
            String haiFA = strings[2];

            WebRequestClient webRequestClient = new WebRequestClient();
            RestResponse restResponseMbasicPage = webRequestClient.GetMbasicPage();
            var cookie = new CookieContainer();
            cookie.Add(restResponseMbasicPage.Cookies);

            string? contentPageReponse = restResponseMbasicPage.Content;
            String lsd = Regex.Match(contentPageReponse, @"name=""lsd"" value=""(.*?)""").Groups[1].ToString();
            String jaz = Regex.Match(contentPageReponse, @"name=""jazoest"" value=""(.*?)""").Groups[1].ToString();
            String m_ts = Regex.Match(contentPageReponse, @"name=""m_ts"" value=""(.*?)""").Groups[1].ToString();
            String li = Regex.Match(contentPageReponse, @"name=""li"" value=""(.*?)""").Groups[1].ToString();
            String fb_dtsg = Regex.Match(contentPageReponse, @"name=""fb_dtsg"" value=""(.*?)""").Groups[1].ToString();
            Console.WriteLine("lsd: " + lsd + "\n " + "jaz: " + jaz + "\n" + "m_ts: " + m_ts + "\n" + "li: " + li + "\n" + "fb_dtsg: " + fb_dtsg);
            //String data_post = $"lsd={lsd}&jazoest={jaz}&m_ts={m_ts}&li={li}&try_number=0&unrecognized_tries=0&email={uid}&pass={pass}&login=Log+In&bi_xrwh=0";
            RestResponse restResponseLogin = webRequestClient.PostLogin(lsd, fb_dtsg, jaz, m_ts, li, uid, pass, cookie.GetAllCookies());
            //MessageBox.Show(restResponse2.Content);
            string? contentLoginResponse = restResponseLogin.Content;
            cookie.Add(restResponseLogin.Cookies);
            if (restResponseLogin.ResponseUri.ToString().Contains("checkpoint"))
            {
                String fb_dtsg_new = Regex.Match(contentLoginResponse, @"name=""fb_dtsg"" value=""(.*?)""").Groups[1].ToString();
                String nh = Regex.Match(contentLoginResponse, @"name=""nh"" value=""(.*?)""").Groups[1].ToString();
                String jaz_new = Regex.Match(contentLoginResponse, @"name=""jazoest"" value=""(.*?)""").Groups[1].ToString();

                string Apprv_code = convertCode2Fa(haiFA);

                Console.WriteLine("fb_dtsg_new: " + fb_dtsg_new + "\n jaz: " + jaz_new + "\n nh: " + nh + " \n Apprv_code: " + Apprv_code);
                //Thread.Sleep(2000);
                RestResponse restResponse = null;

                try
                {
                    restResponse = webRequestClient.PostCheckPoint(cookie.GetAllCookies(), fb_dtsg_new, jaz_new, nh, Apprv_code);
                    string? contentPostCheckPointPage = restResponse.Content;
                    cookie.Add(restResponse.Cookies);
                    Console.WriteLine(contentPostCheckPointPage);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex.Message}");
                }

                try
                {
                    restResponse = webRequestClient.PostSaveDevice(cookie.GetAllCookies(), fb_dtsg_new, jaz_new, nh, Apprv_code);
                    cookie.Add(restResponse.Cookies);
                    string? contentPostCheckPointPage = restResponse.Content;
                    Console.WriteLine(contentPostCheckPointPage);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex.Message}");

                }

                try
                {
                    restResponse = webRequestClient.PostReviewRecentLogin(cookie.GetAllCookies(), fb_dtsg_new, jaz_new, nh, Apprv_code);
                    cookie.Add(restResponse.Cookies);
                    string? contentPostCheckPointPage = restResponse.Content;
                    Console.WriteLine(contentPostCheckPointPage);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex.Message}");

                }
                if (restResponse.ResponseUri.ToString().Contains("home.php"))
                {
                    try
                    {
                        restResponse = webRequestClient.GoToUrl("https://business.facebook.com/business_locations", cookie.GetAllCookies());
                        if (restResponse.ResponseUri.ToString().Contains("https://business.facebook.com/security/twofactor/reauth"))
                        {
                            ChromeOptions chromeOptions = new ChromeOptions();
                            ChromeDriverService chromeDriverService = ChromeDriverService.CreateDefaultService();
                            chromeDriverService.HideCommandPromptWindow = true;

                            chromeOptions.AddArgument("--window-size=630,515");
                            chromeOptions.AddArgument("--disable-notifications");
                            chromeOptions.AddArgument("--disable-images");
                            chromeOptions.AddArgument("--mute-audio");

                            //String profileFloder = "C:\\Users\\" + Environment.UserName + "\\AppData\\Local\\Google\\Chrome\\User Data";
                            //String profileFloderUid = "C:\\Users\\" + Environment.UserName + "\\AppData\\Local\\Google\\Chrome\\User Data\\" + uid;
                            //if (Directory.Exists(profileFloderUid))
                            //{
                            //    existsProfile = true;
                            //}
                            //else
                            //{
                            //    Directory.CreateDirectory(profileFloderUid);
                            //}
                            //chromeOptions.AddArgument(@"user-data-dir=" + profileFloderUid);


                            ChromeDriver driver = new ChromeDriver(chromeDriverService, chromeOptions);
                            for (int i = 0; i < cookie.GetAllCookies().Count; i++)
                            {
                                string name = cookie.GetAllCookies()[i].Name.Trim();
                                string valueCookie = cookie.GetAllCookies()[i].Value.Trim();
                                driver.Manage().Cookies.AddCookie(new OpenQA.Selenium.Cookie(name, valueCookie));

                            }
                            //driver.Manage().Cookies.AddCookie(new OpenQA.Selenium.Cookie(item., ""));

                            //OpenQA.Selenium.Cookie asdasda = 
                            //MessageBox.Show("vao page nhap 2fa cmnr ");
                            var client = new RestClient("https://2fa.live");
                            var request = new RestRequest("https://2fa.live/tok/" + haiFA, Method.Get);
                            RestResponse queryResult = client.Execute(request);
                            string? content2fa = queryResult.Content;
                            Console.WriteLine(content2fa);

                            JObject jsonObject = JObject.Parse(content2fa);
                            Console.WriteLine($"{jsonObject.ToString()}");
                            string twoHaiFaValue = jsonObject["token"].ToString();
                            driver.FindElement(By.XPath("/html/body/div[1]/div[1]/div/div[2]/div/div/div/div/div/div[2]/div[1]/div[4]/span/span/div/div[2]/div/div/div/div[1]/div[2]/div/div/input")).SendKeys(twoHaiFaValue); // nhap code 2fa
                            Thread.Sleep(3000);
                            driver.FindElement(By.XPath("/html/body/div[1]/div[1]/div/div[2]/div/div/div/div/div/div[2]/div[1]/div[4]/span/span/div/div[2]/div/div/div/div[1]/div[2]/div/div/input")).SendKeys(OpenQA.Selenium.Keys.Enter);
                            Thread.Sleep(3000);

                            string? content = driver.PageSource;
                            Console.WriteLine(content);
                            String value = content;
                            int vitriEaag = content.IndexOf("EAAG");
                            value = value.Remove(0, vitriEaag);
                            string[] eaag
                                = value.Split('\"');
                            string tokenEaag = eaag[0];
                            Console.WriteLine("tokenEaag: " + tokenEaag);
                            MessageBox.Show("tokenEaag: " + tokenEaag);
                            Console.WriteLine("thanh cong");
                        }
                        else
                        {
                            String value = restResponse.Content;
                            int vitriEaag = value.IndexOf("EAAG");
                            value = value.Remove(0, vitriEaag);
                            string[] eaag
                                = value.Split('\"');
                            string tokenEaag = eaag[0];
                            Console.WriteLine("tokenEaag: " + tokenEaag);
                            MessageBox.Show("tokenEaag: " + tokenEaag);
                            Console.WriteLine("thanh cong");
                        }



                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.StackTrace);
                        MessageBox.Show(ex.StackTrace);
                    }



                }
            }


        }
    }
}

