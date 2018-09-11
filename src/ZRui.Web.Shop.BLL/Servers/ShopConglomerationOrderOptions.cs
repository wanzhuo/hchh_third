using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Senparc.Weixin.WxOpen.AdvancedAPIs.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZRui.Web.BLL.ServerDto;

namespace ZRui.Web.BLL.Servers
{
    /// <summary>
    /// 拼团订单服务
    /// </summary>
    public class ShopConglomerationOrderOptions
    {

        /// <summary>
        /// 创建订单
        /// </summary>
        /// <returns></returns>
        public ConglomerationOrder Create(ShopDbContext db, ShopConglomerationOrderDto shopConglomerationOrderDto)
        {
            if (shopConglomerationOrderDto.Type.Equals(ConsignmentType.快递))
            {
                var memberAddress = db.Query<MemberAddress>()
                .FirstOrDefault(m => !m.IsDel && m.Id.Equals(shopConglomerationOrderDto.MemberAddressId));
                if (memberAddress == null) throw new Exception("请先设置配送地址");
            }
            var conglomerationActivity = db.ConglomerationActivity.Find(shopConglomerationOrderDto.ConglomerationSetUp.ConglomerationActivityId);
            //活动结束时间校验
            if (conglomerationActivity.ActivityEndTime <= DateTime.Now)
            {
                throw new Exception("活动已经结束了");
            }
            DateTime dateBeginTime = new DateTime(DateTime.Now.Year, conglomerationActivity.DeliveryTakeTheirBeginTimeMD.Month, conglomerationActivity.DeliveryTakeTheirBeginTimeMD.Day, conglomerationActivity.DeliveryTakeTheirBeginTimeHM.Hour, conglomerationActivity.DeliveryTakeTheirBeginTimeHM.Minute, conglomerationActivity.DeliveryTakeTheirBeginTimeHM.Second);
            DateTime dateEndTime = new DateTime(DateTime.Now.Year, conglomerationActivity.DeliveryTakeTheirEndTimeMD.Month, conglomerationActivity.DeliveryTakeTheirEndTimeMD.Day, conglomerationActivity.DeliveryTakeTheirEndTimeHM.Hour, conglomerationActivity.DeliveryTakeTheirEndTimeHM.Minute, conglomerationActivity.DeliveryTakeTheirEndTimeHM.Second);
            ////自提时间校验
            //if (shopConglomerationOrderDto.Type.Equals(ConsignmentType.自提))
            //{
            //    if (!(dateBeginTime <= shopConglomerationOrderDto.Delivery && shopConglomerationOrderDto.Delivery <= dateEndTime))
            //    {
            //        throw new Exception("时间超出范围");
            //    }
            //}
            var conglomerationOrder = InitOrderData(db, shopConglomerationOrderDto);
            conglomerationOrder = TakeOutSetting(db, conglomerationOrder, shopConglomerationOrderDto, conglomerationActivity);
            conglomerationOrder = SelfSetting(db, conglomerationOrder);
            conglomerationOrder = SetAmount(db, conglomerationOrder);
            db.SaveChanges();
            return conglomerationOrder;
        }

        /// <summary>
        /// 设置计算订单金额
        /// </summary>
        /// <param name="db"></param>
        /// <param name="conglomerationOrder"></param>
        /// <returns></returns>
        private ConglomerationOrder SetAmount(ShopDbContext db, ConglomerationOrder conglomerationOrder)
        {
            //拼团金额
            var conglomerationSetUp = db.ConglomerationSetUp
                .Where(m => m.Id.Equals(conglomerationOrder.ConglomerationSetUpId))
                .Include(m => m.ConglomerationActivityType)
                .AsNoTracking()
                .FirstOrDefault();
            ;
            //拼团金额
            conglomerationOrder.Amount += conglomerationSetUp.ConglomerationActivityType.ConglomerationPrice;
            //配送费
            if (conglomerationOrder.Type.Equals(ConsignmentType.快递))
            {
                var conglomerationExpress = db.ConglomerationExpress.Where(m => m.ShopConglomerationOrderId.Equals(conglomerationOrder.Id)).AsNoTracking()
                  .FirstOrDefault(); ;
                conglomerationOrder.Amount += conglomerationExpress.ActivityDeliveryFee;

            }
            conglomerationOrder.Payment = conglomerationOrder.Amount;
            db.SaveChanges();
            return conglomerationOrder;
        }

