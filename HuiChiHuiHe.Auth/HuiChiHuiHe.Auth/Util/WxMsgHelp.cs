using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace HuiChiHuiHe.Auth
{
    public class WxMsgHelp
    {

        /// <summary>
        /// 解析xml
        /// </summary>
        /// <param name="context"></param>
        /// <returns>xmlelement消息对象</returns>
        public static XmlElement Parse(HttpRequest request)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(request.Body);
            return xmlDoc.DocumentElement;
        }
         



    }
}
