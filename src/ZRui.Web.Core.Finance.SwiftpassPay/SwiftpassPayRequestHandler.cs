using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml;
using ZRui.Web.Common;

namespace ZRui.Web.Core.Finance.SwiftpassPay
{
    public class SwiftpassPayRequestHandler
    {
        SwiftpassKey key;
        Encoding encoding = Encoding.UTF8;

        public SwiftpassPayRequestHandler(SwiftpassKey key)
        {
            this.key = key;
            parameters = new Hashtable();
        }
        /// <summary>
        /// 请求的参数
        /// </summary>
        protected Hashtable parameters;

        /// <summary>
        /// debug信息
        /// </summary>
        private string debugInfo;

        /// <summary>
        /// 初始化函数
        /// </summary>
        public virtual void init()
        {
            //nothing to do
        }

        /// <summary>
        /// 获取带参数的请求URL
        /// </summary>
        /// <returns></returns>
        public virtual string getRequestURL()
        {
            this.CreateSign();

            StringBuilder sb = new StringBuilder();
            ArrayList akeys = new ArrayList(parameters.Keys);
            akeys.Sort();
            foreach (string k in akeys)
            {
                string v = (string)parameters[k];
                if (null != v && "key".CompareTo(k) != 0)
                {
                    sb.Append(k + "=" + HttpUtility.UrlEncode(v, encoding) + "&");
                }
            }

            //去掉最后一个&
            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }

            return key.ReqUrl + "?" + sb.ToString();
        }

        /// <summary>
        ///创建md5摘要,规则是:按参数名称a-z排序,遇到空值的参数不参加签名。
        /// </summary>
        public virtual void CreateSign()
        {
            StringBuilder sb = new StringBuilder();
            string mchPrivateKey = string.Empty;
            string sign = string.Empty;
            HashAlgorithmName? halg = null;//RSA类别
            ArrayList akeys = new ArrayList(parameters.Keys);
            akeys.Sort();
            string signtype = string.Empty;
            foreach (string maink in akeys)
            {
                string valuek = (string)parameters[maink];
                if (maink == "sign_type")
                {
                    signtype = valuek;
                }
            }
            //如果不是RSA签名类型就是MD5签名算法
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
            //    sign = MD5Util.GetMD5(sb.ToString(), getCharset()).ToUpper();
            //}
            //else
            //{
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
                halg = HashAlgorithmName.SHA1;
            if (signtype == "RSA_1_256")
                halg = HashAlgorithmName.SHA256;

            mchPrivateKey = RSAConverter.RSAPrivateKeyJava2DotNet(key.PrviateKey);
            sign = RSAHelper.SignData(mchPrivateKey, sb.ToString(), halg);
            //}

            this.setParameter("sign", sign);
            //debug信息
            this.setDebugInfo(sb.ToString() + " => sign:" + sign);
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
        public String getDebugInfo()
        {
            return debugInfo;
        }

        /// <summary>
        /// 设置debug信息
        /// </summary>
        /// <param name="debugInfo"></param>
        public void setDebugInfo(String debugInfo)
        {
            this.debugInfo = debugInfo;
        }

        /// <summary>
        /// 获取所有参数
        /// </summary>
        /// <returns></returns>
        public Hashtable getAllParameters()
        {
            return this.parameters;
        }
    }
}
