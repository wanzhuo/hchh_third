using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml;
using ZRui.Web.Common;
using ZRui.Web.Core.Finance.PayBase;

namespace ZRui.Web.Core.Finance.SwiftpassPay
{
    /// <summary>
    /// 客户端消息返回头
    /// </summary>
    public class SwiftpassPayResponseHandler : PayResponseBaseHandler
    {
        SwiftpassKey options;
        Encoding encoding = Encoding.UTF8;
        ShopDbContext shopDb;
        public string Xml { get; set; }
        public SwiftpassPayResponseHandler(SwiftpassKey options, string xmlContent)
        {
            this.options = options;
            this.shopDb = ZRui.Web.BLL.DbContextFactory.ShopDb;
            parameters = new Hashtable();
            setContent(xmlContent);
            //this.options = shopDb.SwiftpassKey.FirstOrDefault(r => r.ShopFlag == this.Attach);
            //if (this.options == null)
            //{
            //    throw new Exception("商家支付密钥未配置");
            //}
        }

        public string SwiftpassAppid
        {
            get
            {
                return parameters["sub_appid"] + "";
            }
        }

        public string SwiftpassMchid
        {
            get
            {
                return parameters["mch_id"] + "";
            }
        }

        public string TransactionId
        {
            get
            {
                return parameters["transaction_id"] + "";
            }
        }
        public string TradeState
        {
            get
            {
                return parameters["trade_state"] + "";
            }
        }

        public string OutTradeNo
        {
            get
            {
                return parameters["out_trade_no"] + "";
            }
        }

        public int TotalFee
        {
            get
            {
                if (parameters["total_fee"]!=null) {
                    return int.Parse(parameters["total_fee"].ToString());
                }
                return 0;
               
            }
        }

        public int Status
        {
            get
            {
                return int.Parse(parameters["status"].ToString());
            }
        }



        public int ResultCode
        {
            get
            {
                return int.Parse(parameters["result_code"].ToString());
            }
        }

        public string Message
        {
            get
            {
                return parameters["message"] + "";
            }
        }

        public string ErrCode
        {
            get
            {
                return parameters["err_code"] + "";
            }
        }

        public string ErrMsg
        {
            get
            {
                return parameters["err_msg"] + "";
            }
        }

        public string PayInfo
        {
            get
            {
                return parameters["pay_info"] + "";
            }
        }

        public string Attach
        {
            get
            {
                if (parameters["attach"]!=null) {
                    return parameters["attach"] + "";
                }
                return "";
               
            }
        }

        /// <summary>
        /// 应答的参数
        /// </summary>
        public Hashtable parameters;

        /// <summary>
        /// debug信息
        /// </summary>
        private string debugInfo;

        /// <summary>
        /// 设置返回内容
        /// </summary>
        /// <param name="content">XML内容</param>
        void setContent(string content)
        {
            this.Xml = content;
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(content);
            XmlNode root = xmlDoc.SelectSingleNode("xml");
            XmlNodeList xnl = root.ChildNodes;

            foreach (XmlNode xnf in xnl)
            {
                this.setParameter(xnf.Name, xnf.InnerText);
            }
        }

        /// <summary>
        /// 获取参数值
        /// </summary>
        /// <param name="parameter">参数名</param>
        /// <returns></returns>
        public string getParameter(string parameter)
        {
            string s = (string)parameters[parameter];
            return (null == s) ? "" : s;
        }

        /// <summary>
        /// 设置参数值
        /// </summary>
        /// <param name="parameter">参数名</param>
        /// <param name="parameterValue">参数值</param>
        public void setParameter(string parameter, string parameterValue)
        {
            if (parameter != null && parameter != "")
            {
                if (parameters.Contains(parameter))
                {
                    parameters.Remove(parameter);
                }

                parameters.Add(parameter, parameterValue);
            }
        }

