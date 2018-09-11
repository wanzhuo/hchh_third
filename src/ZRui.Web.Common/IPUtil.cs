using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;

namespace ZRui.Web.Common
{
    public class IPUtil
    {
        public static bool IsIP(string IP)
        {
            return Regex.IsMatch(IP, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        public static bool IsIpSection(string IPSection)
        {
            Regex r = new Regex(@"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)/(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
            var m = r.Match(IPSection);
            if (!m.Success) return false;
            var ipstart = int.Parse(m.Groups[3].Value);
            int mash = int.Parse(m.Groups[4].Value);
            //获得每一段的主机数，即全部1的位数-网络为1的位数的2的幕
            var hostTotle = Math.Pow(2, 32 - mash);
            //判断开始ip是否是主机数的倍数
            if (ipstart % hostTotle != 0) return false;

            return true;
        }

        public static string ParseIPSectionToIPSection(string ipSection)
        {
            IList<string> ips = new List<string>();
            if (ipSection.IndexOf('/') >= 0)
            {
                string[] ipsec = ipSection.Split('/');
                long ipstart = ChangeToIpNum(ipsec[0]);

                var count = Convert.ToInt64(Math.Pow(2, 32 - int.Parse(ipsec[1]))) - 1;
                return $"{ChangeToIp(ipstart)}-{ChangeToIp(ipstart + count)}";
            }
            else if (ipSection.IndexOf('-') >= 0)
            {
                return ipSection;
            }

            return ipSection;
        }

        public static IList<string> ParseIPSectionToIPs(string ipSection)
        {
            IList<string> ips = new List<string>();
            if (ipSection.IndexOf('/') >= 0)
            {
                string[] ipsec = ipSection.Split('/');
                long ipstart = ChangeToIpNum(ipsec[0]);

                var count = Convert.ToInt64(Math.Pow(2, 32 - int.Parse(ipsec[1]))) - 1;
                for (int i = 1; i <= count; i++)
                {
                    ips.Add(ChangeToIp(ipstart + i));
                }
            }
            else if (ipSection.IndexOf('-') >= 0)
            {
                string[] ipsc = ipSection.Split('-');
                if (ipsc.Length == 2)
                {//长度为2 并且都是IP，则
                    if (IsIP(ipsc[0]) && IsIP(ipsc[1]))
                    {
                        ips = ParseIPSectionToIPs(ipsc[0], ipsc[1]);
                    }
                }
            }

            return ips;
        }

        public static IList<string> ParseIPSectionToIPs(string startIp, string endIp)
        {
            IList<string> ips = new List<string>();

            long ipstart = ChangeToIpNum(startIp);
            long ipend = ChangeToIpNum(endIp);
            if (ipend >= ipstart)
            {
                for (int i = 0; i <= (ipend - ipstart); i++)
                {
                    ips.Add(ChangeToIp(ipstart + i));
                }
            }
            return ips;
        }

        /// <summary>
        /// 在指定的IP段列表中，是否存在指定的IP
        /// </summary>
        /// <param name="ipSections"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool HasIp(string[] ipSections, string ip)
        {
            if (ipSections == null) return false;
            var isValidIp = false;
            foreach (var ipSection in ipSections)
            {
                if (HasIp(ipSection,ip))
                {
                    isValidIp = true;
                    break;
                }
            }
            return isValidIp;
        }
        /// <summary>
        /// 指定的IP段中，是否存在指定的IP
        /// </summary>
        /// <param name="ipSection"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool HasIp(string ipSection,string ip)
        {
            if (!IsIP(ip)) return false;
            long startNumber = 0, endNumber = 0;
            if (ipSection.IndexOf('/') >= 0)
            {
                string[] ipsec = ipSection.Split('/');
                startNumber = ChangeToIpNum(ipsec[0]);
                var count = Convert.ToInt64(Math.Pow(2, 32 - int.Parse(ipsec[1]))) - 1;
                endNumber = startNumber + count;
            }
            else if (ipSection.IndexOf('-') >= 0)
            {
                string[] ipsec = ipSection.Split('-');
                if (ipsec.Length == 2)
                {//长度为2 并且都是IP，则
                    if (IsIP(ipsec[0]) && IsIP(ipsec[1]))
                    {
                        startNumber = ChangeToIpNum(ipsec[0]);
                        endNumber = ChangeToIpNum(ipsec[1]);
                    }
                }
            }
            var ipNumber = ChangeToIpNum(ip);
            return ipNumber >= startNumber && ipNumber <= endNumber;
        }

        public static long ChangeToIpNum(string ip)
        {
            string[] sitem = ip.Split('.');
            if (sitem.Length != 4) return 0;
            Byte[] item = new Byte[4];
            for (int i = 0; i < sitem.Length; i++)
            {
                item[i] = Byte.Parse(sitem[i]);
            }
            long ipNum = item[3];//	 ip|item[1]<<16|item[2]<<8|item[0];   
            ipNum |= (long)item[2] << 8;
            ipNum |= (long)item[1] << 16;
            ipNum |= (long)item[0] << 24;
            return ipNum;
        }

        public static string ChangeToIp(long ipNum)
        {
            IPAddress b = new IPAddress(ipNum);
            string[] arrip = b.ToString().Split('.');
            string numtoip = "";
            for (int i = arrip.Length - 1; i > -1; i--)
            {
                if (i == arrip.Length - 1)
                {
                    numtoip = arrip[i].ToString();
                }
                else
                {
                    numtoip = numtoip + "." + arrip[i].ToString();
                }
            }
            return numtoip;
        }

        /// <summary>
        /// 将IP字符串转变为long型 系统
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static long ChangeToIpNumForLow(string ip)
        {
            string[] sitem = ip.Split('.');
            if (sitem.Length != 4) return 0;
            Byte[] item = new Byte[4];
            for (int i = 0; i < sitem.Length; i++)
            {
                item[i] = Byte.Parse(sitem[i]);
            }
            long ipNum = item[0];//	 ip|item[1]<<16|item[2]<<8|item[0];   
            ipNum |= (long)item[1] << 8;
            ipNum |= (long)item[2] << 16;
            ipNum |= (long)item[3] << 24;
            return ipNum;
        }
        /// <summary>
        /// 将一个long型转变为IP字符传 系统
        /// </summary>
        public static string ChangeToIpForLow(long ipNum)
        {
            IPAddress b = new IPAddress(ipNum);
            return b.ToString();
        }
    }
}
