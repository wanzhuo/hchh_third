using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using ZRui.Web.Common;
using ZRui.Web.Core.Finance.PayBase;

namespace ZRui.Web.Core.Finance.SwiftpassPay
{
    public class SwiftpassPayProxy : PayProxyBase
    {
        // SwiftpassPayOptions options;
        ShopPayInfo payinfo;
        private readonly ILogger _logger;
        ShopDbContext shopDb;
        SwiftpassKey swifpasskey;
        private static readonly HttpClient httpClient = new HttpClient()
        {
            Timeout = TimeSpan.FromMinutes(2)
        };

        public override string PayChannel => "中信支付";

        public SwiftpassPayProxy(ShopPayInfo payinfo, IPayOption payOption, ILogger _logger) : base(payOption, _logger)
        {

            //            options.WftPublicKey = "MFwwDQYJKoZIhvcNAQEBBQADSwAwSAJBAKF4mYjBMHkRqwNWWJ5t9sMriHPBOfMpJLUhMJE6ri28RHHEbYoRWYV7g3Lt2BZpcopoIpblcC07Kn9Mgur3/RcCAwEAAQ==";
            //options.MchPrivateKey = @"MIIBVAIBADANBgkqhkiG9w0BAQEFAASCAT4wggE6AgEAAkEAoXiZiMEweRGrA1ZY
            //nm32wyuIc8E58ykktSEwkTquLbxEccRtihFZhXuDcu3YFmlyimgiluVwLTsqf0yC
            //6vf9FwIDAQABAkBANuTgNOwhCby33AvsHZEn9tUSK8LvB+q4OdXFu89vEjFttOLJ
            //9yyACYvOjmii6e5ThqDjX1codbXuzbXE06KpAiEAztVBPzd/ZDJReOQq4raYIRhX
            //eYkay8ko2f1mIdiRrnMCIQDH2tyUQqiGhtHMerjKW+mgSP0KguGwpC7u1rWqHY3J
            //zQIgMsHs0Cm7bohWrBc6Wwa6UKzbkqzN0aLdDpn7/WRbY/cCIQC8V9O8nT042mFp
            //NNZlTk1T0rU1bLbIw1G/n/TABu4SmQIgH7UBuIqvA6b2O4RANl+c97PuC9+Mdmpr
            //TBP3JwCg5yE=";
            // options.NotifyUrl = "https://wxapi.91huichihuihe.com/SwiftpassPay/Notify";
            //options.ReqUrl = "https://pay.swiftpass.cn/pay/gateway";
            // this.options = options;
            this.payinfo = payinfo;
            this._logger = _logger;
            this.shopDb = ZRui.Web.BLL.DbContextFactory.ShopDb;
            this.swifpasskey = shopDb.SwiftpassKey.FirstOrDefault(r => r.ShopFlag == payinfo.ShopFlag && r.IsEnable);
            if (swifpasskey == null)
            {
                throw new Exception("商家支付密钥未配置");
            }
        }