        /// <summary>
        /// 快递配置(配送)
        /// </summary>
        /// <param name="db"></param>
        /// <param name="conglomerationOrder"></param>
        /// <returns></returns>
        private ConglomerationOrder TakeOutSetting(ShopDbContext db, ConglomerationOrder conglomerationOrder, ShopConglomerationOrderDto shopConglomerationOrderDto, ConglomerationActivity conglomerationActivity)
        {
            if (conglomerationOrder.Type.Equals(ConsignmentType.快递))
            {

                ConglomerationExpress conglomerationExpress = new ConglomerationExpress();
                conglomerationExpress.CreateTime = DateTime.Now;
                conglomerationExpress.Delivery = conglomerationOrder.Delivery.Value;
                conglomerationExpress.ExpressSingle = "未发货";
                conglomerationExpress.MemberAddressId = shopConglomerationOrderDto.MemberAddressId;
                conglomerationExpress.ShopConglomerationOrderId = conglomerationOrder.Id;
                var memberAddress = db.Query<MemberAddress>()
                  .FirstOrDefault(m => !m.IsDel && m.Id.Equals(conglomerationExpress.MemberAddressId));
                if (memberAddress == null) throw new Exception("请先设置配送地址");
                conglomerationExpress.Address = $"{ memberAddress.Province}{ memberAddress.City}{ memberAddress.Area}{ memberAddress.Detail}";
                conglomerationExpress.Phone = memberAddress.Phone;
                conglomerationExpress.Name = memberAddress.Name;
                conglomerationExpress.Sex = memberAddress.Sex;
                conglomerationExpress.ActivityDeliveryFee = conglomerationActivity.ActivityDeliveryFee;
                conglomerationExpress = db.ConglomerationExpress.Add(conglomerationExpress).Entity;
                db.SaveChanges();
                conglomerationOrder.ConglomerationExpressId = conglomerationExpress.Id;
            }

            return conglomerationOrder;
        }


        /// <summary>
        /// 快递配置(自提)
        /// </summary>
        /// <param name="db"></param>
        /// <param name="conglomerationOrder"></param>
        /// <returns></returns>
        private ConglomerationOrder SelfSetting(ShopDbContext db, ConglomerationOrder conglomerationOrder)
        {
            conglomerationOrder.PickupCode = "";
            if (conglomerationOrder.Type.Equals(ConsignmentType.自提))
            {
                //提货码
                conglomerationOrder.PickupCode = GetPickupCode(db);

            }
            db.SaveChanges();
            return conglomerationOrder;
        }

        /// <summary>
        /// 初始化订单
        /// </summary>
        /// <param name="shopConglomerationOrderDto"></param>
        /// <param name="conglomerationOrder"></param>
        private ConglomerationOrder InitOrderData(ShopDbContext db, ShopConglomerationOrderDto shopConglomerationOrderDto)
        {

            ConglomerationOrder conglomerationOrder = new ConglomerationOrder();
            conglomerationOrder.CreateTime = DateTime.Now;
            conglomerationOrder.FormId = shopConglomerationOrderDto.FormId;
            conglomerationOrder.OrderNumber = OrderCodeGenerator.Generate(OrderCategory.Conglomeration, shopConglomerationOrderDto.ShopId);
            conglomerationOrder.ShopId = shopConglomerationOrderDto.ShopId;
            conglomerationOrder.Type = shopConglomerationOrderDto.Type;
            conglomerationOrder.ConglomerationSetUpId = shopConglomerationOrderDto.ConglomerationSetUpId;
            conglomerationOrder.ShopId = shopConglomerationOrderDto.ShopId;
            conglomerationOrder.MemberId = shopConglomerationOrderDto.MemberId;
            conglomerationOrder.AddUser = $"member{shopConglomerationOrderDto.MemberId}";
            conglomerationOrder.AddIp = shopConglomerationOrderDto.AddIp;
            //conglomerationOrder.Amount = shopConglomerationOrderDto.ConglomerationSetUp.ConglomerationActivityType.ConglomerationPrice;
            conglomerationOrder.ConglomerationActivityId = shopConglomerationOrderDto.ConglomerationSetUp.ConglomerationActivityId;
            conglomerationOrder.Status = ShopOrderStatus.未处理;
            conglomerationOrder.Delivery = shopConglomerationOrderDto.Delivery;
            conglomerationOrder = db.ConglomerationOrder.Add(conglomerationOrder).Entity;
            db.SaveChanges();
            return conglomerationOrder;


        }

