namespace Tool_2M
{
    using Newtonsoft.Json.Linq;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using OtpNet;
    using RestSharp;
    using System;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;
    using static System.Net.Mime.MediaTypeNames;
    using System.Text;
    using RestSharp;

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

        private async void login_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                DataGridViewRow dataGridViewRow = dgv.Rows[i];
                String uid = dataGridViewRow.Cells["uid"].Value.ToString();
                String pass = dataGridViewRow.Cells["pass"].Value.ToString();
                String haiFA = dataGridViewRow.Cells["haifa"].Value.ToString();

                ChromeOptions chromeOptions = new ChromeOptions();
                ChromeDriverService chromeDriverService = ChromeDriverService.CreateDefaultService();
                chromeDriverService.HideCommandPromptWindow = true;

                chromeOptions.AddArgument("--window-size=630,515");
                chromeOptions.AddArgument("--disable-notifications");
                chromeOptions.AddArgument("--disable-images");
                chromeOptions.AddArgument("--mute-audio");

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
                chromeOptions.AddArgument(@"user-data-dir=" + profileFloderUid);


                ChromeDriver driver = new ChromeDriver(chromeDriverService, chromeOptions);
                driver.Navigate().GoToUrl("https://mbasic.facebook.com");
                Thread.Sleep(2000);

                if (!existsProfile)
                {
                    driver.FindElement(By.XPath("/html/body/div/div/div[2]/div/table/tbody/tr/td/div[2]/div/div[2]/form/ul/li[1]/input")).SendKeys(uid);
                    driver.FindElement(By.XPath("/html/body/div/div/div[2]/div/table/tbody/tr/td/div[2]/div/div[2]/form/ul/li[2]/section/input")).SendKeys(pass);
                    Thread.Sleep(3000);
                    driver.FindElement(By.XPath("/html/body/div/div/div[2]/div/table/tbody/tr/td/div[2]/div/div[2]/form/ul/li[3]/input")).Click();
                    Thread.Sleep(3000);
                    byte[] bytes = Base32Encoding.ToBytes(haiFA);
                    Totp totp = new Totp(bytes);
                    String code2Fa = totp.ComputeTotp();
                    //MessageBox.Show(code2Fa);
                    driver.FindElement(By.XPath("/html/body/div/div/div[2]/div/form/div[1]/article/section/section[2]/div[2]/div/input")).SendKeys(code2Fa); //nhap code 2fa

                    Thread.Sleep(3000);
                    driver.FindElement(By.XPath("/html/body/div/div/div[2]/div/form/div[1]/article/div[1]/table/tbody/tr/td/input")).Click(); // click tiep tuc 2fa

                    Thread.Sleep(3000);
                    driver.FindElement(By.XPath("/html/body/div/div/div[2]/div/form/div[1]/article/div[1]/table/tbody/tr/td/input")).Click();
                }

                GoToBusinessLocation(driver, dataGridViewRow);
            }

        }

        private async void GoToBusinessLocation(ChromeDriver driver, DataGridViewRow dataGridViewRow)
        {
            driver.Navigate().GoToUrl("https://business.facebook.com/business_locations");
            Thread.Sleep(3000);
            if (driver.Url.Contains("https://business.facebook.com/security/twofactor/reauth"))
            {
                HttpClient client = new HttpClient();
                var json = await client.GetStringAsync("https://2fa.live/tok/" + dataGridViewRow.Cells["haifa"].Value.ToString());
                JObject jsonObject = JObject.Parse(json);
                string twoHaiFaValue = jsonObject["token"].ToString();
                driver.FindElement(By.XPath("/html/body/div[1]/div[1]/div/div[2]/div/div/div/div/div/div[2]/div[1]/div[4]/span/span/div/div[2]/div/div/div/div[1]/div[2]/div/div/input")).SendKeys(twoHaiFaValue);
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
            dataGridViewRow.Cells["token"].Value = tokenEaag;
            dataGridViewRow.Cells["status"].Value = "Đã Lấy được token EAAG";
        }

        private void dgv_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == System.Windows.Forms.Keys.V)
            {
                string value = Clipboard.GetText(TextDataFormat.Text);
                string[] rows = value.Split('\n');
                for (int i = 0; i < rows.Length; i++)
                {
                    string[] cell = rows[i].Split('|');
                    dgv.Rows.Add(i + 1, cell[0], cell[1], cell[2]);
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

            //HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://mbasic.facebook.com");

            //httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.131 Safari/537.36";
            //httpWebRequest.ContentType = "application/x-www-form-urlencode";
            //httpWebRequest.KeepAlive = true;
            //httpWebRequest.AllowAutoRedirect = true;
            //httpWebRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7";
            //httpWebRequest.Headers.Add("accept-language", "en-VN;q=0.9");
            //httpWebRequest.CookieContainer = new CookieContainer();
            //String contentPage = null;
            //HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
            //Stream stream = response.GetResponseStream();
            //StreamReader reader = new StreamReader(stream);
            //contentPage = reader.ReadToEnd();
            //var cookies = new CookieContainer();
            //CookieCollection cookies1
            //    = response.Cookies;
            //for (int i = 0; i < cookies1.Count; i++)
            //{
            //    MessageBox.Show(cookies1[i].ToString());

            //}
            //cookies.Add(response.Cookies);
            WebRequestClient webRequestClient = new WebRequestClient();
            RestResponse restResponse = webRequestClient.GetMbasicPage();
            string? content = restResponse.Content;
            String lsd = Regex.Match(content, @"name=""lsd"" value=""(.*?)""").Groups[1].ToString();
            String jaz = Regex.Match(content, @"name=""jazoest"" value=""(.*?)""").Groups[1].ToString();
            String m_ts = Regex.Match(content, @"name=""m_ts"" value=""(.*?)""").Groups[1].ToString();
            String li = Regex.Match(content, @"name=""li"" value=""(.*?)""").Groups[1].ToString();
            String fb_dtsg = Regex.Match(content, @"name=""fb_dtsg"" value=""(.*?)""").Groups[1].ToString();
            MessageBox.Show("lsd: " + lsd + "\n " + "jaz: " + jaz + "\n" + "m_ts: " + m_ts + "\n" + "li: " + li + "\n" + "fb_dtsg: " + fb_dtsg);
            //String data_post = $"lsd={lsd}&jazoest={jaz}&m_ts={m_ts}&li={li}&try_number=0&unrecognized_tries=0&email={uid}&pass={pass}&login=Log+In&bi_xrwh=0";
            RestResponse restResponse2 = webRequestClient.PostLogin(lsd, fb_dtsg, jaz, m_ts, li, uid, pass, restResponse.Cookies);
            MessageBox.Show(restResponse2.Content);

        }


    }
}

