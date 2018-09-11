using MailKit.Security;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web
{
    public class GlobalContext
    {
        GlobalContextSettings setting;
        Dictionary<string, object> tempList = new Dictionary<string, object>();

        public bool HasTempValue(string key)
        {
            return tempList.ContainsKey(key);
        }

        public T GetTempValue<T>(string key)
        {
            if (tempList.ContainsKey(key))
            {
                return (T)tempList[key];
            }
            else
            {
                return default(T);
            }
        }

        public void SetTempValue<T>(string key, T value)
        {
            if (tempList.ContainsKey(key))
            {
                tempList[key] = value;
            }
            else
            {
                tempList.Add(key, value);
            }
        }


        public GlobalContext(IOptions<GlobalContextSettings> setting)
        {
            this.setting = setting.Value;
        }

        public async System.Threading.Tasks.Task SendEmailAsync(string[] receiveEmailAddresss, string subject, string body)
        {
            if (setting != null
                        && setting.SmtpServer != null
                        && receiveEmailAddresss != null
                        && receiveEmailAddresss.Length > 0)
            {
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress(setting.SmtpServer.EmailAddress));
                foreach (var email in receiveEmailAddresss)
                {
                    emailMessage.To.Add(new MailboxAddress(email));
                }
                emailMessage.Subject = subject;

                var bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = body;
                emailMessage.Body = bodyBuilder.ToMessageBody();
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(setting.SmtpServer.Host, setting.SmtpServer.Port, SecureSocketOptions.SslOnConnect).ConfigureAwait(false);
                    await client.AuthenticateAsync(setting.SmtpServer.EmailAddress, setting.SmtpServer.EmailPassword);
                    await client.SendAsync(emailMessage).ConfigureAwait(false);
                    await client.DisconnectAsync(true).ConfigureAwait(false);
                }
            }
        }
    }




}
