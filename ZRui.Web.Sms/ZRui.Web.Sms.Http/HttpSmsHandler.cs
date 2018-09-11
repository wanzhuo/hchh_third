using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ZRui.Web.Sms
{
    public class HttpSmsHandler : ISmsHandler
    {
        string userId, password;
        HttpClient client = new HttpClient();
        ILogger _logger;
        HttpSmsOptions options;
        public HttpSmsHandler(IOptions<HttpSmsOptions> options, ILoggerFactory loggerFactory)
        {
            this.options = options.Value;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            _logger = loggerFactory.CreateLogger<HttpSmsHandler>();
        }

        public async Task<bool> SendAsync(string phones, string content)
        {
            var url = string.Format(options.Url, phones, content);
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var reContent = await response.Content.ReadAsStringAsync();
                var result = new HttpSmsSendResult(reContent);
                //这里还需要判定是否发送成功
                return true;
            }
            else
            {
                _logger.LogError("Http 发送失败:{0}", response.IsSuccessStatusCode);
                return false;
            }
        }
    }
}
