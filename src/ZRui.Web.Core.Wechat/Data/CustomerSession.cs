using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ZRui.Web.Core.Wechat
{
    public class CustomerSession
    {
        public long Id { get; set; }
        public string OpenId { get; set; }
        public DateTime Time { get; set; }
        public virtual string Worker { get; set; }
        public CustomerSessionStatus Status { get; set; }
    }


    public enum CustomerSessionStatus
    {
        未接入 = 0,
        程序接入 = 1000,
        客服接入 = 2000,
        关闭 = -1
    }


    public class CustomerSessionQueryCondition
    {
        public string OpenId { get; set; }
    }

    public static class CustomerSessionDbContextExtention
    {
        public static CustomerSession AddToCustomerSession(this DbContext context, CustomerSession model)
        {
            context.Set<CustomerSession>().Add(model);
            return model;
        }

        public static CustomerSession GetSingleCustomerSession(this DbContext context, int id)
        {
            return context.Set<CustomerSession>().Where(m => m.Id == id).FirstOrDefault();
        }

        public static EntityEntry<CustomerSession> DeleteCustomerSession(this DbContext context, int id)
        {
            var model = context.GetSingleCustomerSession(id);
            if (model != null)
            {
                return context.Set<CustomerSession>().Remove(model);
            }
            else
            {
                return null;
            }
        }

        public static IQueryable<CustomerSession> QueryCustomerSession(this DbContext context)
        {
            return context.Set<CustomerSession>().AsQueryable();
        }

        public static DbSet<CustomerSession> CustomerSessionDbSet(this DbContext context)
        {
            return context.Set<CustomerSession>();
        }

        public static void EnsureIsCustomerSessionWorker(this DbContext context, string openId, string worker)
        {
            var customerSession = context.QueryCustomerSession()
            .Where(m => m.OpenId == openId)
            .FirstOrDefault();
            if (customerSession == null) throw new Exception("openId是否有误？未有接入信息");
            if (customerSession.Status != CustomerSessionStatus.客服接入)
            {
                throw new Exception("未接入，不能发送信息");
            }

            if (customerSession.Worker != worker)
            {
                throw new Exception("不是你接入，不能发送信息");
            }
        }

        public static void UpdateCustomerSessionTime(this DbContext context, string openId)
        {
            var customerSession = context.QueryCustomerSession()
            .Where(m => m.OpenId == openId)
            .FirstOrDefault();
            if (customerSession != null)
            {
                customerSession.Time = DateTime.Now;
            }
        }
    }
}