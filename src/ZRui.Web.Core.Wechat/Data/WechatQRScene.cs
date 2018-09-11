using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq;

namespace ZRui.Web.Core.Wechat
{
    public class WechatQRScene
    {
        public virtual long Id { get; set; }
        public virtual int SceneId { get; set; }
        public virtual string QrCodeTicket { get; set; }
        public virtual string Category { get; set; }
        public virtual WechatQRSceneStatus Status { get; set; }
    }

    public enum WechatQRSceneStatus
    {
        未处理 = 0,
        已处理 = 1
    }

    public class WechatQRSceneQueryCondition
    {
        public string OpenId { get; set; }
    }

    public static class WechatQRSceneDbContextExtention
    {
        public static WechatQRScene AddToWechatQRScene(this DbContext context, WechatQRScene model)
        {
            context.Set<WechatQRScene>().Add(model);
            return model;
        }

        public static WechatQRScene GetSingleWechatQRScene(this DbContext context, int id)
        {
            return context.Set<WechatQRScene>().Where(m => m.Id == id).FirstOrDefault();
        }

        public static EntityEntry<WechatQRScene> DeleteWechatQRScene(this DbContext context, int id)
        {
            var model = context.GetSingleWechatQRScene(id);
            if (model != null)
            {
                return context.Set<WechatQRScene>().Remove(model);
            }
            else
            {
                return null;
            }
        }

        public static IQueryable<WechatQRScene> QueryWechatQRScene(this DbContext context)
        {
            return context.Set<WechatQRScene>().AsQueryable();
        }

        public static DbSet<WechatQRScene> WechatQRSceneDbSet(this DbContext context)
        {
            return context.Set<WechatQRScene>(); 
        }
    }
}
