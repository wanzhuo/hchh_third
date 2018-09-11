using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ZRui.Web.BLL
{

    public interface ISMS
    {
        APIResult Send(string mobiles, string content);
    }
    public class SMS_huichihuihe : ISMS
    {
        public APIResult Send(string mobiles, string content)
        {
            throw new NotImplementedException();
        }
    }
    public class SMS_kuaichuang : ISMS
    {
        const string Url = "http://101.37.194.244:25678/HttpMtInfo/save.do";
        const string UserName = "kuaichuang";
        const string Password = "!At@0207#";
        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="mobiles">手机号码，多个手机号码用‘,’分隔</param>
        /// <param name="content">短信内容</param>
        /// <returns>Success：true/false</returns>
        public APIResult Send(string mobiles, string content)
        {
            if (string.IsNullOrWhiteSpace(mobiles))
            {
                return new APIResult() { Success = false, Message = "phone number is null" };
            }
            if (string.IsNullOrWhiteSpace(content) || content.Length > 490)
            {
                return new APIResult() { Success = false, Message = "message  is null or too long" };
            }

            // foreach (string mobile in mobiles.Split(',')

            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssffff");
            string sign = Sign(timestamp);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var postData = new
            {
                username = UserName,
                sign,
                timestamp,
                messagelist = new object[1] { new { content = HttpUtility.UrlEncode("【惠吃惠喝】"+content, Encoding.GetEncoding("gbk")), mobile = mobiles } }
            };

            string resut = SendHttp(HttpMethod.Post, Url, JsonConvert.SerializeObject(postData)).Result;
            JToken jsonResult = (JToken)JsonConvert.DeserializeObject(resut);
            return new APIResult() { Success = jsonResult["result"].ToString() == "1",Message= resut };
        }

        //   return true;



        /// <summary>
        /// 加密签名
        /// </summary>
        /// <param name="timestamp">时间戳</param>
        /// <returns></returns>
        protected static string Sign(string timestamp)
        {
            string strSource = UserName + Password + timestamp;
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();

            //获取密文字节数组
            byte[] bytResult = md5.ComputeHash(Encoding.UTF8.GetBytes(strSource));

            //转换成字符串，并取9到25位 
            //string strResult = BitConverter.ToString(bytResult, 4, 8);  
            //转换成字符串，32位

            string strResult = BitConverter.ToString(bytResult);

            //BitConverter转换出来的字符串会在每个字符中间产生一个分隔符，需要去除掉 
            strResult = strResult.Replace("-", "").ToUpper();
            return strResult;
        }

        /// <summary>
        /// 发送http请求
        /// </summary>
        /// <param name="method">发送方式</param>
        /// <param name="url">发送地址</param>
        /// <param name="jsonData">序列化后的json</param>
        /// <returns>返回结果</returns>
        public async Task<string> SendHttp(HttpMethod method, string url, string jsonData = "")
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
            HttpResponseMessage ResponseMessage = await myHttpClient.SendAsync(RequestMessage);
            string responseJson = await ResponseMessage.Content.ReadAsStringAsync();
            return responseJson;
        }

    }

    public class SMSHelper
    {
        public static APIResult Send(string mobiles, string message)
        {
            ISMS sms = new SMS_kuaichuang();
            return sms.Send(mobiles, message);

        }
    }

}
