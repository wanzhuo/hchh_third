using System;
using System.Collections;
using System.Text;
using System.Xml;

namespace ZRui.Web.Core.Finance.PayBase
{

    public class PayResponseBaseHandler
    {
        Encoding encoding = Encoding.UTF8;
        public string Xml { get; set; }

        public PayResponseBaseHandler() { }

        public PayResponseBaseHandler(string xmlContent)
        {
            SetXmlContent(xmlContent);
        }

        public void SetXmlContent(string xmlContent)
        {
            parameters = new Hashtable();
            setContent(xmlContent);
        }

        public string Mchid
        {
            get
            {
                return parameters["mch_id"] + "";
            }
        }
        /// <summary>
        /// 微信返回的随机字符串
        /// </summary>
        public string NonceStr
        {
            get
            {
                return parameters["nonce_str"] + "";
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
                return int.Parse(parameters["total_fee"].ToString());
            }
        }

        public string ReturnCode
        {
            get
            {
                return parameters["return_code"].ToString();
            }
        }



        public string ResultCode
        {
            get
            {
                return parameters["result_code"].ToString();
            }
        }

        public string Message
        {
            get
            {
                return parameters["return_msg"] + "";
            }
        }

        public string ErrCode
        {
            get
            {
                return parameters["err_code"] + "";
            }
        }

        /// <summary>
        /// 调用接口提交的小程序ID
        /// </summary>
        public string Appid
        {
            get
            {
                return parameters["appid"] + "";
            }
        }


        public string ErrMsg
        {
            get
            {
                return parameters["err_msg"] + "";
            }
        }

        public virtual object PayInfo
        {
            get
            {
                return new
                {
                    pay_info = parameters["pay_info"] + ""
                };
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


        /// <summary>
        /// 是否平台签名
        /// </summary>
        /// <returns></returns>
        public virtual Boolean isTenpaySign(Func<Hashtable, string> signMethod)
        {
            string sign = signMethod(parameters);
            bool right_sign = getParameter("sign").ToUpper().Equals(sign);
            return right_sign;
        }

        /// <summary>
        /// 应答的参数
        /// </summary>
        protected Hashtable parameters;

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
        /// 获取返回的所有参数
        /// </summary>
        /// <returns></returns>
        public Hashtable getAllParameters()
        {
            return this.parameters;
        }
    }
}
