using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ZRui.Web.Core.Wechat
{
    public class MemberWechat:EntityBase
    {
        public virtual int MemberId { get; set; }
        public virtual string OpenId { get; set; }
    }
    /// <summary>
    /// 商铺会员
    /// </summary>
    public class ShopMember : EntityBase
    {
   
        /// <summary>
        /// 关联商铺ID
        /// </summary>
        public int ShopId { get; set; }
        /// <summary>
        /// 关联会员
        /// </summary>
        [ForeignKey("MemberId")]
        public Member Member { get; set; }
        /// <summary>
        /// 关联会员ID
        /// </summary>
        public int MemberId { get; set; }
        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; set; }
        /// <summary>
        /// 手机
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 积分
        /// </summary>
        public int Credits { get; set; }
        /// <summary>
        /// 余额
        /// </summary>
        public int Balance { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddTime { get; set; }
        /// <summary>
        /// 生日
        /// </summary>
        public DateTime BirthDay { get; set; }
        /// <summary>
        /// 添加时的Ip
        /// </summary>
        public string AddIp { get; set; }
    }
    /// <summary>
    /// 拼团订单表
    /// </summary>
    public class ConglomerationOrder : EntityBase
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderNumber { get; set; }

        /// <summary>
        /// 发货类型（1自提，2快递）
        /// </summary>
        public ConsignmentType Type { get; set; }


        /// <summary>
        /// 已发起的拼团FK_Id
        /// </summary>
        public int ConglomerationSetUpId { get; set; }

  
        /// <summary>
        /// 提货码
        /// </summary>
        public string PickupCode { get; set; }



        /// <summary>
        /// 应付金额(不包含优惠)
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// 实际支付金额(最终支付，包含所有费用)
        /// </summary>
        public int? Payment { get; set; }

        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTime? PayTime { get; set; }

        /// <summary>
        /// 退款时间
        /// </summary>
        public DateTime RefundTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime FinishTime { get; set; }



        /// <summary>
        /// 关联的商铺fK_Id
        /// </summary>
        public int ShopId { get; set; }


        /// <summary>
        /// 关联的用户Id
        /// </summary>
        public int MemberId { get; set; }

        /// <summary>
        /// 添加者用户名
        /// </summary>
        public string AddUser { get; set; }
        /// <summary>
        /// 添加时的Ip
        /// </summary>
        public string AddIp { get; set; }


        /// <summary>
        /// 拼团活动KF_Id
        /// </summary>
        public int ConglomerationActivityId { get; set; }




        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public ShopOrderStatus Status { get; set; }


        /// <summary>
        /// 快递配配送信息外键
        /// </summary>
        public int ConglomerationExpressId { get; set; }



        /// <summary>
        /// 自提时间
        /// </summary>
        public DateTime? Delivery { get; set; }

        /// <summary>
        /// 是否推送
        /// </summary>
        public bool IsSend { get; set; }



        /// <summary>
        /// 小程序提交的formId
        /// </summary>
        public string FormId { get; set; }
    }


    public enum ConsignmentType
    {
        自提 = 1,
        快递 = 2
    }
    /// <summary>
    /// 商铺订单类型
    /// </summary>
    public enum ShopOrderStatus
    {

        待成团 = 10,
        待自提 = 20,
        待配送 = 30,
        已完成 = 100,
        已取消 = -1,
        已确认 = 50,
        已退款 = -2,
        未处理 = 0,

        已支付 = 60,
        待支付 = 70,

        //发票相关
        已打印 = 61,
        加急申请中 = 62,
        申请发票 = 63,
        发票待领取 = 64,
        已领取 = 65,

        //外卖相关
        配送中 = 66,
        待取消 = 67,
        退款审批 = 68,
        退款中 = 69,
        自提中 = 71
    }


    public class MemberWechatQueryCondition
    {
        public string OpenId { get; set; }
    }

    public static class MemberechatDbContextExtention
    {
        public static MemberWechat AddToMemberWechat(this DbContext context, MemberWechat model)
        {
            context.Set<MemberWechat>().Add(model);
            return model;
        }

        public static MemberWechat GetSingleMemberWechat(this DbContext context, int id)
        {
            return context.Set<MemberWechat>().Where(m => m.Id == id).FirstOrDefault();
        }

        public static MemberWechat GetSingleMemberWechat(this DbContext context, string openId)
        {
            return context.Set<MemberWechat>().Where(m => m.OpenId == openId).FirstOrDefault();
        }

        public static EntityEntry<MemberWechat> DeleteMemberWechat(this DbContext context, int id)
        {
            var model = context.GetSingleMemberWechat(id);
            if (model != null)
            {
                return context.Set<MemberWechat>().Remove(model);
            }
            else
            {
                return null;
            }
        }

        public static IQueryable<MemberWechat> QueryMemberWechat(this DbContext context)
        {
            return context.Set<MemberWechat>().AsQueryable();
        }

        public static DbSet<MemberWechat> MemberWechatDbSet(this DbContext context)
        {
            return context.Set<MemberWechat>();
        }
    }
}
