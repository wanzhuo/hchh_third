using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;


namespace HuiChiHuiHe.WinService
{
    public class WebApiHelper
    {
         static  string Domain
        {
            get
            {
                string result = "admintest.91huichihuihe.com";
                if (ConfigurationManager.AppSettings["domain"] != null )
                {
                    result = ConfigurationManager.AppSettings["domain"].ToString();
                }
                return result;               
            }
        }
        public static ApiResult Invoke(HttpMethod method, string api, Dictionary<string, string> args, Dictionary<string, string> headers)
        {
            HttpClient myHttpClient = new HttpClient();
            myHttpClient.DefaultRequestHeaders.Accept.Clear();
            if (headers != null && headers.Count > 0)
            {
                foreach (var header in headers)
                {
                    myHttpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            string url = Domain + api;
            HttpResponseMessage response = null;
            if (method == HttpMethod.Get)
            {
                if (args != null && args.Count > 0)
                {
                    url += "?";
                    foreach (var arg in args)
                    {
                        url += arg.Key + "=" + arg.Value + "&";
                    }
                    url = url.TrimEnd('&');
                }
                response = myHttpClient.GetAsync(url).Result;
            }
            else if (method == HttpMethod.Post)
            {
                FormUrlEncodedContent content = null;
                if (args != null)
                {
                    content = new FormUrlEncodedContent(args);
                }
                response = myHttpClient.PostAsync(url, content).Result;
            }
            else
            {
                return new ApiResult() { StatusCode = APIStatusCode.Failure, Message = "不支持post和get以外的方法" };
            }

            if (!response.IsSuccessStatusCode)
            {
                return new ApiResult() { StatusCode = APIStatusCode.Failure, Message = "调用失败:" + response.ReasonPhrase };
            }
            return Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResult>(response.Content.ReadAsStringAsync().Result);

        }


        public static string Send(HttpMethod method, string url, string jsonData = "")
        {
            HttpClient myHttpClient = new HttpClient();
            //设置RequestMessage
            HttpRequestMessage RequestMessage = new HttpRequestMessage();
            RequestMessage.Method = method;
            RequestMessage.RequestUri = new Uri(url);
            if (!string.IsNullOrEmpty(jsonData))
            {
                RequestMessage.Content = new StringContent(jsonData);
                RequestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }
            HttpResponseMessage ResponseMessage = myHttpClient.SendAsync(RequestMessage).Result;
            var responseJson = ResponseMessage.Content.ReadAsStringAsync().Result;
            return responseJson;
        }


    }


}
