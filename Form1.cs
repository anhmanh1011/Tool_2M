namespace Tool_2M
{
    using Newtonsoft.Json.Linq;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using OtpNet;
    using System;
    using System.Windows.Forms;
    using static System.Windows.Forms.VisualStyles.VisualStyleElement;

    public partial class Form1 : Form
    {
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
                goToBusinessLocation(driver, dataGridViewRow);

            }

        }
    }

    private async void goToBusinessLocation(ChromeDriver driver, DataGridViewRow dataGridViewRow)
    {
        driver.Navigate().GoToUrl("https://business.facebook.com/business_locations");
        Thread.Sleep(3000);
        if (driver.Url.Contains("https://business.facebook.com/security/twofactor/reauth"))
        {
            Thread.Sleep(2000);
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

}