using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Senparc.Weixin.Open;
using Senparc.Weixin.Open.MessageHandlers;
using System.IO;
using Senparc.Weixin.Open.Entities.Request;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace ZRui.Web.Core.Wechat.ThirdPartyMessageHandlers
{
    public class CustomThirdPartyMessageHandler : ThirdPartyMessageHandler
    {
        IHostingEnvironment hostingEnvironment;
        public CustomThirdPartyMessageHandler(Stream inputStream, PostModel encryptPostModel, IHostingEnvironment hostingEnvironment)
            : base(inputStream, encryptPostModel)
        {
            this.hostingEnvironment = hostingEnvironment;
        }

        public override string OnComponentVerifyTicketRequest(RequestMessageComponentVerifyTicket requestMessage)
        {

            //使用数据库去记录
            var _contextOptions = new DbContextOptionsBuilder<WechatOpenCoreDbContext>()
                .UseMySql("Server=120.79.31.209;Port=3336;Uid=root;Pwd=628VqB2sgJwLgOvngXQ3;Database=hchh;")
                .Options;

            using (var db = new WechatOpenCoreDbContext(_contextOptions))
            {
                var model = db.Query<WechatOpenTicket>()
                    .Where(m => m.AppId == requestMessage.AppId)
                    .FirstOrDefault();

                if (model == null)
                {
                    model = new WechatOpenTicket()
                    {
                        AppId = requestMessage.AppId,
                        OpenTicket = requestMessage.ComponentVerifyTicket,
                        CreateTime = requestMessage.CreateTime
                    };
                    db.AddTo(model);
                }
                else
                {
                    model.OpenTicket = requestMessage.ComponentVerifyTicket;
                    model.CreateTime = requestMessage.CreateTime;
                }
                db.SaveChanges();
            }

            //var openTicketPath = hostingEnvironment.MapPath("/App_Data/OpenTicket");
            //if (!Directory.Exists(openTicketPath))
            //{
            //    Directory.CreateDirectory(openTicketPath);
            //}

            ////RequestDocument.Save(Path.Combine(openTicketPath, string.Format("{0}_Doc.txt", DateTime.Now.Ticks)));

            ////记录ComponentVerifyTicket（也可以存入数据库或其他可以持久化的地方）
            //using (FileStream fs = new FileStream(Path.Combine(openTicketPath, string.Format("{0}.txt", RequestMessage.AppId)),FileMode.OpenOrCreate,FileAccess.ReadWrite))
            //{
            //    using (TextWriter tw = new StreamWriter(fs))
            //    {
            //        tw.Write(requestMessage.ComponentVerifyTicket);
            //        tw.Flush();
            //        //tw.Close();
            //    }
            //}
            return base.OnComponentVerifyTicketRequest(requestMessage);
        }

        public override string OnUnauthorizedRequest(RequestMessageUnauthorized requestMessage)
        {

            //取消授权
            return base.OnUnauthorizedRequest(requestMessage);
        }
    }
}
