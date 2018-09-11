using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using  ZRui.Web.Common;
namespace ZRui.Web
{
    public abstract class MD5AuthorizeArgs
    {
        [JsonProperty("sign")]
        public string Sign { get; set; }
        [JsonProperty("appid")]
        public string AppId { get; set; }
        /// <summary>
        /// 时间戳
        /// </summary>
        /// <remarks>
        /// 这里设置为字符串是为了能够正常接受到提交过来的数据，实际其为整形
        /// </remarks>
        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }

        public abstract Dictionary<string, object> GetValues();

        public void CheckSign()
        {
            if (string.IsNullOrEmpty(AppId)) throw new Exception("缺少appid");
            if (string.IsNullOrEmpty(Sign)) throw new Exception("缺少sign");
            if (string.IsNullOrEmpty(Timestamp)) throw new Exception("缺少timestamp");
            long timestamp = 0;
            if (!long.TryParse(Timestamp, out timestamp)) throw new Exception("timestamp必须是整形");
            var time = ConvertTimeStampToDateTime(timestamp);
            if (time > DateTime.Now.AddMinutes(5) || time < DateTime.Now.AddMinutes(-5)) throw new Exception("时间戳错误");

            var values = GetValues();
            values.Add("sign", Sign);
            values.Add("appid", AppId);
            values.Add("timestamp", Timestamp);

            var result = MD5AuthorizeConfigs.Instance.GetSingle(AppId);
            if (result == null)
            {
                //loginfo.DebugFormat("MD5:不存在AppId为{0}的相关信息", sortDic["appid"]);
                throw new Exception("MD5:不存在AppId为"+ AppId + "的相关信息");
            }
            var key = result.ClientSecret;
            bool isSign = MD5Util.Verify(values, Sign, key);
            if (!isSign) throw new Exception("MD5签名错误");
        }

        public static  string CreateSign(MD5AuthorizeArgs args,string appKey)
        {
            var values = args.GetValues();
            values.Add("appid", args.AppId);
            values.Add("timestamp", args.Timestamp);
            var sign = MD5Util.GetSign(values, appKey);
            return sign;
        }

        private DateTime ConvertTimeStampToDateTime(long unixTimeStamp)
        {
            //System.DateTime startTime = System.TimeZoneInfo.ConvertTime(new System.DateTime(1970, 1, 1).ToUniversalTime(), TimeZoneInfo.Local); // 当地时区
            var startTime = new System.DateTime(1970, 1, 1).ToLocalTime();
            DateTime dt = startTime.AddSeconds(unixTimeStamp);
            return dt;
        }
    }
}
