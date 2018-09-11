using System;
using System.Linq;
using ZRui.Web.BLL.MemberAddressExtension;
using ZRui.Web.BLL.ShopDbContextExtension;
using ZRui.Web.BLL.Utils;

namespace ZRui.Web.BLL
{
    public class ShopOrderServer
    {
        private ShopOrder mShopOrder;
        
        public ShopOrderServer(ShopOrder shopOrder)
        {
            mShopOrder = shopOrder;
        }

        /// <summary>
        /// 产生订单编码
        /// </summary>
        /// <param name="category">订单类型</param>
        /// <param name="shopId">商户id</param>
        /// <returns></returns>
        public string GenerateOrderNumber()
        {
            int shopId = mShopOrder.ShopId;
            OrderCategory category = GetOrderCategory();
            return OrderCodeGenerator.Generate(category, shopId);
        }

        /// <summary>
        /// 记录其它费用
        /// </summary>
        /// <param name="db"></param>
        /// <param name="takeWay"></param>
        public void RecordOtherFee(ShopDbContext db, TakeWay? takeWay, int orderItemNumb)
        {
            if (mShopOrder.IsTakeOut)
            {
                var takeOutInfo = db.Query<ShopTakeOutInfo>()
                    .Where(m => !m.IsDel)
                    .Where(m => m.ShopId == mShopOrder.ShopId)
                    .FirstOrDefault();
                if (takeOutInfo == null) throw new Exception("该商铺未有外卖功能");
                ShopOrderOtherFee otherFee = new ShopOrderOtherFee()
                {
                    BoxFee = takeOutInfo.BoxFee
                };
                if (takeWay == TakeWay.送货上门)
                    otherFee.DeliveryFee = takeOutInfo.DeliveryFee;
                mShopOrder.ShopOrderOtherFee = otherFee;
            }else if (mShopOrder.ShopOrderSelfHelp!=null)  //自助点餐
            {
                var selfHelpInfo = db.Query<ShopSelfHelpInfo>()
                    .Where(m => !m.IsDel && m.ShopId == mShopOrder.ShopId)
                    .FirstOrDefault();
                if ( mShopOrder.ShopOrderSelfHelp.IsTakeOut && selfHelpInfo != null && selfHelpInfo.HasBoxFee)
                {
                    ShopOrderOtherFee otherFee = new ShopOrderOtherFee()
                    {
                        BoxFee = selfHelpInfo.BoxFee
                    };
                    mShopOrder.ShopOrderOtherFee = otherFee;
                }
            }
        }

        /// <summary>
        /// 获取订单的类型
        /// </summary>
        /// <returns></returns>
        public OrderCategory GetOrderCategory()
        {
            if (mShopOrder.ShopPartId.HasValue)
                return OrderCategory.ScanCode;
            else if (mShopOrder.ShopOrderSelfHelp!=null)
                return OrderCategory.SelfOrder;
            else if (mShopOrder.IsTakeOut)
                return OrderCategory.Takeout;
            else
                return OrderCategory.Subscribe;
        }


        /// <summary>
        /// 记录外卖信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="shop"></param>
        /// <param name="memberId"></param>
        /// <param name="takeWay"></param>
        /// <param name="pickupTime"></param>
        public void RecordTakeout(ShopDbContext db, Shop shop, int memberId, TakeWay takeWay, DateTime? pickupTime)
        {
            ShopOrderTakeout takeout = null;
            takeout = new ShopOrderTakeout()
            {
                ShopOrderId = mShopOrder.Id,
                MemberId = memberId,
                TakeWay = takeWay
            };
            if (takeout.TakeWay == TakeWay.送货上门)
            {
                var memberAddress = db.Query<MemberAddress>()
                .Where(m => !m.IsDel && m.IsUsed)
                .Where(m => m.MemberId == memberId)
                .FirstOrDefault();
                if (memberAddress == null) throw new Exception("请先设置配送地址");
                if (!pickupTime.HasValue) throw new Exception("预计配送时间不能为空");
                memberAddress.CheckIsInScope(db, shop);
                takeout.Address = memberAddress.Detail;
                takeout.Phone = memberAddress.Phone;
                takeout.Name = memberAddress.Name;
                takeout.Sex = memberAddress.Sex;
                takeout.Longitude = memberAddress.Longitude;
                takeout.Latitude = memberAddress.Latitude;
            }
            else if (takeout.TakeWay == TakeWay.自提)
            {
                if (!pickupTime.HasValue) throw new Exception("自提时间不能为空");
            }
            takeout.PickupTime = pickupTime.Value;
            db.AddTo(takeout);
        }

