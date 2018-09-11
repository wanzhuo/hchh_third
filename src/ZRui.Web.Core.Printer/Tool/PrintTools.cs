using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace ZRui.Web.Core.Printer
{
    public class PrintTools
    {

        /// <summary>
        /// 返回一周中的星期几
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string GetDayOfWeekZh(DateTime time)
        {
            string weekName = time.DayOfWeek.ToString();
            string week = "未知日期";
            switch (weekName)
            {
                case "Sunday":
                    week = "星期日";
                    break;
                case "Monday":
                    week = "星期一";
                    break;
                case "Tuesday":
                    week = "星期二";
                    break;
                case "Wednesday":
                    week = "星期三";
                    break;
                case "Thursday":
                    week = "星期四";
                    break;
                case "Friday":
                    week = "星期五";
                    break;
                case "Saturday":
                    week = "星期五";
                    break;
            }
            return week;
        }

        /// <summary>
        /// 返回time到1970.01.01的秒数
        /// </summary>
        /// <param name="time"></param>
        /// <returns>UNIX时间戳</returns>
        public static int GetTotalSeconds(DateTime time)
        {
            TimeSpan span = time - new DateTime(1970, 1, 1);
            return (int)span.TotalSeconds;
        }
        /// <summary>
        /// SHA1加密
        /// </summary>
        /// <param name="user">	飞鹅云后台注册用户名。</param>
        /// <param name="ukey">注册账号后生成的UKEY</param>
        /// <param name="stime">时间戳</param>
        /// <returns></returns>
        public static string Sha1(string user, string ukey, string stime)
        {
            var buffer = Encoding.UTF8.GetBytes(user + ukey + stime);
            var data = SHA1.Create().ComputeHash(buffer);

            var sb = new StringBuilder();
            foreach (var t in data)
            {
                sb.Append(t.ToString("X2"));
            }

            return sb.ToString().ToLower();

        }
        /// <summary>    
        /// 检测是否有Sql危险字符    
        /// </summary>    
        /// <param name="str">要判断字符串</param>    
        /// <returns>判断结果</returns>    
        public static bool IsSafeString(string str)
        {
            return !Regex.IsMatch(str, @"[-|;|,|\/|||
|
|\}|\{|%|@|\*|!|\']");
        }
        public static string StripSQLInjection(string sql)
        {
            if (!string.IsNullOrEmpty(sql))
            {
                //过滤 ' --    
                string pattern1 = @"(\%27)|(\')|(\-\-)";

                //防止执行 ' or    
                string pattern2 = @"((\%27)|(\'))\s*((\%6F)|o|(\%4F))((\%72)|r|(\%52))";

                //防止执行sql server 内部存储过程或扩展存储过程    
                string pattern3 = @"\s+exec(\s|\+)+(s|x)p\w+";

                sql = Regex.Replace(sql, pattern1, string.Empty, RegexOptions.IgnoreCase);
                sql = Regex.Replace(sql, pattern2, string.Empty, RegexOptions.IgnoreCase);
                sql = Regex.Replace(sql, pattern3, string.Empty, RegexOptions.IgnoreCase);
            }
            return sql;
        }
        public static bool CheckBadStr(string strString)
        {
            bool outValue = false;
            if (strString != null && strString.Length > 0)
            {
                string[] bidStrlist = new string[9];
                bidStrlist[0] = "'";
                bidStrlist[1] = ";";
                bidStrlist[2] = ":";
                bidStrlist[3] = "%";
                bidStrlist[4] = "@";
                bidStrlist[5] = "&";
                bidStrlist[6] = "#";
                bidStrlist[7] = "\"";
                bidStrlist[8] = "net user";
                bidStrlist[9] = "exec";
                bidStrlist[10] = "net localgroup";
                bidStrlist[11] = "select";
                bidStrlist[12] = "asc";
                bidStrlist[13] = "char";
                bidStrlist[14] = "mid";
                bidStrlist[15] = "insert";
                bidStrlist[19] = "order";
                bidStrlist[20] = "exec";
                bidStrlist[21] = "delete";
                bidStrlist[22] = "drop";
                bidStrlist[23] = "truncate";
                bidStrlist[24] = "xp_cmdshell";
                bidStrlist[25] = "<";
                bidStrlist[26] = ">";
                string tempStr = strString.ToLower();
                for (int i = 0; i < bidStrlist.Length; i++)
                {
                    if (tempStr.IndexOf(bidStrlist[i]) != -1)
                    //if (tempStr == bidStrlist[i])    
                    {
                        outValue = true;
                        break;
                    }
                }
            }
            return outValue;
        }
    }
}
