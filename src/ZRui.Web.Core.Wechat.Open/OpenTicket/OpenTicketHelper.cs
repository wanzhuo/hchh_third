using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Senparc.Weixin.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace ZRui.Web.Core.Wechat.Open
{
    /// <summary>
    /// OpenTicket即ComponentVerifyTicket
    /// </summary>
    public static class OpenTicketExtention
    {
        /// <summary>
        /// 获取openTicket
        /// </summary>
        /// <param name="hostingEnvironment"></param>
        /// <param name="componentAppId"></param>
        /// <param name="_contextOptions"></param>
        /// <returns></returns>
        public static string GetOpenTicket(this IHostingEnvironment hostingEnvironment, string componentAppId, DbContextOptions<WechatOpenCoreDbContext> _contextOptions)
        {

            using (var db = new WechatOpenCoreDbContext(_contextOptions))
            {
                return db.Query<WechatOpenTicket>().First().OpenTicket;

                //var model = db.Query<WechatOpenTicket>()
                //    .Where(m => m.AppId == componentAppId)
                //    .FirstOrDefault();

                //if (model == null) throw new Exception($"获取OpenTicket失败,appId为：{componentAppId}");

                //return model.OpenTicket;
            }



        }
    }
}
