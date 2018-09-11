using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Text;
using ZRui.Web.Common;
using ZRui.Web.Core.Finance.PayBase;

namespace ZRui.Web.Core.Finance.WechatPay
{
    public class WechatPayProxy: PayProxyBase
    {
        //todo
        //string Mchid = "1502945001", serctKey = "653o2wtivz3t45gl2v7dqym15drgov6r", sub_appid = "wx1056c536d7f26c3e";

        Encoding encoding = Encoding.UTF8;
        ShopPayInfo shopPayInfo;

        public WechatPayProxy(ShopPayInfo shopPayInfo, IPayOption options, ILogger logger) : base(options, logger)
            => this.shopPayInfo = shopPayInfo;

        public override string PayChannel => "微信支付";

        public override string GetPayAppInfo(MemberTradeForRechange rechange, string appid)
        {
            throw new NotImplementedException();
        }

        public override object GetPayInfo(MemberTradeForRechange rechange, string sub_openid)
        {
            var requestHandler = new WechatPayRequestHandler(options);
            requestHandler.setParameter("appid", shopPayInfo.AppId);//微信分配的小程序ID 
            requestHandler.setParameter("mch_id", shopPayInfo.MchId);//必填项，商户号，由平台分配
            requestHandler.setParameter("device_info", "");//终端设备号
            requestHandler.setParameter("nonce_str", CommonUtil.CreateNoncestr(16));//随机字符串，必填项，不长于 32 位
            requestHandler.setParameter("body", rechange.Detail);//商品描述
            requestHandler.setParameter("attach", "");//附加信息
            requestHandler.setParameter("out_trade_no", rechange.TradeNo);//商户订单号
            requestHandler.setParameter("total_fee", rechange.TotalFee.ToString());//总金额
            requestHandler.setParameter("spbill_create_ip", rechange.AddIP);//终端IP
            requestHandler.setParameter("time_start", rechange.AddTime.ToString("yyyyMMddHHmmss")); //订单生成时间
            requestHandler.setParameter("time_expire", rechange.AddTime.AddHours(1).ToString("yyyyMMddHHmmss"));//订单超时时间
            requestHandler.setParameter("goods_tag", "");//订单优惠标记   
            requestHandler.setParameter("notify_url", options.NotifyUrl);
            requestHandler.setParameter("trade_type", "JSAPI");//小程序取值如下：JSAPI            
            requestHandler.setParameter("openid", sub_openid);//测试账号不传值,此处默认给空值。正式账号必须传openid值，获取openid值指导文档地址：http://www.cnblogs.com/txw1958/p/weixin76-user-info.html
            //requestHandler.setParameter("sign_type", "MD5");//签名类型，默认为MD5 
            requestHandler.CreateSign(MakeSign);//创建签名

            string data = toXml(requestHandler.getAllParameters());//生成XML报文
            var result = Post<WechatPayResponseHandler>(options.OrderUrl, data);
            //Hashtable param = requestHandler.getAllParameters();

            if ("FAIL".Equals(result.ReturnCode) || "FAIL".Equals(result.ResultCode))
                throw new Exception($"错误代码：{result.ReturnCode},错误信息：{result.Message}");

            if (result.isTenpaySign(MakeSign))
            {
                return result.CreatePayInfo(MakeSign);
            }
            else
            {
                throw new Exception($"状态：{result.ReturnCode},信息：{result.Message}");
            }
        }

        public override PayResponseBaseHandler GetPayResult(MemberTradeForRechange rechange)
        {
            var requestHandler = new WechatPayRequestHandler(options);
            requestHandler.setParameter("appid", shopPayInfo.AppId);//小程序id
            requestHandler.setParameter("mch_id", shopPayInfo.MchId);//必填项，商户号，由平台分配
            requestHandler.setParameter("out_trade_no", rechange.TradeNo);//平台订单号                
            requestHandler.setParameter("nonce_str", CommonUtil.CreateNoncestr(16));//随机字符串，必填项，不长于 32 位
            requestHandler.CreateSign(MakeSign);//创建签名
            //_logger.LogInformation("GetPayResult result 签名成功");

            string data = toXml(requestHandler.getAllParameters());//生成XML报文

            //_logger.LogInformation("GetPayResult result 生成XML报文：" + data);

            var result = Post(options.OrderQueryUrl, data);
            //Hashtable param = requestHandler.getAllParameters();
            if ("FAIL".Equals(result.ReturnCode) || "FAIL".Equals(result.ResultCode))
                throw new Exception($"错误代码：{result.ReturnCode},错误信息：{result.Message}");
            if (result.isTenpaySign(this.MakeSign))
            {
                return result;
            }
            else
            {
                throw new Exception($"状态：{result.ReturnCode},信息：{result.Message}");
            }
        }

        public override PayResponseBaseHandler GetRefundResult(MemberTradeForRefund refund)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 生成签名
        /// </summary>
        /// <returns></returns>
        public override string MakeSign(Hashtable parameters)
        {
            StringBuilder sb = new StringBuilder();
            string mchPrivateKey = string.Empty;
            string sign = string.Empty;
            ArrayList akeys = new ArrayList(parameters.Keys);
            akeys.Sort();
            string signtype = string.Empty;
            foreach (string k in akeys)
            {
                string v = (string)parameters[k];
                if (null != v && "".CompareTo(v) != 0
                    && "sign".CompareTo(k) != 0 && "key".CompareTo(k) != 0)
                {
                    sb.Append(k + "=" + v + "&");
                }
            }
            sb.Append("key=" + shopPayInfo.SecretKey);
            sign = MD5Util.GetMD5Hash(sb.ToString(), encoding).ToUpper();
            return sign;
        }

        public override object Refund(MemberTradeForRefund refund)
        {
            throw new NotImplementedException();
        }
    }
}