        /// <summary>
        /// 是否平台签名,规则是:按参数名称a-z排序,遇到空值的参数不参加签名。
        /// </summary>
        /// <returns></returns>
        public virtual Boolean isTenpaySign()
        {
            StringBuilder sb = new StringBuilder();
            HashAlgorithmName? halg = null;//签名类别
            string signtype = string.Empty;//签名类型
            bool right_sign = false;//检验结果真假
            ArrayList akeys = new ArrayList(parameters.Keys);
            akeys.Sort();
            foreach (string item in akeys)
            {
                string myvalue = (string)parameters[item];
                if (item == "sign_type")
                {
                    signtype = myvalue;
                }
            }
            //if (signtype == "MD5")
            //{
            //    foreach (string k in akeys)
            //    {
            //        string v = (string)parameters[k];
            //        if (null != v && "".CompareTo(v) != 0
            //            && "sign".CompareTo(k) != 0 && "key".CompareTo(k) != 0)
            //        {
            //            sb.Append(k + "=" + v + "&");
            //        }
            //    }

            //    sb.Append("key=" + this.getKey());
            //    string sign = MD5Util.GetMD5(sb.ToString(), getCharset()).ToLower();
            //    this.setDebugInfo(sb.ToString() + " => sign:" + sign);
            //    right_sign = getParameter("sign").ToLower().Equals(sign);
            //}
            //else
            //{ 
            if (signtype == string.Empty)
            {
                right_sign = false;
            }
            else
            {
                int cnt = akeys.Count;
                int i = 0;
                foreach (string k in akeys)
                {
                    string v = (string)parameters[k];
                    if (null != v && "".CompareTo(v) != 0 && "sign".CompareTo(k) != 0)
                    {
                        if (i == (cnt - 1))
                        {
                            sb.Append(k + "=" + v);
                        }
                        else
                        {
                            sb.Append(k + "=" + v + "&");
                        }
                    }
                    i++;
                }
                if (signtype == "RSA_1_1")
                {
                    halg = HashAlgorithmName.SHA1;
                }
                if (signtype == "RSA_1_256")
                {
                    halg = HashAlgorithmName.SHA256;
                }
                string wftPublicKey = RSAConverter.RSAPublicKeyJava2DotNet(options.PublicKey);
                right_sign = RSAHelper.VerifyData(wftPublicKey, sb.ToString(), getParameter("sign"), halg);
            }
            //}
            return right_sign;
        }

        /// <summary>
        /// 获取debug信息
        /// </summary>
        /// <returns></returns>
        public string getDebugInfo()
        { return debugInfo; }

        /// <summary>
        /// 设置debug信息
        /// </summary>
        /// <param name="debugInfo"></param>
        protected void setDebugInfo(String debugInfo)
        { this.debugInfo = debugInfo; }

        /// <summary>
        /// 是否威富通签名,规则是:按参数名称a-z排序,遇到空值的参数不参加签名。
        /// </summary>
        /// <param name="akeys"></param>
        /// <returns></returns>
        public virtual Boolean _isTenpaySign(ArrayList akeys)
        {
            StringBuilder sb = new StringBuilder();

            foreach (string k in akeys)
            {
                string v = (string)parameters[k];
                if (null != v && "".CompareTo(v) != 0
                    && "sign".CompareTo(k) != 0 && "key".CompareTo(k) != 0)
                {
                    sb.Append(k + "=" + v + "&");
                }
            }

            sb.Append("key=" + "key");
            string sign = MD5Util.GetMD5Hash(sb.ToString(), encoding).ToLower();

            //debug信息
            this.setDebugInfo(sb.ToString() + " => sign:" + sign);
            return getParameter("sign").ToLower().Equals(sign);
        }

        /// <summary>
        /// 获取返回的所有参数
        /// </summary>
        /// <returns></returns>
        public Hashtable getAllParameters()
        {
            return this.parameters;
        }
    }
}