        public string GetPayInfo(MemberTradeForRechange rechange, string sub_openid, string sub_appid)
        {
            var requestHandler = new SwiftpassPayRequestHandler(swifpasskey);
            requestHandler.setParameter("out_trade_no", rechange.TradeNo);//商户订单号
            requestHandler.setParameter("body", rechange.Detail);//商品描述
            requestHandler.setParameter("attach", "");//附加信息
            requestHandler.setParameter("total_fee", rechange.TotalFee.ToString());//总金额
            requestHandler.setParameter("mch_create_ip", rechange.AddIP);//终端IP
            requestHandler.setParameter("time_start", rechange.AddTime.ToString("yyyyMMddHHmmss")); //订单生成时间
            requestHandler.setParameter("time_expire", rechange.AddTime.AddHours(1).ToString("yyyyMMddHHmmss"));//订单超时时间
            requestHandler.setParameter("service", "pay.weixin.jspay");//接口类型：pay.weixin.jspay
            requestHandler.setParameter("mch_id", payinfo.MchId);//必填项，商户号，由平台分配
            requestHandler.setParameter("version", options.Version);//接口版本号
            requestHandler.setParameter("notify_url", options.NotifyUrl);
            //通知地址，必填项，接收平台通知的URL，需给绝对路径，255字符内;此URL要保证外网能访问   
            requestHandler.setParameter("nonce_str", Common.CommonUtil.CreateNoncestr(16));//随机字符串，必填项，不长于 32 位
            requestHandler.setParameter("charset", "UTF-8");//字符集
            requestHandler.setParameter("sign_type", "RSA_1_256");//签名方式
            requestHandler.setParameter("is_raw", "0");//原生JS值
            requestHandler.setParameter("is_minipg", "1");//表示小程序支付
            requestHandler.setParameter("device_info", "");//终端设备号
            requestHandler.setParameter("sub_appid", sub_appid);
            requestHandler.setParameter("sub_openid", sub_openid);//测试账号不传值,此处默认给空值。正式账号必须传openid值，获取openid值指导文档地址：http://www.cnblogs.com/txw1958/p/weixin76-user-info.html
            requestHandler.setParameter("callback_url", "https://www.swiftpass.cn");//前台地址  交易完成后跳转的 URL，需给绝对路径，255字 符 内 格 式如:http://wap.tenpay.com/callback.asp
            requestHandler.setParameter("goods_tag", "");//商品标记                
            requestHandler.CreateSign();//创建签名

            string data = toXml(requestHandler.getAllParameters());//生成XML报文
            var result = Post(swifpasskey.ReqUrl, data);
            //Hashtable param = requestHandler.getAllParameters();
            if (result.isTenpaySign())
            {
                if (result.Status != 0 || result.ResultCode != 0)
                    throw new Exception($"错误代码：{result.Status},错误信息：{result.Message}");

                return result.PayInfo;
            }
            else
            {
                throw new Exception($"状态：{result.Status},信息：{result.Message}");
            }
        }