        /// <summary>
        /// 获取提货码
        /// </summary>
        /// <returns></returns>
        private string GetPickupCode(ShopDbContext db)
        {
            //var guid = Guid.NewGuid().ToString().Substring(0,16);
            Random rd = new Random();
            var guid = rd.Next(100000, 999999).ToString();
            guid = guid + DateTime.Now.ToString("yyyyMMddHH");
            var query = db.ConglomerationOrder.Where(m => m.PickupCode.Equals(guid));
            if (query.Count() > 0)
            {
                GetPickupCode(db);
            }
            return guid;
        }


        /// <summary>
        /// 拼团通知用户
        /// </summary>
        /// <param name="db"></param>
        public static async void MemberInform(ShopDbContext db, MemberDbContext memberDbContext, int ConglomerationOrderId, ILogger logger)
        {
            logger.LogInformation("======================发送通知开始调用==================");
            try
            {
                if (ConglomerationOrderId == 0)
                {
                    return;
                }
                var conglomerationOrder = db.ConglomerationOrder.Find(ConglomerationOrderId);
                logger.LogInformation($"订单Id：{conglomerationOrder.Id}");

                if (conglomerationOrder == null)
                {
                    return;
                }
                var authorizerAccessToken = GetAuthorizerAccessToken(db, conglomerationOrder.ShopId);
                var templateMessage = db.ShopTemplateMessageInfo.FirstOrDefault(m => m.ShopId.Equals(conglomerationOrder.ShopId));
                var conglomerationSetUp = db.ConglomerationSetUp.FirstOrDefault(m => m.Id.Equals(conglomerationOrder.ConglomerationSetUpId));
                conglomerationSetUp.ConglomerationActivity = db.ConglomerationActivity.FirstOrDefault(m => m.Id.Equals(conglomerationSetUp.ConglomerationActivityId));
                logger.LogInformation($"拼团Id：{conglomerationSetUp.Id} 是否发通知conglomerationSetUp.IsSendSMS{conglomerationSetUp.IsSendSMS}");
                if (conglomerationSetUp == null)
                {
                    return;
                }
                if (conglomerationSetUp.Status == ConglomerationSetUpStatus.已经成团 && !conglomerationSetUp.IsSendSMS)
                {

                    var conglomerationParticipations = db.ConglomerationParticipation.Where(m => m.ConglomerationSetUpId.Equals(conglomerationSetUp.Id)).ToList();
                    foreach (var item in conglomerationParticipations)
                    {
                        logger.LogInformation($"开始通知循环参团人员memmberID：{item.MemberId}");

                        var memberLogins = memberDbContext.MemberLogins.FirstOrDefault(m => m.MemberId.Equals(item.MemberId));
                        var memberOrder = db.ConglomerationOrder.FirstOrDefault(m => m.Id.Equals(item.ConglomerationOrderId));
                        var data = GetSendTemplateMessageData(conglomerationSetUp, memberOrder);
                        var sendTemplateMessage = TemplateApi.SendTemplateMessage(authorizerAccessToken, memberLogins.OpenId, templateMessage.TemplateId, data, memberOrder.FormId, "pages/home/statusinfo");
                    }
                    conglomerationSetUp.IsSendSMS = true;
                    db.SaveChanges();
                }


            }
            catch (Exception e)
            {
                logger.LogInformation($"发送通知错误{e}");
            }
            logger.LogInformation("======================发送通知结束调用==================");

        }

