using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Senparc.Weixin.MP;

namespace ZRui.Web.Core.Wechat
{
    public class RequestMsgData
    {
        public int Id { get; set; }
        public string ToUserName { get; set; }
        public string FromUserName { get; set; }
        public DateTime CreateTime { get; set; }
        public RequestMsgType MsgType { get; set; }
        public long MsgId { get; set; }
        public string Content { get; set; }
        public string Xml { get; set; }
        public RequestMsgDataStatus Status { get; set; }
    }


    public enum RequestMsgDataStatus
    {
        未处理,
        已处理
    }


    public class RequestMsgDataQueryCondition
    {
        public string OpenId { get; set; }
    }

    public static class RequestMsgDataDbContextExtention
    {
        public static RequestMsgData AddToRequestMsgData(this DbContext db, RequestMsgData model)
        {
            db.Set<RequestMsgData>().Add(model);
            //判断接入表
            var session = db.QueryCustomerSession()
            .Where(m => m.OpenId == model.FromUserName)
            .FirstOrDefault();
            if (session == null)
            {
                //未接入
                //添加一条接入纪录
                session = new CustomerSession()
                {
                    OpenId = model.FromUserName,
                    Time = model.CreateTime,
                    Worker = "",
                    Status = CustomerSessionStatus.未接入
                };
                db.AddToCustomerSession(session);
            }
            //更新时间
            session.Time = DateTime.Now;
            //插入到客服的聊天纪录中
            var msg = new CustomerMessage()
            {
                FromUser = model.FromUserName,
                Time = model.CreateTime,
                ToUser = session.Worker,//如果为关闭或者未接入，这里是空
                Content = model.Content,
                ChatFlag = model.FromUserName //这里使用OpenId作为交流的标识
            };
            db.AddToCustomerMessage(msg);

            //客服接入
            if (session.Status == CustomerSessionStatus.客服接入)
            {
                
            }
            else if (session.Status == CustomerSessionStatus.程序接入)
            {
                //暂时未实现
            }
            else if (session.Status == CustomerSessionStatus.未接入)
            {
                //暂时未实现
            }
            else if (session.Status == CustomerSessionStatus.关闭)
            {
                //重启为未接入
                session.Status = CustomerSessionStatus.未接入;
            }
            //程序接入
            return model;
        }

        public static RequestMsgData GetSingleRequestMsgData(this DbContext context, int id)
        {
            return context.Set<RequestMsgData>().Where(m => m.Id == id).FirstOrDefault();
        }

        public static EntityEntry<RequestMsgData> DeleteRequestMsgData(this DbContext context, int id)
        {
            var model = context.GetSingleRequestMsgData(id);
            if (model != null)
            {
                return context.Set<RequestMsgData>().Remove(model);
            }
            else
            {
                return null;
            }
        }

        public static IQueryable<RequestMsgData> QueryRequestMsgData(this DbContext context)
        {
            return context.Set<RequestMsgData>().AsQueryable();
        }

        public static DbSet<RequestMsgData> RequestMsgDataDbSet(this DbContext context)
        {
            return context.Set<RequestMsgData>();
        }
    }
}