        SwiftpassPayResponseHandler Post(string url, string requestContent)
        {
            var responseWait = httpClient.PostAsync(url, new StringContent(requestContent));
            responseWait.Wait();
            var response = responseWait.Result;

            if (response.IsSuccessStatusCode)
            {
                var reContentWait = response.Content.ReadAsStringAsync();
                reContentWait.Wait();
                var reContent = reContentWait.Result;
                _logger.LogInformation("SwiftpassPay result" + reContent);
                return new SwiftpassPayResponseHandler(swifpasskey, reContent);
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
        string toXml(Hashtable _params)
        {
            StringBuilder sb = new StringBuilder("<xml>");
            foreach (DictionaryEntry de in _params)
            {
                string key = de.Key.ToString();

                sb.Append("<").Append(key).Append("><![CDATA[").Append(de.Value + "").Append("]]></").Append(key).Append(">");
            }

            return sb.Append("</xml>").ToString();
        }

        public override string MakeSign(Hashtable parameters)
        {
            throw new NotImplementedException();
        }

        public override object GetPayInfo(MemberTradeForRechange payrecording, string sub_openid)
        {
            var requestHandler = new SwiftpassPayRequestHandler(swifpasskey);
            requestHandler.setParameter("out_trade_no", payrecording.TradeNo);//商户订单号
            requestHandler.setParameter("body", payrecording.Detail);//商品描述
            requestHandler.setParameter("attach", $"{payinfo.ShopFlag}");//附加信息
            requestHandler.setParameter("total_fee", payrecording.TotalFee.ToString());//总金额
            requestHandler.setParameter("mch_create_ip", payrecording.AddIP);//终端IP
            requestHandler.setParameter("time_start", payrecording.AddTime.ToString("yyyyMMddHHmmss")); //订单生成时间
            requestHandler.setParameter("time_expire", payrecording.AddTime.AddHours(1).ToString("yyyyMMddHHmmss"));//订单超时时间
            requestHandler.setParameter("service", "pay.weixin.jspay");//接口类型：pay.weixin.jspay
            requestHandler.setParameter("mch_id", payinfo.MchId);//必填项，商户号，由平台分配
            requestHandler.setParameter("version", "2.0");//接口版本号
            requestHandler.setParameter("notify_url", swifpasskey.Notify);
            //通知地址，必填项，接收平台通知的URL，需给绝对路径，255字符内;此URL要保证外网能访问   
            requestHandler.setParameter("nonce_str", Common.CommonUtil.CreateNoncestr(16));//随机字符串，必填项，不长于 32 位
            requestHandler.setParameter("charset", "UTF-8");//字符集
            requestHandler.setParameter("sign_type", "RSA_1_256");//签名方式
            requestHandler.setParameter("is_raw", "0");//原生JS值
            requestHandler.setParameter("is_minipg", "1");//表示小程序支付
            requestHandler.setParameter("device_info", "");//终端设备号
            requestHandler.setParameter("sub_appid", payinfo.AppId);
            requestHandler.setParameter("sub_openid", sub_openid);//测试账号不传值,此处默认给空值。正式账号必须传openid值，获取openid值指导文档地址：http://www.cnblogs.com/txw1958/p/weixin76-user-info.html
            //requestHandler.setParameter("callback_url", "https://www.swiftpass.cn");//前台地址  交易完成后跳转的 URL，需给绝对路径，255字 符 内 格 式如:http://wap.tenpay.com/callback.asp
            //requestHandler.setParameter("goods_tag", "");//商品标记                
            requestHandler.CreateSign();//创建签名

            string data = toXml(requestHandler.getAllParameters());//生成XML报文
            var result = Post(swifpasskey.ReqUrl, data);
            //Hashtable param = requestHandler.getAllParameters();
            if (!result.isTenpaySign())
            {
                if (result.Status != 0 || result.ResultCode != 0)
                    throw new Exception($"错误代码：{result.Status},错误信息：{result.Message}");

                return result.PayInfo;
            }
            else
            {
                throw new Exception($"状态：{result.Status},信息：{result.Message}");
            }
        }

        public override PayResponseBaseHandler GetPayResult(MemberTradeForRechange rechange)
        {
            var requestHandler = new SwiftpassPayRequestHandler(swifpasskey);
            requestHandler.setParameter("out_trade_no", rechange.TradeNo);//商户订单号

            requestHandler.setParameter("transaction_id", rechange.MechanismTradeNo);//平台订单号                
            requestHandler.setParameter("service", "unified.trade.query");//接口 unified.trade.query 
            requestHandler.setParameter("mch_id", payinfo.MchId);//必填项，商户号，由平台分配
            requestHandler.setParameter("version", options.Version);//接口版本号
            requestHandler.setParameter("sign_type", "RSA_1_256");//签名方式

            requestHandler.setParameter("nonce_str", Common.CommonUtil.CreateNoncestr(16));//随机字符串，必填项，不长于 32 位
            requestHandler.CreateSign();//创建签名
            //_logger.LogInformation("GetPayResult result 签名成功");

            string data = toXml(requestHandler.getAllParameters());//生成XML报文

            //_logger.LogInformation("GetPayResult result 生成XML报文：" + data);

            var result = Post(swifpasskey.ReqUrl, data);
            //Hashtable param = requestHandler.getAllParameters();
            //if (result.isTenpaySign())
            //{
            if (result.Status != 0 || result.ResultCode != 0)
                throw new Exception($"错误代码2：{result.ErrCode},错误信息2：{result.ErrMsg}");

            return result;
            //}
            //else
            //{
            throw new Exception($"错误代码1：{result.Status},错误信息1：{result.Message}");
            //}
        }

        public override object Refund(MemberTradeForRefund refund)
        {
            var requestHandler = new SwiftpassPayRequestHandler(swifpasskey);
            requestHandler.setParameter("out_trade_no", refund.TradeNo);//商户订单号
            requestHandler.setParameter("total_fee", refund.TotalFee.ToString());//总金额
            requestHandler.setParameter("refund_fee", refund.TotalFee.ToString());//退款金额
            requestHandler.setParameter("out_refund_no", refund.RefundTradeNo);//商户退款单号
            requestHandler.setParameter("service", "unified.trade.refund");//接口类型：pay.weixin.jspay
            requestHandler.setParameter("mch_id", payinfo.MchId);//必填项，商户号，由平台分配
            requestHandler.setParameter("version", "2.0");//接口版本号 
            requestHandler.setParameter("nonce_str", Common.CommonUtil.CreateNoncestr(16));//随机字符串，必填项，不长于 32 位
            requestHandler.setParameter("charset", "UTF-8");//字符集
            requestHandler.setParameter("sign_type", "RSA_1_256");//签名方式
            requestHandler.setParameter("op_user_id", payinfo.MchId);//必填项，操作员帐号,默认为商户号
            requestHandler.CreateSign();//创建签名

            string data = toXml(requestHandler.getAllParameters());//生成XML报文
            return Post(swifpasskey.ReqUrl, data);
        }

        public override PayResponseBaseHandler GetRefundResult(MemberTradeForRefund refund)
        {
            var requestHandler = new SwiftpassPayRequestHandler(swifpasskey);
            requestHandler.setParameter("out_trade_no", refund.TradeNo);//商户订单号

            requestHandler.setParameter("transaction_id", refund.MechanismTradeNo);//平台订单号                
            requestHandler.setParameter("service", "unified.trade.refundquery");//接口 unified.trade.query 
            requestHandler.setParameter("mch_id", payinfo.MchId);//必填项，商户号，由平台分配
            requestHandler.setParameter("version", "2.0");//接口版本号
            requestHandler.setParameter("sign_type", "RSA_1_256");//签名方式
            requestHandler.setParameter("nonce_str", Common.CommonUtil.CreateNoncestr(16));//随机字符串，必填项，不长于 32 位
            requestHandler.CreateSign();//创建签名
            //_logger.LogInformation("GetPayResult result 签名成功");

            string data = toXml(requestHandler.getAllParameters());//生成XML报文

            //_logger.LogInformation("GetPayResult result 生成XML报文：" + data);

            var result = Post(swifpasskey.ReqUrl, data);
            //Hashtable param = requestHandler.getAllParameters();

            if (result.Status != 0 || result.ResultCode != 0)
                throw new Exception($"错误代码2：{result.ErrCode},错误信息2：{result.ErrMsg}");

            return result;

        }
        /// <summary>
        /// 生成支付信息（APP）
        /// </summary>
        /// <param name="rechange"></param>
        /// <param name="appid"></param>
        /// <returns></returns>
        public override string GetPayAppInfo(MemberTradeForRechange rechange, string appid)
        {
            var requestHandler = new SwiftpassPayRequestHandler(swifpasskey);
            requestHandler.setParameter("out_trade_no", rechange.TradeNo);//商户订单号
            requestHandler.setParameter("body", rechange.Detail);//商品描述
            requestHandler.setParameter("attach", $"{payinfo.ShopFlag}");//附加信息
            requestHandler.setParameter("total_fee", rechange.TotalFee.ToString());//总金额
            requestHandler.setParameter("mch_create_ip", rechange.AddIP);//终端IP
            requestHandler.setParameter("time_start", rechange.AddTime.ToString("yyyyMMddHHmmss")); //订单生成时间
            requestHandler.setParameter("time_expire", rechange.AddTime.AddHours(1).ToString("yyyyMMddHHmmss"));//订单超时时间
            requestHandler.setParameter("service", "pay.weixin.raw.app");//接口类型：pay.weixin.raw.app
            requestHandler.setParameter("mch_id", payinfo.MchId);//必填项，商户号，由平台分配
            requestHandler.setParameter("version", "2.0");//接口版本号
            requestHandler.setParameter("notify_url", swifpasskey.Notify);
            //通知地址，必填项，接收平台通知的URL，需给绝对路径，255字符内;此URL要保证外网能访问   
            requestHandler.setParameter("nonce_str", Common.CommonUtil.CreateNoncestr(16));//随机字符串，必填项，不长于 32 位
            requestHandler.setParameter("charset", "UTF-8");//字符集
            requestHandler.setParameter("sign_type", "RSA_1_256");//签名方式
            requestHandler.setParameter("appid", appid);
          //  requestHandler.setParameter("sub_openid", sub_openid);//测试账号不传值,此处默认给空值。正式账号必须传openid值，获取openid值指导文档地址：http://www.cnblogs.com/txw1958/p/weixin76-user-info.html
            //requestHandler.setParameter("callback_url", "https://www.swiftpass.cn");//前台地址  交易完成后跳转的 URL，需给绝对路径，255字 符 内 格 式如:http://wap.tenpay.com/callback.asp
            //requestHandler.setParameter("goods_tag", "");//商品标记                
            requestHandler.CreateSign();//创建签名

            string data = toXml(requestHandler.getAllParameters());//生成XML报文
            var result = Post(swifpasskey.ReqUrl, data);
            //Hashtable param = requestHandler.getAllParameters();
            if (!result.isTenpaySign())
            {
                if (result.Status != 0 || result.ResultCode != 0)
                    throw new Exception($"错误代码：{result.Status},错误信息：{result.ErrMsg}");

                return result.PayInfo;
            }
            else
            {
                throw new Exception($"状态：{result.Status},信息：{result.ErrMsg}");
            }
        }
    }

    public class PayInfo
    {

    }
}
