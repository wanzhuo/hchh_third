using System;
using System.Linq;
using ZRui.Web.EmailServerAPIModels;
using ZRui.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using MimeKit;
using MailKit.Security;
using MailKit.Net.Smtp;

namespace ZRui.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class EmailServerAPIController : CommunityApiControllerBase
    {
        readonly IHostingEnvironment hostingEnvironment;
        EmailServerOptions emailServerSetting;
        public EmailServerAPIController(ICommunityService communityService, IOptions<MemberAPIOptions> options, IOptions<EmailServerOptions> emailServerOptions, IHostingEnvironment hostingEnvironment)
            : base(communityService, options)
        {
            this.hostingEnvironment = hostingEnvironment;
            emailServerSetting = emailServerOptions.Value;
        }

        [HttpPost]
        [Authorize]
        public async System.Threading.Tasks.Task<APIResult> Send([FromBody]SendArgsModel args)
        {
            if (string.IsNullOrEmpty(args.Title)) throw new ArgumentNullException("Title");
            if (string.IsNullOrEmpty(args.To)) throw new ArgumentNullException("To");
            if (string.IsNullOrEmpty(args.Content)) throw new ArgumentNullException("Content");

            if (emailServerSetting != null
                        && emailServerSetting.SmtpServer != null)
            {
                var subject = args.Title;

                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress(emailServerSetting.SmtpServer.EmailAddress));
                emailMessage.To.Add(new MailboxAddress(args.To));

                emailMessage.Subject = subject;
                var bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = args.Content;
                emailMessage.Body = bodyBuilder.ToMessageBody();
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(emailServerSetting.SmtpServer.Host, emailServerSetting.SmtpServer.Post, SecureSocketOptions.SslOnConnect).ConfigureAwait(false);
                    await client.AuthenticateAsync(emailServerSetting.SmtpServer.EmailAddress, emailServerSetting.SmtpServer.EmailPassword);
                    await client.SendAsync(emailMessage).ConfigureAwait(false);
                    await client.DisconnectAsync(true).ConfigureAwait(false);
                }

            }

            return Success();
        }
    }
}
