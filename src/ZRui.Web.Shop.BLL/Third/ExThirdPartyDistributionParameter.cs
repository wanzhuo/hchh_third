using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ZRui.Web.Common;

namespace ZRui.Web.BLL.Third
{
    public class ExThirdPartyDistributionParameter
    {
        ThirdConfig thirdConfig;
        public ExThirdPartyDistributionParameter(ThirdConfig options)
        {
            this.thirdConfig = options;
        }

        /// <summary>
        /// 预请求第三方配送
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Dictionary<string, string> ExThirdPartyDistributionParameterAction(ThirdPartyDistributionParameterModel model)
        {
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            pairs.Add("app_key", thirdConfig.Appkey);
            pairs.Add("body", model.body);
            pairs.Add("format", "json");

            pairs.Add("source_id", model.source_id);
            pairs.Add("timestamp", CommonUtil.ToTimestamp(DateTime.Now).ToString());
            pairs.Add("v", "1.0");
            ArrayList array = new ArrayList(pairs.Keys);
            array.Sort();
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < array.Count; i++)
            {
                string value = string.Empty;
                pairs.TryGetValue(array[i].ToString(), out value);
                builder.Append(array[i].ToString() + value);
            }
            string signature = MD5Util.GetMD5Hash(thirdConfig.AppSecret + builder.ToString() + thirdConfig.AppSecret).ToUpper();
            pairs.Add("signature", signature);
            //  return builder.ToString();
            return pairs;
        }
    }
}
