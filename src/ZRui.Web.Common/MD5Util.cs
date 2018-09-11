using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ZRui.Web.Common
{
    public static class MD5Util
    {
        #region 签名相关
        /// <summary>
        /// 获取签名
        /// </summary>
        /// <param name="sPara"></param>
        /// <returns></returns>
        public static string GetSign(Dictionary<string, object> oPara, string key, string input_charset = "utf-8")
        {
            var sPara = FilterPara(oPara);

            string prestr = CreateLinkString(sPara);
            return Sign(prestr, key, input_charset);
        }

        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="prestr"></param>
        /// <param name="key"></param>
        /// <param name="_input_charset"></param>
        /// <returns></returns>
        public static String Sign(string prestr, string key, string _input_charset = "utf-8")
        {
            StringBuilder sb = new StringBuilder(32);
            prestr = prestr + key;
            Encoding encoding = Encoding.GetEncoding(_input_charset);
            string hash = GetMD5Hash(prestr, encoding);
            return hash;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dicArray"></param>
        /// <returns></returns>
        public static String CreateLinkString(Dictionary<string, string> dicArray)
        {
            StringBuilder prestr = new StringBuilder();

            foreach (KeyValuePair<string, string> temp in dicArray.OrderBy(m => m.Key))
            {
                prestr.Append(temp.Key + "=" + temp.Value + "&");
            }
            //去掉最後一個&字符
            int nLen = prestr.Length;
            prestr.Remove(nLen - 1, 1);
            return prestr.ToString();
        }
        #endregion

        #region 校验相关

        public static bool Verify(Dictionary<string, object> inputPara, string sign, string key)
        {
            string sign_type = inputPara.ContainsKey("sign_type") ? inputPara["sign_type"].ToString() : "MD5";
            string input_charset = inputPara.ContainsKey("input_charset") ? inputPara["input_charset"].ToString() : "utf-8";

            //过滤sign与sign_type参数
            var sPara = FilterPara(inputPara);

            // 获取返回时的签名验证结果
            bool isSign = GetSignVeryfy(sPara, sign, key, sign_type, input_charset);

            return isSign;
        }

        private static bool Verify(string prestr, string sign, string key, string _input_charset)
        {
            bool flag = false;
            string mysign = Sign(prestr, key, _input_charset);
            if (mysign == sign)
            {
                flag = true;
            }
            return flag;
        }

        /// <summary>
        /// 获取返回时的签名验证结果
        /// </summary>
        /// <param name="inputPara">通知返回参数数组</param>
        /// <param name="sign">对比的签名结果</param>
        /// <returns>签名验证结果</returns>
        private static bool GetSignVeryfy(Dictionary<string, string> sPara, string sign, string key, string sign_type, string input_charset)
        {
            string preSignStr = CreateLinkString(sPara);
            //获得签名验证结果
            bool isSgin = false;
            if (sign != null && sign != "")
            {
                switch (sign_type)
                {
                    case "MD5":
                        isSgin = Verify(preSignStr, sign, key, input_charset);
                        break;
                    default:
                        break;
                }
            }

            return isSgin;
        }

        private static bool GetSignVeryfy(string preSignStr, string sign, string key, string sign_type, string input_charset)
        {
            //获得签名验证结果
            bool isSgin = false;
            if (sign != null && sign != "")
            {
                switch (sign_type)
                {
                    case "MD5":
                        isSgin = Verify(preSignStr, sign, key, input_charset);
                        break;
                    default:
                        break;
                }
            }

            return isSgin;
        }

        /// <summary>
        /// 过滤掉sign等，并将byte[] 进行转换为md5hash
        /// </summary>
        /// <param name="dicArrayPre"></param>
        /// <returns></returns>
        public static Dictionary<string, string> FilterPara(Dictionary<string, object> dicArrayPre)
        {
            Dictionary<string, string> dicArray = new Dictionary<string, string>();
            foreach (KeyValuePair<string, object> item in dicArrayPre)
            {
                if (item.Key.ToLower() != "sign" && item.Key.ToLower() != "sign_type" && item.Key.ToLower() != "input_charset")
                {
                    if (item.Value is string && string.IsNullOrEmpty(item.Value as string))//如果是空值，则给一个empty
                    {
                        dicArray.Add(item.Key, "");
                    }
                    else
                    {
                        if (item.Value is string)
                        {
                            dicArray.Add(item.Key, item.Value as string);
                        }
                        else if (item.Value is byte[])//如果是byte[] 则获取byte[]的哈希。
                        {
                            dicArray.Add(item.Key, GetMD5Hash(item.Value as byte[]));
                        }
                    }
                }
            }
            return dicArray;
        }

        public static Dictionary<string, string> ToDictionary(string param)
        {
            Dictionary<string, string> dicArray = new Dictionary<string, string>();
            foreach (var item in param.Split(new Char[] { '&' }))
            {
                var tmp = item.Split(new Char[] { '=' });
                dicArray.Add(tmp[0], tmp[1]);
            }
            return dicArray;
        }

        #endregion

        /// <summary>
        /// MD5函数，默认使用utf-8编码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetMD5Hash(string input)
        {
            return GetMD5Hash(input, null);
        }

        /// <summary>
        /// MD5函数
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <returns>MD5结果</returns>
        public static string GetMD5Hash(string input, Encoding encoding)
        {
            if (encoding == null) encoding = Encoding.UTF8;
            var data = encoding.GetBytes(input);
            return GetMD5Hash(data);
        }

        /// <summary>
        /// MD5函数
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <returns>MD5结果</returns>
        public static string GetMD5Hash(byte[] input)
        {
            MD5 md5Hash = MD5.Create();
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(input);

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
    }
}
