using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.IO;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZRui.Web.Core.Wechat
{
    public class CustomerMessage
    {
        public virtual long Id { get; set; }
        public virtual string FromUser { get; set; }
        public virtual string ToUser { get; set; }
        public virtual string Content { get; set; }
        [NotMapped]
        public virtual object ContentObject
        {
            get
            {
                if (Content.StartsWith("{"))
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject(Content);
                }
                else
                {
                    return new { };
                }
            }
            set
            {
                Content = Newtonsoft.Json.JsonConvert.SerializeObject(value);
            }
        }
        public virtual DateTime Time { get; set; }
        /// <summary>
        /// 聊天标识，在这里通常使用OpenId作为标识，来表示跟某一个客户聊天的纪录
        /// </summary>
        /// <returns></returns>
        public virtual string ChatFlag { get; set; }
        /// <summary>
        /// 用户是否已经读过
        /// </summary>
        public bool MemberIsRead { get; set; }

        public static string GetCachePathForTemporaryMedia(string mediaId)
        {
            var folderPath = $"{Directory.GetCurrentDirectory()}/App_Data/Weixin/TemporaryMedia";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            var fileName = $"{folderPath}/{mediaId}.jpg";

            return fileName;
        }

        public static string GetCacheSmallPathForTemporaryMedia(string mediaId)
        {
            var folderPath = $"{Directory.GetCurrentDirectory()}/App_Data/Weixin/TemporaryMedia";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            var fileName = $"{folderPath}/{mediaId}_small.jpg";

            return fileName;
        }
    }


    public class CustomerMessageQueryCondition
    {
        public string ChatFlag { get; set; }
    }

    public static class CustomerMessageDbContextExtention
    {
        public static CustomerMessage AddToCustomerMessage(this DbContext context, CustomerMessage model)
        {
            context.Set<CustomerMessage>().Add(model);
            return model;
        }

        public static CustomerMessage GetSingleCustomerMessage(this DbContext context, int id)
        {
            return context.Set<CustomerMessage>().Where(m => m.Id == id).FirstOrDefault();
        }

        public static EntityEntry<CustomerMessage> DeleteCustomerMessage(this DbContext context, int id)
        {
            var model = context.GetSingleCustomerMessage(id);
            if (model != null)
            {
                return context.Set<CustomerMessage>().Remove(model);
            }
            else
            {
                return null;
            }
        }

        public static IQueryable<CustomerMessage> QueryCustomerMessage(this DbContext context)
        {
            return context.Set<CustomerMessage>().AsQueryable();
        }

        public static DbSet<CustomerMessage> CustomerMessageDbSet(this DbContext context)
        {
            return context.Set<CustomerMessage>();
        }
    }
}