using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace ZRui.Web.Core.Wechat
{
    public class WechatMediaApiProxy
    {
        private static readonly HttpClient httpClient = new HttpClient();
        public static async System.Threading.Tasks.Task<T> UploadAsync<T>(string accessToken, string mediaType, string filePath)
        {
            var url = $"https://api.weixin.qq.com/cgi-bin/media/upload?access_token={accessToken}&type={mediaType}";
            var boundary = Guid.NewGuid().ToString();
            var requestContent = new MultipartFormDataContent(boundary);
            requestContent.Headers.Remove("Content-Type");
            requestContent.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data; boundary=" + boundary);
            var contentByte = new ByteArrayContent(File.ReadAllBytes(filePath));
            contentByte.Headers.Remove("Content-Disposition");
            contentByte.Headers.TryAddWithoutValidation("Content-Disposition", $"form-data; name=\"media\";filename=\"{Path.GetFileName(filePath)}\"");
            requestContent.Add(contentByte);
            //requestContent.Add(new StreamContent(new FileStream(filePath, FileMode.Open)), "media", Path.GetFileName(filePath));
            var response = await httpClient.PostAsync(url, requestContent);
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

        public static async System.Threading.Tasks.Task<T> UploadForeverAsync<T>(string accessToken, string mediaType, string filePath)
        {
            var url = $"https://api.weixin.qq.com/cgi-bin/material/add_material?access_token={accessToken}&type={mediaType}";
            var boundary = Guid.NewGuid().ToString();
            var requestContent = new MultipartFormDataContent(boundary);
            requestContent.Headers.Remove("Content-Type");
            requestContent.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data; boundary=" + boundary);
            var contentByte = new ByteArrayContent(File.ReadAllBytes(filePath));
            contentByte.Headers.Remove("Content-Disposition");
            contentByte.Headers.TryAddWithoutValidation("Content-Disposition", $"form-data; name=\"media\";filename=\"{Path.GetFileName(filePath)}\"");
            requestContent.Add(contentByte);
            //requestContent.Add(new StreamContent(new FileStream(filePath, FileMode.Open)), "media", Path.GetFileName(filePath));
            var response = await httpClient.PostAsync(url, requestContent);
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
    }
}
