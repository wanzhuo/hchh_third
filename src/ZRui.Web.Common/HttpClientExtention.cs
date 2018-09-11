using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using ZRui.Web.Common;
using System.IO;

namespace ZRui.Web
{
    public static class HttpClientExtention
    {
        public static async System.Threading.Tasks.Task<T> Post<T>(this HttpClient client, string url, object data)
        {
            var json = JsonConvert.SerializeObject(data);
            var requestContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, requestContent);

            if (response.IsSuccessStatusCode)
            {
                var reContent = await response.Content.ReadAsStringAsync();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(reContent);
            }
            else
            {
                throw new Exception("提交失败");
            }
        }

        public static async System.Threading.Tasks.Task<string> GetHtml(this HttpClient httpClient, string url)
        {
            var content = string.Empty;
            var data = await httpClient.GetByteArrayAsync(url);

            var ide = new IdentifyEncoding();
            var encodingName = ide.GetEncodingString(IdentifyEncoding.ToSByteArray(data));
            var encoding = System.Text.Encoding.GetEncoding(encodingName);

            content = encoding.GetString(data);

            return content;
        }

        public static async System.Threading.Tasks.Task<string> DownloadHtml(this HttpClient httpClient, string url, string savePath)
        {
            ZRui.Web.Common.FileUtils.CreateDirectory(savePath);
            var content = string.Empty;
            try
            {
                //applyDocument.AttachmentDownloadUrl 形如：//dfiles.tms.beisen.com/download/resume/180033/1442375888/038a9a37460c4b2e841b4763fa74051e.html?sig_a=beisen.recruitment.openapi&sig_t=1489501016&sig_v=1&sig=23ceb9a2c34e5ff811db8fface5cd43cb4786952
                //所以需要在上面的基础上加上http:

                content = await httpClient.GetHtml(url);
                System.IO.File.WriteAllText(savePath, content, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                content = $"Download Fail：{ex.Message}";
                System.IO.File.WriteAllText(savePath, content, Encoding.UTF8);
                //throw;
            }

            return content;
        }

        public static async System.Threading.Tasks.Task Download(this HttpClient httpClient, string url, string savePath)
        {
            ZRui.Web.Common.FileUtils.CreateDirectory(savePath);
            var data = await httpClient.GetByteArrayAsync(url);
            System.IO.File.WriteAllBytes(savePath, data);
        }
    }
}