using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace ZRui.Web.Core.Printer
{
    /// <summary>
    /// 对调用API时HttpRequest相关方法的实现
    /// </summary>
   public class PrintRequest
    {
        /// <summary>
        /// 远程请求方法
        /// </summary>
        /// <param name="requestData">所传递的参数字符串id=&xx=</param>
        /// <param name="url">请求地址</param>
        /// <param name="encoding">采用的编码方式，如若为null 采用UTF-8</param>
        /// <param name="method">请求方式 默认POST</param>
        /// <param name="contenttype">默认application/x-www-form-urlencoded</param>
        /// <returns></returns>
        public static string RequestMethod(string requestData, string url, Encoding encoding, string method = "POST", string contenttype = "application/x-www-form-urlencoded")
        {
            if (string.IsNullOrEmpty(requestData))
                return "请求参数错误";
            if (string.IsNullOrEmpty(url))
                return "请求地址错误";
            HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
            byte[] data = encoding == null ? Encoding.Default.GetBytes(requestData) : encoding.GetBytes(requestData);
            request.Method = method;
            request.ContentType = contenttype;
            request.ContentLength = data.Length;
            Stream stream = request.GetRequestStream();
            stream.Write(data, 0, data.Length);
            stream.Close();
            //获取返回信息
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            string result = "";
            try
            {
                Stream responsestrem = response.GetResponseStream();
                StreamReader reader = new StreamReader(responsestrem);
                result = reader.ReadToEnd();
                return Regex.Unescape(result);
            }
            catch (Exception e)
            {

                return e.Message;
            }
            finally
            {
                response.Close();
                request.Abort();
            }

        }
    }
}
