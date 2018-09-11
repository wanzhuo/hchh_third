using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Xml.Linq;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Engines;
using System.Text.RegularExpressions;

namespace ZRui.Web.Common
{
    public class CommonUtil
    {
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

            MD5 md5Hash = MD5.Create();
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(encoding.GetBytes(input));

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

        public static String CreateIntNoncestr(int length)
        {
            String chars = "0123456789";
            String res = "";
            Random rd = new Random();
            for (int i = 0; i < length; i++)
            {
                res += chars[rd.Next(chars.Length - 1)];
            }
            return res;
        }

        public static String CreateNoncestr(int length)
        {
            String chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            String res = "";
            Random rd = new Random();
            for (int i = 0; i < length; i++)
            {
                res += chars[rd.Next(chars.Length - 1)];
            }
            return res;
        }

        public static String CreateNoncestr()
        {
            String chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            String res = "";
            Random rd = new Random();
            for (int i = 0; i < 16; i++)
            {
                res += chars[rd.Next(chars.Length - 1)];
            }
            return res;
        }

        public static long ToTimestamp(DateTime value)
        {
            // System.DateTime startTime = TimeZoneInfo.ConvertTime(new System.DateTime(1970, 1, 1), TimeZoneInfo.Local);
            // return (int)(value - startTime).TotalSeconds;

            var startTime = new System.DateTime(1970, 1, 1).ToLocalTime();
            return (long)(value - startTime).TotalSeconds;
        }

        public static DateTime ToDateTime(long unixTimeStamp)
        {
            //System.DateTime startTime = System.TimeZoneInfo.ConvertTime(new System.DateTime(1970, 1, 1).ToUniversalTime(), TimeZoneInfo.Local); // 当地时区
            var startTime = new System.DateTime(1970, 1, 1).ToLocalTime();
            DateTime dt = startTime.AddSeconds(unixTimeStamp);
            return dt;
        }

        public static bool IsNumeric(String str)
        {
            try
            {
                int.Parse(str);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string ArrayToXml(Dictionary<string, string> arr)
        {
            String xml = "<xml>";

            foreach (KeyValuePair<string, string> pair in arr)
            {
                String key = pair.Key;
                String val = pair.Value;
                if (IsNumeric(val))
                {
                    xml += "<" + key + ">" + val + "</" + key + ">";

                }
                else
                    xml += "<" + key + "><![CDATA[" + val + "]]></" + key + ">";
            }

            xml += "</xml>";
            return xml;
        }

        public static Dictionary<string, string> XmlToArray(string xml)
        {
            XDocument xdoc = XDocument.Parse(xml);
            var arr = new Dictionary<string, string>();
            foreach (var item in xdoc.Root.Elements())
            {
                arr.Add(item.Name.LocalName, item.Value);
            }

            return arr;
        }


        public static Dictionary<string, int> EnumToDic(Enum em)
        {
            Dictionary<string, int> itemsDic = new Dictionary<string, int>();
            Array array = Enum.GetValues(em.GetType());
            foreach (int val in array)
            {
                itemsDic.Add(Enum.GetName(em.GetType(), val), val);
            }
            return itemsDic;
        }

        public static List<EnumModel> EnumToList(Enum em)
        {
            var itemsDic = EnumToDic(em);
            var list = itemsDic.Select(m => new EnumModel
            {
                ID = m.Value,
                Name = m.Key
            })
                .ToList();
            return list;
        }

        public static string HTML2Markdown(string html)
        {
            var s = html;
            s = Regex.Replace(s, "<code>", "", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, "</code>", "", RegexOptions.IgnoreCase);

            //s = Regex.Replace(s, "<pre.*?>(.*?)</pre>", "\r\n```$1```\r\n", RegexOptions.IgnoreCase);
            var preMatches = Regex.Matches(s, @"<pre.*?>([\s\S]*?)</pre>");
            var preItems = new List<string>();
            foreach (Match match in preMatches)
            {
                preItems.Add(match.Groups[1].Value);
                s = s.Replace(match.Value, $"\r\n```\r\n--pre{preItems.Count - 1}--\r\n```\r\n");
            }

            s = Regex.Replace(s, @"<div.*?>([\s\S]*?)</div>", "\r\n$1\r\n", RegexOptions.IgnoreCase);
            for (int i = 1; i <= 6; i++)
            {
                s = Regex.Replace(s, $@"<h{i}.*?>([\s\S]*?)</h{i}>", $"\r\n{"".PadRight(i, '#')} $1\r\n", RegexOptions.IgnoreCase);
            }

            s = Regex.Replace(s, "<p.*?>", "\r\n\r\n", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, "</p>", "\r\n\r\n", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, @"<br[/|\s]*?>", "\r\n", RegexOptions.IgnoreCase);
            var matches = Regex.Matches(s, @"<img .*?>");
            foreach (Match match in matches)
            {
                var attrs = Regex.Matches(match.Value, " (.*?)=[\"|\'](.*?)[\"|\']", RegexOptions.IgnoreCase);
                var src = "";
                var alt = "";
                foreach (Match attr in attrs)
                {
                    if (attr.Groups[1].ToString().ToLower() == "src")
                    {
                        src = attr.Groups[2].Value;
                    }

                    if (attr.Groups[1].ToString().ToLower() == "alt")
                    {
                        alt = attr.Groups[2].Value;
                    }
                }

                s = s.Replace(match.Value, $"\r\n\r\n![{alt}]({src})\r\n\r\n");
            }

            s = s.Replace("\r\n\r\n\r\n\r\n", "\r\n\r\n");
            s = s.Replace("\r\n\r\n\r\n", "\r\n\r\n");

            for (int i = 0; i < preItems.Count; i++)
            {
                s = s.Replace($"--pre{i}--", $"{preItems[i]}");
            }
            
            return s;
        }


    }


    public static class StringExtention
    {
        public static string GetSubString(this string o, int startIndex, int length)
        {
            if (string.IsNullOrEmpty(o)) return o;
            if (o.Length - startIndex <= length) return o;
            return o.Substring(startIndex, length);
        }
    }

    public class EnumModel
    {
        public string Name { get; set; }
        public int ID { get; set; }
    }
}
