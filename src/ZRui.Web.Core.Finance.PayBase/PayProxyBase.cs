using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Net.Http;
using System.Text;

namespace ZRui.Web.Core.Finance.PayBase
{
    public abstract class PayProxyBase
    {
        protected IPayOption options;
        protected readonly ILogger _logger;
        protected static readonly HttpClient httpClient = new HttpClient()
        {
            Timeout = TimeSpan.FromMinutes(2)
        };
        public PayProxyBase(IPayOption options,ILogger logger)
        {
            this.options = options;
            _logger = logger;
        }

        protected PayResponseBaseHandler Post(string url, string requestContent)
            => Post<PayResponseBaseHandler>(url, requestContent);

        protected T Post<T>(string url, string requestContent) where T: PayResponseBaseHandler,new ()
        {
            var responseWait = httpClient.PostAsync(url, new StringContent(requestContent));
            responseWait.Wait();
            var response = responseWait.Result;

            if (response.IsSuccessStatusCode)
            {
                var reContentWait = response.Content.ReadAsStringAsync();
                reContentWait.Wait();
                var reContent = reContentWait.Result;
                //_logger.LogInformation("Pay result" + reContent);
                T rtn = new T();
                rtn.SetXmlContent(reContent);
                return rtn;
            }
            else
            {
                throw new Exception($"提交失败：{response.Content}");
            }
        }

        /// <summary>
        /// 将Hashtable参数传为XML
        /// </summary>
        /// <param name="_params"></param>
        /// <returns></returns>
        protected string toXml(Hashtable _params)
        {
            StringBuilder sb = new StringBuilder("<xml>");
            foreach (DictionaryEntry de in _params)
            {
                string key = de.Key.ToString();

                sb.Append("<").Append(key).Append("><![CDATA[").Append(de.Value + "").Append("]]></").Append(key).Append(">");
            }

            return sb.Append("</xml>").ToString();
        }

        /// <summary>
        /// 支付渠道
        /// </summary>
        public abstract string PayChannel { get; }

        /// <summary>
        /// 生成签名
        /// </summary>
        public abstract string MakeSign(Hashtable parameters);

        /// <summary>
        /// 获取支付信息
        /// </summary>
        /// <param name="rechange"></param>
        /// <param name="sub_openid"></param>
        /// <param name="sub_appid"></param>
        /// <returns></returns>
        public abstract object GetPayInfo(MemberTradeForRechange rechange, string sub_openid);

        public abstract string GetPayAppInfo(MemberTradeForRechange rechange, string appid);

        /// <summary>
        /// 获取支付结果
        /// </summary>
        /// <param name="rechange"></param>
        /// <returns></returns>
        public abstract PayResponseBaseHandler GetPayResult(MemberTradeForRechange rechange);

        /// <summary>
        /// 退款
        /// </summary>
        /// <param name="sub_openid"></param>
        /// <returns></returns>
        public abstract object Refund(MemberTradeForRefund refund);

        /// <summary>
        /// 退款通知
        /// </summary>
        /// <param name="rechange"></param>
        /// <returns></returns>
        public abstract PayResponseBaseHandler GetRefundResult(MemberTradeForRefund refund);

    }
}
