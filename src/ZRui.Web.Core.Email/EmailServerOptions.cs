using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZRui.Web
{
    public class EmailServerOptions
    {
        public SmtpServerSetting SmtpServer { get; set; }
        public string[] ReceiveEmailAddresss { get; set; }
    }

    public class SmtpServerSetting
    {
        public string Host { get; set; }
        public int Post { get; set; }
        public string EmailAddress { get; set; }
        public string EmailPassword { get; set; }
        public string[] ReceiveEmailAddresss { get; set; }
    }
}
