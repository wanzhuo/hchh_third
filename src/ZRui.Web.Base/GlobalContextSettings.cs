using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web
{
    public class GlobalContextSettings
    {
        public SmtpServerSetting SmtpServer { get; set; }
    }

    public class SmtpServerSetting
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string EmailAddress { get; set; }
        public string EmailPassword { get; set; }
    }
}
