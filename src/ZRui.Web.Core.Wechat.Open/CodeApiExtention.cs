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
    public class QRCodeJumpGetResult : WxJsonResult
    {
        public IList<QRCodeJumpItem> rule_list { get; set; }
        public int qrcodejump_open { get; set; }
        public int list_size { get; set; }
        public int qrcodejump_pub_quota { get; set; }
    }

    public class QRCodeJumpItem
    {
        public string prefix { get; set; }
        public string path { get; set; }
        public int state { get; set; }
        public int open_version { get; set; }
        public int permit_sub_rule { get; set; }
        public string[] debug_url { get; set; }
    }

    public class QRCodeJumpDownloadResult : WxJsonResult
    {
        public string file_name { get; set; }
        public string file_content { get; set; }
    }

    public static class CodeApiExt
    {
        public static CodeResultJson QRCodeJumpAdd(
            string accessToken
            , string prefix
            , string permit_sub_rule
            , string path
            , string open_version
            , string[] debug_url
            , bool is_edit
            , int timeOut = Config.TIME_OUT)
        {
            var url = string.Format(Config.ApiMpHost + "/cgi-bin/wxopen/qrcodejumpadd?access_token={0}", accessToken.AsUrlData());

            object data;

            data = new
            {
                prefix = prefix,
                permit_sub_rule = permit_sub_rule,
                path = path,
                open_version = open_version,
                debug_url = debug_url,
                is_edit = is_edit ? 1 : 0
            };
            return CommonJsonSend.Send<CodeResultJson>(null, url, data, CommonJsonSendType.POST, timeOut);
        }

        public static QRCodeJumpDownloadResult QRCodeJumpDownload(
            string accessToken
            , int timeOut = Config.TIME_OUT)
        {
            var url = string.Format(Config.ApiMpHost + "/cgi-bin/wxopen/qrcodejumpdownload?access_token={0}", accessToken.AsUrlData());

            var data = new { };

            return CommonJsonSend.Send<QRCodeJumpDownloadResult>(null, url, data, CommonJsonSendType.POST, timeOut);
        }

        public static QRCodeJumpGetResult QRCodeJumpGet(
            string accessToken
            , int timeOut = Config.TIME_OUT)
        {
            var url = string.Format(Config.ApiMpHost + "/cgi-bin/wxopen/qrcodejumpget?access_token={0}", accessToken.AsUrlData());

            var data = new { };

            return CommonJsonSend.Send<QRCodeJumpGetResult>(null, url, data, CommonJsonSendType.POST, timeOut);
        }

        public static CodeResultJson QRCodeJumpDelete(
            string accessToken
            , string prefix
            , int timeOut = Config.TIME_OUT)
        {
            var url = string.Format(Config.ApiMpHost + "/cgi-bin/wxopen/qrcodejumpdelete?access_token={0}", accessToken.AsUrlData());

            var data = new { prefix = prefix };

            return CommonJsonSend.Send<CodeResultJson>(null, url, data, CommonJsonSendType.POST, timeOut);
        }

        public static CodeResultJson QRCodeJumpPublish(
           string accessToken
           , string prefix
           , int timeOut = Config.TIME_OUT)
        {
            var url = string.Format(Config.ApiMpHost + "/cgi-bin/wxopen/qrcodejumppublish?access_token={0}", accessToken.AsUrlData());

            var data = new { prefix };

            return CommonJsonSend.Send<CodeResultJson>(null, url, data, CommonJsonSendType.POST, timeOut);
        }

        /// <summary>
        /// 添加，发布二维码规则
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="authorizerAccessToken"></param>
        /// <param name="hostingEnvironment"></param>
        public static CodeResultJson QRCodeJumpAddPublish(int shopId,string authorizerAccessToken, Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment)
        {
            List<string> RequestDomains = new List<string>
            {
                "https://wxapi.91huichihuihe.com",
                 "https://wxappapitest.91huichihuihe.com",
            };
            ModifyDomainApi.ModifyDomain(authorizerAccessToken, Senparc.Weixin.Open.ModifyDomainAction.set, RequestDomains, new List<string>(), new List<string>(), new List<string>());

            string prefixUrl = $"http://manager.91huichihuihe.com/qrcodeJump/{shopId}/shopPart/";
            var prefix = new Uri(prefixUrl);

            var fileInfo = CodeApiExt.QRCodeJumpDownload(authorizerAccessToken);
            var filePath = hostingEnvironment.MapWebPath(Path.Combine(prefix.PathAndQuery, fileInfo.file_name));
            Common.FileUtils.CreateDirectory(filePath);
            File.WriteAllText(filePath, fileInfo.file_content);
            var DebugUrl = new List<string>();
            string permitSubRule = "1";
            string path = "pages/order/home";
            string openVersion = "3";
            try
            {
                QRCodeJumpAdd(authorizerAccessToken, prefixUrl, permitSubRule, path, openVersion, DebugUrl.ToArray(), false);
            }
            catch (Exception) { }
            return QRCodeJumpPublish(authorizerAccessToken, prefixUrl);
        }

    }
}
