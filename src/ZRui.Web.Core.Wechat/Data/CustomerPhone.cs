using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZRui.Web.Core.Wechat
{
    public class CustomerPhone
    {
        public long Id { get; set; }
        public string OpenId { get; set; }
        public string Phone { get; set; }
        public bool IsDel { get; set; }
        public CustomerPhoneStatus Status { get; set; }

    }

    public enum CustomerPhoneStatus
    {
        已绑定 = 1, 已解绑 = 2
    }


    public class CustomerPhoneQueryCondition
    {
        public string ChatFlag { get; set; }
    }

    public static class CustomerPhoneDbContextExtention
    {
        public static CustomerPhone AddToCustomerPhone(this DbContext context, CustomerPhone model)
        {
            context.Set<CustomerPhone>().Add(model);
            return model;
        }

        public static bool HasCustomerPhone(this DbContext context, string openId, string phone)
        {
            var isBindCustomerPhone = context.QueryCustomerPhone()
                                    .Where(m => m.OpenId == openId)
                                    .Where(m => m.Phone == phone)
                                    .Where(m => !m.IsDel)
                                    .Where(m => m.Status == CustomerPhoneStatus.已绑定).Count() > 0;
            return isBindCustomerPhone;
        }

        public static CustomerPhone GetSingleCustomerPhone(this DbContext context, int id)
        {
            return context.Set<CustomerPhone>().Where(m => m.Id == id).FirstOrDefault();
        }

        public static string GetOpenIdByCustomerPhone(this DbContext context, string phone)
        {
            return context.Set<CustomerPhone>().Where(m => m.Status == CustomerPhoneStatus.已绑定 && m.Phone == phone).Select(m => m.OpenId).FirstOrDefault();
        }

        public static EntityEntry<CustomerPhone> DeleteCustomerPhone(this DbContext context, int id)
        {
            var model = context.GetSingleCustomerPhone(id);
            if (model != null)
            {
                return context.Set<CustomerPhone>().Remove(model);
            }
            else
            {
                return null;
            }
        }

        public static IQueryable<CustomerPhone> QueryCustomerPhone(this DbContext context)
        {
            return context.Set<CustomerPhone>().AsQueryable();
        }

        public static DbSet<CustomerPhone> CustomerPhoneDbSet(this DbContext context)
        {
            return context.Set<CustomerPhone>();
        }


    }
}