        /// <summary>
        /// 记录桌号信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="shopPartFlag"></param>
        public void RecordScancode(ShopDbContext db, string shopPartFlag)
        {
            var shopPartId = db.Query<ShopPart>()
                .Where(m => !m.IsDel)
                .Where(m => m.Flag == shopPartFlag && m.ShopId == mShopOrder.ShopId)
                .Select(m => m.Id)
                .FirstOrDefault();
            if (shopPartId <= 0) throw new Exception("商铺桌号/房号纪录不存在");
            mShopOrder.ShopPartId = shopPartId;
        }


        /// <summary>
        /// 记录自助点餐信息
        /// </summary>
        /// <returns></returns>
        public void RecordShopOrderSelfHelp(ShopDbContext db, int shopId, bool isTakeOut)
        {
            ShopOrderSelfHelp model = new ShopOrderSelfHelp()
            {
                Number = db.GetCodeForShopOrderSelfHelp(shopId),
                IsTakeOut = isTakeOut
            };
            mShopOrder.ShopOrderSelfHelp = model;
        }


        /// <summary>
        /// 计算应付金额
        /// </summary>
        /// <param name="shopDb"></param>
        /// <param name="shopId"></param>
        public void ComputePayment(ShopDbContext shopDb)
        {
            int shopId = mShopOrder.ShopId;
            MoneyOffType moneyOffType;
            if (mShopOrder.IsTakeOut)
            {
                moneyOffType = MoneyOffType.外卖;
            }
            else if (mShopOrder.ShopOrderSelfHelp != null)
            {
                moneyOffType = MoneyOffType.自助;
            }
            else
            {
                moneyOffType = MoneyOffType.堂食;
            }
            var moneyOffRule = shopDb.GetMoneyOffRule(shopId, mShopOrder.Amount, moneyOffType);
            int totalFee;
            if (moneyOffRule == null)
            {
                totalFee = mShopOrder.Amount;
            }
            else
            {
                totalFee = mShopOrder.Amount - moneyOffRule.Discount;
            }

            //其它费用
            if (mShopOrder.ShopOrderOtherFee!=null)
            {
                int allOtherFee = CountOtherFee(shopDb, mShopOrder.ShopOrderOtherFee);
                totalFee += allOtherFee;
            }
            mShopOrder.Payment = totalFee;
            mShopOrder.MoneyOffRuleId = moneyOffRule?.Id;
        }

       



        /// <summary>
        /// 获取订单支付状态
        /// </summary>
        /// <returns></returns>
        public ShopOrderPayStatus GetPayStatus()
        {
            switch (mShopOrder.Status)
            {
                case ShopOrderStatus.未处理:
                case ShopOrderStatus.待支付:
                    return ShopOrderPayStatus.未支付;
                case ShopOrderStatus.已退款:
                case ShopOrderStatus.退款中:
                case ShopOrderStatus.退款审批:
                    return ShopOrderPayStatus.支付过;
                default:
                    return ShopOrderPayStatus.已支付;
            }
        }

        /// <summary>
        /// 获取订单外卖信息
        /// </summary>
        /// <param name="shopDb"></param>
        /// <returns></returns>
        ShopOrderTakeout GetOrderTakeOut(ShopDbContext shopDb)
        {
            if (mShopOrder.IsTakeOut)
                return shopDb.Query<ShopOrderTakeout>()
                    .Where(m => !m.IsDel && m.ShopOrderId == mShopOrder.Id)
                    .FirstOrDefault();
            return null;
        }
        
        //计算其它费用
        int CountOtherFee(ShopDbContext shopDb, ShopOrderOtherFee otherFee)
        {
            if (otherFee == null) return 0;
            int rtn = otherFee.BoxFee+ otherFee.DeliveryFee;
            return rtn;
        }
    }
}
