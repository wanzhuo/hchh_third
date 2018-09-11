using Senparc.Weixin;
using Senparc.Weixin.CommonAPIs;
using Senparc.Weixin.Entities;
using Senparc.Weixin.Helpers;
using Senparc.Weixin.HttpUtility;
using Senparc.Weixin.Open.WxaAPIs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ZRui.Web.Core.Wechat.Open
{
    public class TesterGetListResult : WxJsonResult
    {
        public IList<TesterItem> members { get; set; }
    }

    public class TesterItem
    {
        public string userstr { get; set; }
    }

    public static class TesterApiExt
    {

        public static TesterGetListResult GetList(
            string accessToken
            , int timeOut = Config.TIME_OUT)
        {
            var url = string.Format(Config.ApiMpHost + "/wxa/memberauth?access_token={0}", accessToken.AsUrlData());

            var data = new
            {
                action = "get_experiencer"
            };

            return CommonJsonSend.Send<TesterGetListResult>(null, url, data, CommonJsonSendType.POST, timeOut);
        }
    }
}
