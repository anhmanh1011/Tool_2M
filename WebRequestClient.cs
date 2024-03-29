﻿using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Tool_2M
{
    public class WebRequestClient
    {
        public RestResponse GoToUrl(String url, CookieCollection cookieCollection)
        {
            var client = new RestClient(url);
            var request = new RestRequest(url);
            SetDefaultRequest(request, cookieCollection);
            return client.Execute(request);
        }

        public RestResponse GetMbasicPage()
        {
            var client = new RestClient("https://mbasic.facebook.com/");
            var request = new RestRequest();
            request.AddHeader("authority", "mbasic.facebook.com");
            request.AddHeader("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
            request.AddHeader("accept-language", "en");
            request.AddHeader("sec-ch-ua", "\"Chromium\";v=\"110\", \"Not A(Brand\";v=\"24\", \"Google Chrome\";v=\"110\"");
            request.AddHeader("sec-ch-ua-mobile", "?0");
            request.AddHeader("sec-ch-ua-platform", "\"Windows\"");
            request.AddHeader("sec-fetch-dest", "document");
            request.AddHeader("sec-fetch-mode", "navigate");
            request.AddHeader("sec-fetch-site", "none");
            request.AddHeader("sec-fetch-user", "?1");
            request.AddHeader("upgrade-insecure-requests", "1");
            client.AddDefaultHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36");
            return client.Execute(request);
        }

        public RestResponse PostLogin(String lsd, String fb_dtsg, String jazoest, String m_ts, String li, String uid, String pass, CookieCollection? cookies)
        {
            var client = new RestClient("https://mbasic.facebook.com/login/device-based/regular/login/?refsrc=deprecated&lwv=100&refid=8");
            var request = new RestRequest("https://mbasic.facebook.com/login/device-based/regular/login/?refsrc=deprecated&lwv=100&refid=8", Method.Post);
            SetDefaultRequest(request, cookies);

            client.AddDefaultHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36");
            request.AddParameter("fb_dtsg", fb_dtsg);
            request.AddParameter("lsd", lsd);
            request.AddParameter("jazoest", jazoest);
            request.AddParameter("m_ts", m_ts);
            request.AddParameter("li", li);
            request.AddParameter("try_number", "0");
            request.AddParameter("unrecognized_tries", "0");
            request.AddParameter("email", uid);
            request.AddParameter("pass", pass);
            request.AddParameter("login", "Log+In");
            request.AddParameter("bi_xrwh", "0");
            return client.Execute(request);
        }

        private void SetDefaultRequest(RestRequest request, CookieCollection? cookies)
        {
            request.AddHeader("authority", "mbasic.facebook.com");
            request.AddHeader("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
            request.AddHeader("accept-language", "en-US,en;q=0.9");
            request.AddHeader("cache-control", "max-age=0");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddHeader("origin", "https://mbasic.facebook.com");
            request.AddHeader("sec-ch-ua", "\"Chromium\";v=\"110\", \"Not A(Brand\";v=\"24\", \"Microsoft Edge\";v=\"110\"");
            request.AddHeader("sec-ch-ua-mobile", "?0");
            request.AddHeader("sec-ch-ua-platform", "\"Windows\"");
            request.AddHeader("sec-fetch-dest", "document");
            request.AddHeader("sec-fetch-mode", "navigate");
            request.AddHeader("sec-fetch-site", "same-origin");
            request.AddHeader("sec-fetch-user", "?1");
            request.AddHeader("upgrade-insecure-requests", "1");
            String cookie = "";
            for (int i = 0; i < cookies.Count; i++)
            {
                cookie += cookies[i].Name + "=" + cookies[i].Value + ";";

            }
            request.AddHeader("Cookie", cookie);

        }

        public RestResponse PostCheckPoint(CookieCollection? cookies, String fb_dtsg, String jazoest, String nh, String approvals_code)
        {

            var client = new RestClient("https://mbasic.facebook.com/login/checkpoint/");
            client.AddDefaultHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36");
            var request = new RestRequest("https://mbasic.facebook.com/login/checkpoint/", Method.Post);
            request.Method = Method.Post;
            SetDefaultRequest(request, cookies);
            String body = $"fb_dtsg={HttpUtility.UrlEncode(fb_dtsg)}&jazoest={HttpUtility.UrlEncode(jazoest)}&checkpoint_data=&approvals_code={approvals_code}&submit%5BSubmit+Code%5D=Submit+Code&nh={HttpUtility.UrlEncode(nh)}&fb_dtsg={HttpUtility.UrlEncode(fb_dtsg)}&jazoest={HttpUtility.UrlEncode(jazoest)}";
            Console.WriteLine(body);
            request.AddBody(body, "application/x-www-form-urlencoded");

            return client.ExecutePost(request);

        }
        public RestResponse PostSaveDevice(CookieCollection? cookies, String fb_dtsg, String jazoest, String nh, String approvals_code)
        {
            fb_dtsg = HttpUtility.UrlEncode(fb_dtsg);
            jazoest = HttpUtility.UrlEncode(jazoest);
            nh = HttpUtility.UrlEncode(nh);
            approvals_code = HttpUtility.UrlEncode(approvals_code);




            var client = new RestClient("https://mbasic.facebook.com/login/checkpoint/");
            client.AddDefaultHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36");
            var request = new RestRequest("https://mbasic.facebook.com/login/checkpoint/", Method.Post);
            request.Method = Method.Post;
            SetDefaultRequest(request, cookies);
            //String body = $"fb_dtsg={}&jazoest={HttpUtility.UrlEncode(jazoest)}&checkpoint_data=&approvals_code={approvals_code}&submit%5BSubmit+Code%5D=Submit+Code&nh={HttpUtility.UrlEncode(nh)}&fb_dtsg={HttpUtility.UrlEncode(fb_dtsg)}&jazoest={HttpUtility.UrlEncode(jazoest)}";
            String body = $"fb_dtsg={fb_dtsg}&jazoest={jazoest}&checkpoint_data=&name_action_selected=save_device&submit%5BContinue%5D=Continue&nh={nh}&fb_dtsg={fb_dtsg}&jazoest={jazoest}";

            Console.WriteLine(body);
            request.AddBody(body, "application/x-www-form-urlencoded");

            return client.ExecutePost(request);
        }

        public RestResponse PostReviewRecentLogin(CookieCollection? cookies, String fb_dtsg, String jazoest, String nh, String approvals_code)
        {
            fb_dtsg = HttpUtility.UrlEncode(fb_dtsg);
            jazoest = HttpUtility.UrlEncode(jazoest);
            nh = HttpUtility.UrlEncode(nh);
            approvals_code = HttpUtility.UrlEncode(approvals_code);

            var client = new RestClient("https://mbasic.facebook.com/login/checkpoint/");
            client.AddDefaultHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36");
            var request = new RestRequest("https://mbasic.facebook.com/login/checkpoint/", Method.Post);
            request.Method = Method.Post;
            SetDefaultRequest(request, cookies);
            //String body = $"fb_dtsg={}&jazoest={HttpUtility.UrlEncode(jazoest)}&checkpoint_data=&approvals_code={approvals_code}&submit%5BSubmit+Code%5D=Submit+Code&nh={HttpUtility.UrlEncode(nh)}&fb_dtsg={HttpUtility.UrlEncode(fb_dtsg)}&jazoest={HttpUtility.UrlEncode(jazoest)}";
            String body = $"fb_dtsg={fb_dtsg}&jazoest={jazoest}&checkpoint_data=&submit%5BContinue%5D=Continue&nh={nh}&fb_dtsg{fb_dtsg}&jazoest={jazoest}";

            Console.WriteLine(body);
            request.AddBody(body, "application/x-www-form-urlencoded");

            return client.ExecutePost(request);
        }
        public RestResponse Post(CookieCollection? cookies, String url, String body)
        {

            var client = new RestClient(url);
            client.AddDefaultHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36");
            var request = new RestRequest(url, Method.Post);
            request.Method = Method.Post;
            SetDefaultRequest(request, cookies);
            //String body = $"fb_dtsg={}&jazoest={HttpUtility.UrlEncode(jazoest)}&checkpoint_data=&approvals_code={approvals_code}&submit%5BSubmit+Code%5D=Submit+Code&nh={HttpUtility.UrlEncode(nh)}&fb_dtsg={HttpUtility.UrlEncode(fb_dtsg)}&jazoest={HttpUtility.UrlEncode(jazoest)}";
            Console.WriteLine(body);
            request.AddBody(body, "application/x-www-form-urlencoded");

            return client.ExecutePost(request);
        }

        public String get2FaApi(String haiFa)
        {
            var client = new RestClient("https://2fa.live");
            var request = new RestRequest("https://2fa.live/tok/" + haiFa, Method.Get);
            RestResponse queryResult = client.Execute(request);
            string? content2fa = queryResult.Content;
            Console.WriteLine(content2fa);
            JObject jsonObject = JObject.Parse(content2fa);
            Console.WriteLine($"{jsonObject.ToString()}");
            return jsonObject["token"].ToString();
        }
    }
}
