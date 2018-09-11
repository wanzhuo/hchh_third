using Newtonsoft.Json;
using Senparc.Weixin.Open;
using Senparc.Weixin.Open.Entities.Request;
using Senparc.Weixin.Open.MessageHandlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HuiChiHuiHe.Auth
{
    public class HchhThirdPartyMessageHandler: ThirdPartyMessageHandler
    {
        ShopDbContext db;
        HchhLogDbContext log;
        public HchhThirdPartyMessageHandler(Stream input, PostModel encryptPostModel, ShopDbContext db,HchhLogDbContext log)
        : base(input, encryptPostModel)
        {
            this.db = db;
            this.log = log;
        }

        public override string OnComponentVerifyTicketRequest(RequestMessageComponentVerifyTicket requestMessage)
        {
            return base.OnComponentVerifyTicketRequest(requestMessage);
        }

        public override string OnUnauthorizedRequest(RequestMessageUnauthorized requestMessage)
        {
            //取消授权
            try
            {
                var authorizerAppid = requestMessage.AuthorizerAppid;
                var authorizer = db.Query<WechatOpenAuthorizer>()
                    .Where(m => !m.IsDel)
                    .Where(m => m.AuthorizerAppId == authorizerAppid)
                    .FirstOrDefault();
                if (authorizer != null)
                {
                    authorizer.IsDel = true;
                    var shopAuthorizer = db.Query<ShopWechatOpenAuthorizer>()
                        .Where(m => !m.IsDel)
                        .Where(m => m.WechatOpenAuthorizerId == authorizer.Id)
                        .FirstOrDefault();
                    if (shopAuthorizer != null)
                    {
                        shopAuthorizer.IsDel = true;
                    }
                    db.SaveChanges();
                }
                return base.OnUnauthorizedRequest(requestMessage);
            }
            catch (Exception ex)
            {
                log.Add(new TaskLog() { AddTime = DateTime.Now, TaskName = "OnUnauthorizedRequest", ExeResult =  ex.Message });
                log.SaveChanges();
                return base.OnUnauthorizedRequest(requestMessage);
            }
            finally {
                log.Add(new TaskLog() { AddTime = DateTime.Now, TaskName = "OnUnauthorizedRequest", ExeResult = JsonConvert.SerializeObject(requestMessage) });
                log.SaveChanges();
            }
          
        }

    }
}