        /// <summary>
        /// 核销拼团提货码
        /// </summary>
        /// <param name="db"></param>
        /// <param name="orderId"></param>
        /// <param name="pickupCode"></param>
        /// <param name="_logger"></param>
        /// <returns></returns>
        public static async Task<bool> TakeTheirFinishOrderAsync(ShopDbContext db, int orderId, string pickupCode, ILogger _logger)
        {
            var order = db.ConglomerationOrder.Find(orderId);
            if (!order.Type.Equals(ConsignmentType.自提))
            {
                throw new Exception("订单类型为快递类型");
            }
            if (!order.Status.Equals(ShopOrderStatus.待自提))
            {
                throw new Exception("订单不是待自提状态");

            }
            if (!order.PickupCode.Equals(pickupCode))
            {
                throw new Exception("提货码错误");

            }
            order.Status = ShopOrderStatus.已完成;
            db.SaveChanges();

            #region 积分获取
            var sourceType = await ShopIntegralRechargeServer.GetOrderSourceType(db, orderId, true, _logger);
            await ShopIntegralRechargeServer.GetOrderIntegral(db, orderId, sourceType, _logger);
            #endregion
            return true;
        }

        #region 内部使用函数
        /// <summary>
        /// 获取发送推送内容
        /// </summary>
        /// <returns></returns>
        static object GetSendTemplateMessageData(ConglomerationSetUp conglomerationSetUp, ConglomerationOrder conglomerationOrder)
        {
            var data = new
            {
                //活动名称
                keyword1 = new
                {
                    value = conglomerationSetUp.ConglomerationActivity.ActivityName,
                    color = "#173177"
                },
                //成团人数
                keyword2 = new
                {
                    value = conglomerationSetUp.CurrentMemberNumber,
                    color = "#173177"
                },
                //订单号
                keyword3 = new
                {
                    value = conglomerationOrder.OrderNumber,
                    color = "#173177"
                },
                //发货时间
                keyword4 = conglomerationOrder.Type == ConsignmentType.快递 ? new
                {
                    value = string.Format("{0:yyyy-MM-dd HH:mm:ss}", conglomerationOrder.Delivery),
                    color = "#173177"
                } :
                new
                {
                    value = "无",
                    color = "#173177"
                }

                ,
                //配送方式
                keyword5 = new
                {
                    value = conglomerationOrder.Type.ToString(),
                    color = "#173177"
                },
                //提货码
                keyword6 = conglomerationOrder.Type == ConsignmentType.自提 ? new
                {
                    value = conglomerationOrder.PickupCode,
                    color = "#173177"
                } : new
                {
                    value = "无",
                    color = "#173177"
                },
            };

            return data;
        }
        public static string GetAuthorizerAccessToken(ShopDbContext db, int shopId)
        {
            var model = db.Query<ShopWechatOpenAuthorizer>()
                .Where(m => !m.IsDel)
                .Where(m => m.ShopId == shopId)
                .Select(m => new
                {
                    AuthorizerAppId = m.WechatOpenAuthorizer.AuthorizerAppId,
                    AuthorizerAccessToken = m.WechatOpenAuthorizer.AuthorizerAccessToken,
                    ExpiresTime = m.WechatOpenAuthorizer.ExpiresTime
                })
                .FirstOrDefault();

            if (model == null) throw new Exception("指定的纪录不存在");
            //if (model.ExpiresTime.AddMinutes(20) > DateTime.Now) return model.AuthorizerAccessToken;

            return GetAuthorizerAccessToken(model.AuthorizerAppId);
        }

        static string GetAuthorizerAccessToken(string authorizerAppId)
        {
            //  var authorizerAccessToken = AuthorizerContainer.TryGetAuthorizerAccessToken(wechatOpenOptions.AppId, authorizerAppId);
            //  return authorizerAccessToken;
            return ZRui.Web.BLL.AuthorizerHelper.GetAuthorizerAccessToken(authorizerAppId);
        }
        #endregion
    }
}
