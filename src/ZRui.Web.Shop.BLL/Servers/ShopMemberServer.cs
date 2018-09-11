using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace ZRui.Web.BLL.Servers
{
    public class ShopMemberServer
    {
        private string _memberPasswordMD5Key = "kspig";

        private ShopMember mShopMember;
        private ShopDbContext shopDb;

        public static ShopMember GetShopMember(ShopDbContext shopDb, int shopId, int memberId)
        {
            ShopMember shopMember = shopDb.Query<ShopMember>()
                .Where(m => !m.IsDel && m.MemberId == memberId && m.ShopId == shopId)
                .FirstOrDefault();
            return shopMember;
        }

        public static ShopMemberSet GetShopMemberSet(ShopDbContext shopDb, int shopId)
            => shopDb.Query<ShopMemberSet>().FirstOrDefault(m => !m.IsDel && m.ShopId == shopId);

        public ShopMemberServer(ShopDbContext shopDb,ShopMember shopMember)
        {
            this.shopDb = shopDb;
            mShopMember = shopMember;
        }

        public ShopMemberServer(ShopDbContext shopDb, int shopId, int memberId)
        {
            this.shopDb = shopDb;
            mShopMember = GetShopMember(shopDb,shopId,memberId);
            if (mShopMember == null) throw new Exception("会员信息不存在");
        }

        //会员余额
        public int GetBalance() => mShopMember.Balance;

        /// <summary>
        /// 设置支付密码
        /// </summary>
        /// <param name="pwd"></param>
        public void SetPassword(string pwd)
        {
            Regex regex = new Regex(@"\d{6}");
            if (!regex.IsMatch(pwd)) throw new Exception("支付密码必须为6位数字");
            mShopMember.PayPassword = MemberPasswordToMD5(pwd);
        }

        //是否可以用此手机号码
        public bool CheckPhoneNumCanUse(string phone, int shopId)
            => shopDb.Query<ShopMember>()
            .Where(m => !m.IsDel && m.ShopId == shopId && m.Phone == phone)
            .FirstOrDefault() == null;

        public bool CheckPassword(string pwd)
            => mShopMember.PayPassword == MemberPasswordToMD5(pwd);

        /// <summary>
        /// 余额消费
        /// </summary>
        /// <param name="shopOrder"></param>
        public bool BalanceConsume(ShopOrder shopOrder)
        {
            if (shopOrder.PayTime.HasValue) throw new Exception("该订单已支付");
            //shopOrder.MemberDiscount = ComputeMemberDiscount(shopOrder.Id);
            //shopOrder.Payment -= shopOrder.MemberDiscount;
            if (mShopMember.Balance < shopOrder.Payment)
                return false;
            var record = AddShopMemberConsumeRecorder(shopDb,shopOrder,mShopMember.Id);
            mShopMember.Balance -= shopOrder.Payment.Value;
            shopOrder.ShopMemberConsume = record;
            return true;
        }


        /// <summary>
        /// 退款到余额
        /// </summary>
        /// <param name="amount"></param>
        public void RefundToBalance(ShopOrder shopOrder)
        {
            mShopMember.Balance += shopOrder.Payment.Value;
            ShopMemberRufund shopMemberRufund = new ShopMemberRufund()
            {
                Amount = shopOrder.Payment.Value,
                ShopMemberId = mShopMember.Id,
                ShopOrderId = shopOrder.Id,
                TransactionTime = DateTime.Now
            };
            shopDb.Add(shopMemberRufund);
        }

        /// <summary>
        /// 添加消费记录
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static ShopMemberConsume AddShopMemberConsumeRecorder(ShopDbContext shopDb, ShopOrder shopOrder, int shopMemberId)
        {
            var record = new ShopMemberConsume()
            {
                Amount = shopOrder.Payment.Value,
                ShopMemberId = shopMemberId,
                TransactionTime = DateTime.Now,
            };
            shopDb.Add(record);
            return record;
        }

        /// <summary>
        /// 计算订单会员折扣
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        public int ComputeMemberDiscount(int orderId)
        {
            var list = shopDb.Query<ShopOrderItem>()
                .Where(m => !m.IsDel && m.ShopOrderId == orderId)
                .ToList();
            var dic = new Dictionary<int, IQueryable<ShopCommodityStock>>(); 
            foreach (var item in list)
            {
                var stock = shopDb.Query<ShopCommodityStock>()
                    .Where(m => m.Id == item.CommodityStockId);
                dic.Add(item.Count, stock);
            }
            
            return ComputeMemberDiscount(dic);
        }

        /// <summary>
        /// 计算订单会员折扣
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        public int ComputeMemberDiscount(Dictionary<int,IQueryable<ShopCommodityStock>> stocks)
        {
            double rtn = 0;
            var memberLevel = shopDb.GetSingle<ShopMemberLevel>(mShopMember.ShopMemberLevelId);
            if (memberLevel == null) return 0;
            double discountLevel = memberLevel.Discount;
            foreach (var item in stocks)
            {
                int salePrice = item.Value.Where(m => m.Sku.Commodity.UseMemberPrice)
                    .Select(m => m.SalePrice)
                    .FirstOrDefault();
                double discount = salePrice * discountLevel * item.Key;
                rtn += discount;
            }
            return (int)rtn;
        }
        
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="old"></param>
        /// <param name="newPwd"></param>
        public void ChangePassword(string old, string newPwd)
        {
            if (!MemberPasswordToMD5(old).Equals(mShopMember.PayPassword))
                throw new Exception("原密码不正确");
            mShopMember.PayPassword = MemberPasswordToMD5(newPwd);
        }

        string MemberPasswordToMD5(string password)
        {
            MD5 md5Hash = MD5.Create();
            var source = string.Format("{0}__{1}", _memberPasswordMD5Key, password);
            string hash = Common.CommonUtil.GetMD5Hash(source);
            return hash;
        }

        //单品会员价
        public static int GetMemberPrice(int price, double memberDiscount)
        {
            int rtn = (int)(price * memberDiscount);
            return rtn == 0 ? 1 : rtn;
        }


    }
}
