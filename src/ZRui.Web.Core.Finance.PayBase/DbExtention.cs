using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Senparc.Weixin.MP.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ZRui.Web.BLL.Servers;
using ZRui.Web.BLL.Third;
using ZRui.Web.Common;
using ZRui.Web.Core.Printer;
using ZRui.Web.Core.Printer.Base;
using ZRui.Web.Core.Printer.Data;
using ZRui.Web.Core.Printer.Models;
using ZRui.Web.Core.Wechat;

namespace ZRui.Web.Core.Finance.PayBase
{
    public static class DbExtention
    {
        private const string TemplateSendColor = "#3c3c3c";

        public static void SetFinish(this MemberTradeForRechange rechange, PrintDbContext printDbContext, ThirdConfig toptions, ShopDbContext shopDb, DbContext db, WechatTemplateSendOptions options, PayResponseBaseHandler result, ILogger _logger)
        {
            if (result.TradeState != "NOTPAY")
            {
                if (rechange.Status == MemberTradeForRechangeStatus.未完成)
                {

                    if (rechange.TotalFee != result.TotalFee) throw new Exception("指定的金额不对应");
                    rechange.OutBank = result.Xml;
                    rechange.MechanismTradeNo = result.TransactionId;
                    switch (result.TradeState)
                    {
                        case "SUCCESS":
                            db.SetMemberTradeForRechangeSuccess(rechange);
                            if (rechange.OrderType == OrderType.充值订单)
                                SetShopMemberRechangeFinish(rechange, shopDb);
                            else
                                SetShopOrderFinish(printDbContext, shopDb, rechange, options, toptions, _logger);
                            break;
                        case "CLOSED":
                            rechange.Status = MemberTradeForRechangeStatus.取消;
                            break;
                        default:
                            db.SetMemberTradeForRechangeFail(rechange, result.TradeState);
                            break;
                    }
                }
            }
        }


        /// <summary>
        /// 设置充值完成
        /// </summary>
        /// <param name="rechange"></param>
        /// <param name="shopDb"></param>
        private static void SetShopMemberRechangeFinish(MemberTradeForRechange rechange, ShopDbContext shopDb)
        {
            var memberTransaction = shopDb.GetSingle<ShopMemberRecharge>(rechange.ShopOrderId.Value);
            if (memberTransaction == null) return;
            memberTransaction.Status = ShopMemberTransactionStatus.已完成;
            int increaseAmount = memberTransaction.Amount + memberTransaction.PresentedAmount;
            var shopMember = shopDb.Query<ShopMember>()
                .Where(m => !m.IsDel && m.ShopId == rechange.ShopId && m.MemberId == rechange.MemberId)
                .FirstOrDefault();
            shopMember.Balance += increaseAmount;
            shopDb.SaveChanges();
        }

        public static void SetFinish(this MemberTradeForRechange rechange, PrintDbContext printDbContext, ShopDbContext shopDb, DbContext db, WechatTemplateSendOptions options, ThirdConfig toptions, ILogger _logger)
        {
            if (rechange.ShopOrderId != 0)
            {
                if (rechange.OrderType == OrderType.充值订单)
                    SetShopMemberRechangeFinish(rechange, shopDb);
                else
                    SetShopOrderFinish(printDbContext, shopDb, rechange, options, toptions, _logger);
            }
            if (rechange.ConglomerationOrderId != 0)
            {
                _logger.LogInformation("进入回调，拼团订单");

                try
                {
                    SetConglomerationOrderFinish(printDbContext, shopDb, rechange, options, _logger);
                }
                catch (Exception e)
                {
                    _logger.LogInformation($"回调错误，错误信息：{ e}");

                    throw;
                }
            }

        }

        /// <summary>
        /// 设置拼团订单支付完成
        /// </summary>
        /// <param name="printDbContext"></param>
        /// <param name="shopDb"></param>
        /// <param name="rechange"></param>
        /// <param name="options"></param>
        /// <param name="logger"></param>
        static void SetConglomerationOrderFinish(PrintDbContext printDbContext, ShopDbContext db, MemberTradeForRechange memberTradeForRechange, WechatTemplateSendOptions options, ILogger _logger)
        {
            var model = db.ConglomerationOrder.Find(memberTradeForRechange.ConglomerationOrderId);
            if (model.PayTime.HasValue)
            {
                return;
            }
            model.PayTime = DateTime.Now;
            model.Status = ShopOrderStatus.已支付;
            model.PayWay = "微信支付";
            db.SaveChanges();
            model.Payment = (int)memberTradeForRechange.TotalFee;
            var shopName = db.Query<Shop>()
              .Where(m => m.Id == model.ShopId)
              .Select(m => m.Name)
              .FirstOrDefault();

            AfterConglomerationOrderPlacing(printDbContext, db, model, shopName, options, _logger);

            memberTradeForRechange.Status = MemberTradeForRechangeStatus.成功;

            db.SaveChanges();
        }

        /// <summary>
        /// 拼团支付成功之后操作
        /// </summary>
        static void AfterConglomerationOrderPlacing(PrintDbContext printDbContext, ShopDbContext db, ConglomerationOrder conglomerationOrder, string shopName, WechatTemplateSendOptions options, ILogger _logger)
        {
            _logger.LogInformation("=================拼团订单回调开始===============");

            var isExist = db.ConglomerationParticipation.FirstOrDefault(m => !m.IsDel && m.ConglomerationSetUpId.Equals(conglomerationOrder.ConglomerationSetUpId) && m.ConglomerationOrderId.Equals(conglomerationOrder.Id));
            if (isExist != null)
            {
                return;
            }
            var conglomerationSetUp = db.ConglomerationSetUp.Find(conglomerationOrder.ConglomerationSetUpId);
            var member = db.Member.Find(conglomerationOrder.MemberId);

            //添加参团信息
            ConglomerationParticipation conglomerationParticipation = new ConglomerationParticipation();
            conglomerationParticipation.CreateTime = DateTime.Now;
            conglomerationParticipation.AvatarUrl = member.Avatar ?? "";
            conglomerationParticipation.NickName = member.NickName ?? "";
            conglomerationParticipation.ConglomerationSetUpId = conglomerationOrder.ConglomerationSetUpId;
            conglomerationParticipation.Role = conglomerationSetUp.MemberId.Equals(conglomerationOrder.MemberId) ? ParticipationRole.团长 : ParticipationRole.团员;
            conglomerationParticipation.MemberId = conglomerationOrder.MemberId;
            conglomerationParticipation.ConglomerationOrderId = conglomerationOrder.Id;
            conglomerationParticipation.ConglomerationActivityId = conglomerationOrder.ConglomerationActivityId;
            db.ConglomerationParticipation.Add(conglomerationParticipation);
            //更新已发起拼团的队伍状态
            conglomerationSetUp.CurrentMemberNumber = conglomerationSetUp.CurrentMemberNumber + 1;
            conglomerationSetUp.Status = conglomerationSetUp.CurrentMemberNumber.Equals(conglomerationSetUp.MemberNumber) ? ConglomerationSetUpStatus.已经成团 : ConglomerationSetUpStatus.未成团;
            _logger.LogInformation($"当前拼团状态 conglomerationSetUp.Status：{conglomerationSetUp.Status}");
            conglomerationOrder.Status = ShopOrderStatus.待成团;
            if (conglomerationSetUp.Status.Equals(ConglomerationSetUpStatus.已经成团))
            {
                conglomerationSetUp.SuccessfulTime = DateTime.Now;
                _logger.LogInformation("=============================已经成团修改订单状态开始========================");
                _logger.LogInformation($"团IDconglomerationOrder.ConglomerationSetUpId {conglomerationOrder.ConglomerationSetUpId}");
                conglomerationOrder.Status = conglomerationOrder.Type == ConsignmentType.自提 ? ShopOrderStatus.待自提 : ShopOrderStatus.待配送;
                var order = db.ConglomerationOrder.Where(m => !m.IsDel && m.ConglomerationSetUpId.Equals(conglomerationOrder.ConglomerationSetUpId)).ToList();
                _logger.LogInformation($"需更改订单状态数量{order.Count()}");
                foreach (var item in order)
                {
                    if (item.Type == ConsignmentType.自提)
                    {
                        _logger.LogInformation($"订单Id:{item.Id} 更改为执行待自提");
                        if (item.Status == ShopOrderStatus.待成团)
                        {
                            item.Status = ShopOrderStatus.待自提;
                        }
                    }
                    else
                    {
                        if (item.Status == ShopOrderStatus.待成团)
                        {
                            _logger.LogInformation($"订单Id:{item.Id} 更改为执行待配送");
                            item.Status = ShopOrderStatus.待配送;
                        }
                    }
                }
                _logger.LogInformation("=============================已经成团修改订单状态结束========================");
            }

            //发送推送
            if (!conglomerationOrder.IsSend)
            {
                var receiver = db.Query<ShopOrderReceiver>()
                    .Where(m => !m.IsDel)
                    .Where(m => m.IsUsed)
                    .Where(m => m.ShopId == conglomerationOrder.ShopId)
                    .Select(m => m.ReceiverOpenId)
                    .Distinct()
                    .ToList();
                receiver.ForEach(o =>
                    conglomerationOrder.SendTemplateMessage(options, o, _logger));
            }

            conglomerationOrder.IsSend = true;
            db.SaveChanges();
            _logger.LogInformation("=================拼团订单回调结束===============");

        }


        static void SetShopOrderFinish(PrintDbContext printDbContext, ShopDbContext db,
            MemberTradeForRechange memberTradeForRechange, WechatTemplateSendOptions options, ThirdConfig toptions, ILogger _logger)
        {
            if (!memberTradeForRechange.ShopOrderId.HasValue)
            {
                throw new Exception("memberTradeForRechange.ShopOrderId is null");
            }
            var shopOrder = db.ShopOrders.FirstOrDefault(p => p.Id == memberTradeForRechange.ShopOrderId.Value);
            shopOrder.PayWay = "微信支付";
            memberTradeForRechange.Status = MemberTradeForRechangeStatus.成功;
            db.SaveChanges();
            OrderFinish(printDbContext, db, options, toptions, shopOrder, _logger);
            var shopName = db.Query<Shop>()
             .Where(m => m.Id == shopOrder.ShopId)
             .Select(m => m.Name)
             .FirstOrDefault();
            AfterOrderPlacing(printDbContext, db, toptions, shopOrder, shopName, options, _logger);

        }

        /// <summary>
        /// 直接设置订单完成
        /// </summary>
        /// <param name="shopOrder"></param>
        /// <param name="printDbContext"></param>
        /// <param name="db"></param>
        /// <param name="options"></param>
        /// <param name="_logger"></param>
        public static void SetShopOrderFinish(this ShopOrder shopOrder, PrintDbContext printDbContext,
            ShopDbContext db, WechatTemplateSendOptions options, ThirdConfig toptions, ILogger _logger)
        {
            OrderFinish(printDbContext, db, options, toptions, shopOrder, _logger);
            var shopName = db.Query<Shop>()
               .Where(m => m.Id == shopOrder.ShopId)
               .Select(m => m.Name)
               .FirstOrDefault();
            AfterOrderPlacing(printDbContext, db, toptions, shopOrder, shopName, options, _logger);
        }

        /// <summary>
        /// 完成订单
        /// </summary>
        /// <param name="printDbContext"></param>
        /// <param name="db"></param>
        /// <param name="options"></param>
        /// <param name="shopOrderid"></param>
        /// <param name="_logger"></param>
        static void OrderFinish(PrintDbContext printDbContext, ShopDbContext db,
            WechatTemplateSendOptions options, ThirdConfig toptions, ShopOrder shopOrder, ILogger _logger)
        {

            shopOrder.PayTime = DateTime.Now;
            shopOrder.Status = ShopOrderStatus.已支付;
            db.SaveChanges();

            //获取订单中商品的数量
            var commodityIdAndCounts = db.Query<ShopOrderItem>()
                .Where(m => m.ShopOrderId == shopOrder.Id)
                .Where(m => !m.IsDel)
                .Select(m => new
                {
                    m.CommodityStock.Sku.CommodityId,
                    m.Count
                })
                .ToList()
                .GroupBy(m => m.CommodityId)
                .ToDictionary(m => m.Key, m => m.Select(x => x.Count).Sum());

            //更新商品的销售量
            //注意，这里如果有一个品牌，多个店铺的情况，会出现销售额共享的情况
            var commodityIds = commodityIdAndCounts.Select(m => m.Key).ToList();
            var commoditys = db.Query<ShopBrandCommodity>()
                    .Where(m => commodityIds.Contains(m.Id))
                    .ToList();
            foreach (var item in commoditys)
            {
                item.SalesForMonth += commodityIdAndCounts[item.Id];
            }



        }
        /// <summary>
        /// 达达配送
        /// </summary>
        /// <param name="db"></param>
        /// <param name="shopOrderid"></param>
        /// <param name="_logger"></param>
        public static async Task<ThirdAddOrderResult> ThirdOrderFinish(ShopDbContext db, ThirdConfig toptions, ShopOrder model, ILogger _logger, ExSource exSource)
        {
            var result = new ThirdAddOrderResult();
            try
            {
                _logger.LogInformation("===========开始OrderFinish=============");

                if (model == null)
                    _logger.LogInformation($"============订单不存在============");
                var shoptakeoutinfo = db.ShopTakeOutInfo.FirstOrDefault(r => r.ShopId == model.ShopId && !r.IsDel);
                if (shoptakeoutinfo != null && shoptakeoutinfo.TakeDistributionType == TakeDistributionType.达达配送 && model.IsTakeOut)
                {
                    _logger.LogInformation($"===========开始处理达达配送业务=============");
                    _logger.LogInformation($"===========订单信息：{JsonConvert.SerializeObject(model)}=============");
                    double fee = 0;
                    model.Status = exSource == ExSource.发起支付 ? ShopOrderStatus.待支付 : ShopOrderStatus.待接单;
                    ThirdServer thirdServer = new ThirdServer(db, toptions);
                    var merchant = db.Merchant.FirstOrDefault(r => r.ShopId == model.ShopId);
                    if (merchant == null)
                        _logger.LogInformation($"============商户ID{model.ShopId}未在达达开户============");
                    var thirdshop = db.ThirdShop.FirstOrDefault(r => r.ShopId == model.ShopId && r.Status == Data.ThirdShop.ShopStatus.门店激活);
                    if (thirdshop == null)
                        _logger.LogInformation($"============商户ID{model.ShopId}商户门店不存在============");
                    var shopordertakeout = db.ShopOrderTakeouts.FirstOrDefault(r => r.ShopOrderId == model.Id && !r.IsDel);
                    if (shoptakeoutinfo == null)
                        _logger.LogInformation($"============商户ID{model.ShopId}订单外卖信息不存在============");
                    //处理达达配送
                    var thirddshopaddmodel = new ThirdShopAddOrderModel()
                    {
                        ShopId = model.ShopId,
                        origin_id = model.OrderNumber,//DateTime.Now.Ticks + CommonUtil.CreateNoncestr(5),//
                        //shop_no = "11047059",//测试
                        shop_no = thirdshop.OriginShopId,
                        cargo_type = -1,
                        cargo_price = model.Amount,
                        city_code = "0769",
                        is_prepay = 0,
                        origin_mark = "HCHH",
                        origin_mark_no = model.OrderNumber,
                        receiver_lng = shopordertakeout.Longitude.Value,
                        receiver_lat = shopordertakeout.Latitude.Value,
                        receiver_phone = shopordertakeout.Phone,
                        receiver_address = shopordertakeout.Address,
                        receiver_name = shopordertakeout.Name,
                        callback = toptions.CallBackUrl,
                    };
                    _logger.LogInformation($"============商户ID{model.ShopId}达达配送信息：{JsonConvert.SerializeObject(thirddshopaddmodel)}============");
                    result = await thirdServer.ThirdAddOrder(thirddshopaddmodel, model);
                    _logger.LogInformation($"============商户ID{model.ShopId}达达发单返回信息：{JsonConvert.SerializeObject(result)}============");
                    if (result.errorCode != 0 || result.status != "success")
                    {

                        _logger.LogInformation($"============商户ID{model.ShopId}达达发单失败。原因：{result.msg}============");
                        return result;
                    }
                    // db.SaveChanges();

                }



            }
            catch (Exception ex)
            {
                _logger.LogInformation($"===========OrderFinish出现异常==============");
                _logger.LogInformation($"============{ex.Message} {ex.StackTrace}============");

            }
            return result;
        }
        /// <summary>
        /// 处理达达预发单信息发起订单
        /// </summary>
        /// <param name="db"></param>
        /// <param name="thirdConfig"></param>
        /// <param name="model"></param>
        /// <param name="_logger"></param>
        public static void ThirdAfterOrder(ShopDbContext db, ThirdConfig thirdConfig, ShopOrder model, ILogger _logger)
        {
            try
            {
                var queryfee = db.ThirdApiData.FirstOrDefault(r => r.OrderId == model.OrderNumber && r.JsonData == "QueryFee");
                if (queryfee != null)
                {
                    var queryfeedata = JsonConvert.DeserializeObject<CallbackAfterOrder>(queryfee.ResultData);
                    ThirdServer thirdServer = new ThirdServer(db, thirdConfig);
                    var result = thirdServer.CallbackAfter(new AddAfterQuery()
                    {
                        deliveryNo = queryfeedata.result.deliveryNo
                      ,
                        Fee = queryfeedata.result.fee,
                        OrderNumber = model.OrderNumber,
                        ShopId = model.ShopId
                    });
                    if (result.errorCode == 0 || result.status == "success")
                    {
                        _logger.LogInformation($"===========回调达达发单成功 更改订单状态为待接单=============");

                        model.Status = ShopOrderStatus.待接单;

                    }
                    else
                    {
                        _logger.LogInformation($"===========回调达达发单失败 原因{result.msg}=============");

                    }
                }
                else
                {
                    _logger.LogInformation($"===========回调达达发单失败 未找到订单预发单信息=============");

                }
            }
            catch (Exception ex)
            {

                _logger.LogInformation($"=========== ThirdAfterOrder Exception {ex.Message}=============");
            }

        }

        public static async Task<CThirdRechargeQueryResult> ThirdAmountFinish(ShopDbContext db, ThirdConfig thirdConfig, CThirdShopRechargeQueryModel model)
        {
            ThirdServer thirdServer = new ThirdServer(db, thirdConfig);
            return await thirdServer.ThirdShopRechargeQuery(model);

        }


        /// <summary>
        /// 订单下单后进行的操作
        /// </summary>
        static async void AfterOrderPlacing(PrintDbContext printDbContext, ShopDbContext db, ThirdConfig thirdConfig, ShopOrder shopOrder, string shopName, WechatTemplateSendOptions options, ILogger _logger)
        {
            //是否外卖单
            if (shopOrder.IsTakeOut)
            {
                //var takeOutInfo = db.Query<ShopTakeOutInfo>()
                //.Where(m => !m.IsDel)
                //.Where(m => m.ShopId == shopOrder.ShopId)
                //.FirstOrDefault();
                ////自动接单
                //if (takeOutInfo!=null && takeOutInfo.AutoTakeOrdre)
                //{
                //    shopOrder.Status = ShopOrderStatus.已确认;
                //    PrintOrder(printDbContext, db, shopOrder, shopName, _logger);
                //}
                PrintOrder(printDbContext, db, shopOrder, shopName, _logger);
            }
            else
            {
                PrintOrder(printDbContext, db, shopOrder, shopName, _logger);
            }


            if (!shopOrder.IsSend)
            {
                var receiver = db.Query<ShopOrderReceiver>()
                    .Where(m => !m.IsDel)
                    .Where(m => m.IsUsed)
                    .Where(m => m.ShopId == shopOrder.ShopId)
                    .Select(m => m.ReceiverOpenId)
                    .Distinct()
                    .ToList();
                receiver.ForEach(o =>
                    shopOrder.SendTemplateMessage(db, options, o, _logger));
            }
            shopOrder.IsSend = true;

            if (!shopOrder.ShopPartId.HasValue && !shopOrder.ShopOrderSelfHelpId.HasValue)
            {
                var shoptakeoutinfo = db.ShopTakeOutInfo.FirstOrDefault(r => r.ShopId == shopOrder.ShopId && !r.IsDel);
                var shopordertakeoutinfo = db.ShopOrderTakeouts.FirstOrDefault(r => r.ShopOrderId == shopOrder.Id && !r.IsDel);
                if (shoptakeoutinfo != null && shoptakeoutinfo.TakeDistributionType == TakeDistributionType.达达配送 && shopordertakeoutinfo.TakeWay == TakeWay.送货上门)
                {
                    //  await ThirdOrderFinish(db, thirdConfig, shopOrder, _logger, ExSource.支付成功);

                    ThirdAfterOrder(db, thirdConfig, shopOrder, _logger);

                }
            }



            db.SaveChanges();
            _logger.LogInformation("=================其他订单回调完成===============");
        }

        private static readonly HttpClient httpClient = new HttpClient()
        {
            Timeout = TimeSpan.FromMinutes(2)

        };

        static DbExtention()
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }



        static bool SendTemplateMessage(this ShopOrder shopOrder, ShopDbContext db, WechatTemplateSendOptions wechatOptions,
                                                     string receiverOpenId, ILogger logger)
        {
            //logger.LogInformation("推送订单订单shopOrderId:{0}", shopOrder.Id);
            try
            {
                if (string.IsNullOrEmpty(receiverOpenId)) return false;
                string accessToken = AccessTokenContainer.GetAccessTokenResult(wechatOptions.AppId).access_token;
                //logger.LogInformation("推送的accesstoken:{0}", accessToken);
                string url = wechatOptions.SendUrl + accessToken;
                TemplateData data = new TemplateData();
                string templateId;
                string remark = shopOrder.Remark ?? "";

                if (shopOrder.IsTakeOut)
                {
                    var takeOutInfo = db.Query<ShopOrderTakeout>().FirstOrDefault(m => m.ShopOrderId == shopOrder.Id);

                    MemberAddress address = null;
                    if (takeOutInfo != null && takeOutInfo.TakeWay == TakeWay.送货上门)
                        address = db.Query<MemberAddress>().FirstOrDefault(m => m.MemberId == shopOrder.MemberId);
                    templateId = wechatOptions.TakeOutTemplateId;
                    string customInfo = "", customAddress = "";
                    if (address != null)
                    {
                        customInfo = $"{address.Name}{address.Sex}({address.Phone})";
                        customAddress = $"{address.Area}{address.Detail}";
                    }
                    else
                    {
                        customInfo = "自提";
                        customAddress = "自提";
                    }
                    #region 订单参数
                    data.Add("first", new TemplateDataItem()
                    {
                        value = $"收到新的外卖订单！({DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")})",
                        color = TemplateSendColor
                    });
                    data.Add("keyword1", new TemplateDataItem()
                    {
                        value = shopOrder.OrderNumber,
                        color = TemplateSendColor
                    });
                    data.Add("keyword2", new TemplateDataItem()
                    {
                        value = db.GetSingle<Shop>(shopOrder.ShopId).Name,
                        color = TemplateSendColor
                    });
                    data.Add("keyword3", new TemplateDataItem()
                    {
                        value = customInfo,
                        color = TemplateSendColor
                    });
                    data.Add("keyword4", new TemplateDataItem()
                    {
                        value = customAddress,
                        color = TemplateSendColor
                    });
                    data.Add("keyword5", new TemplateDataItem()
                    {
                        value = $"￥{(shopOrder.Payment ?? 0) / 100f}",
                        color = TemplateSendColor
                    });
                    data.Add("remark", new TemplateDataItem()
                    {
                        value = remark,
                        color = TemplateSendColor
                    });
                    #endregion
                }
                else
                {
                    templateId = wechatOptions.ServiceTemplateId;
                    #region 订单参数
                    data.Add("first", new TemplateDataItem()
                    {
                        value = "您好，您有新的支付订单请及时处理！",
                        color = TemplateSendColor
                    });
                    data.Add("keyword1", new TemplateDataItem()
                    {
                        value = shopOrder.OrderNumber.ToString(),
                        color = TemplateSendColor
                    });
                    data.Add("keyword2", new TemplateDataItem()
                    {
                        value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        color = TemplateSendColor
                    });
                    data.Add("remark", new TemplateDataItem()
                    {
                        value = remark,
                        color = TemplateSendColor
                    });
                    #endregion
                }

                string detailUrl = $"http://manager.91huichihuihe.com/api/ShopOrderSetAPI/Manager/GetShopOrderView?orderid={shopOrder.Id}&openid={receiverOpenId}";
                ShopOrderTemplateModel templateModel = new ShopOrderTemplateModel()
                {
                    touser = receiverOpenId,
                    template_id = templateId,
                    url = detailUrl,
                    data = data
                };


                JsonSerializer jsonSerializer = new JsonSerializer();

                var responseWait = httpClient.PostAsync(url, new StringContent(JsonConvert.SerializeObject(templateModel)));
                responseWait.Wait();
                var response = responseWait.Result;

                if (response.IsSuccessStatusCode)
                {
                    var reContentWait = response.Content.ReadAsStringAsync();
                    reContentWait.Wait();
                    var reContent = reContentWait.Result;
                    //if (JObject.Parse(reContent).TryGetValue("errcode", out JToken errcode))
                    //{
                    //    if (errcode.Value<int>() == 0)
                    //    {
                    //        shopOrder.IsSend = true;
                    //    }
                    //    else
                    //    {
                    //        shopOrder.IsSend = false;
                    //    }
                    //}
                    //else
                    //{
                    //    shopOrder.IsSend = false;
                    //}
                    return true;
                }
                else
                {
                    var reContentWait = response.Content.ReadAsStringAsync();
                    reContentWait.Wait();
                    var reContent = reContentWait.Result;
                    logger.LogInformation("模板消息出错：{0}", reContent);
                    return false;
                }
            }
            catch (Exception e)
            {
                logger.LogInformation("模板消息出错：{0}", e.Message);
                return false;
            }
        }

        /// <summary>
        /// 拼团订单推送
        /// </summary>
        /// <param name="shopOrder"></param>
        /// <param name="wechatOptions"></param>
        /// <param name="receiverOpenId"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        static bool SendTemplateMessage(this ConglomerationOrder conglomerationOrder, WechatTemplateSendOptions wechatOptions,
                                                  string receiverOpenId, ILogger logger)
        {
            logger.LogInformation($"推送拼团订单conglomerationOrderId:{conglomerationOrder.Id}");

            try
            {
                if (string.IsNullOrEmpty(receiverOpenId)) return false;
                string accessToken = AccessTokenContainer.GetAccessTokenResult(wechatOptions.AppId).access_token;
                //logger.LogInformation("推送的accesstoken:{0}", accessToken);
                string url = wechatOptions.SendUrl + accessToken;

                TemplateData data = new TemplateData();
                #region 订单参数
                string remark = "";
                data.Add("first", new TemplateDataItem()
                {
                    value = "您好，您有新的支付订单请及时处理！",
                    color = TemplateSendColor
                });
                data.Add("keyword1", new TemplateDataItem()
                {
                    value = conglomerationOrder.OrderNumber.ToString(),
                    color = TemplateSendColor
                });
                data.Add("keyword2", new TemplateDataItem()
                {
                    value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    color = TemplateSendColor
                });
                data.Add("remark", new TemplateDataItem()
                {
                    value = remark,
                    color = TemplateSendColor
                });
                #endregion
                string detailUrl = $"http://manager.91huichihuihe.com//api/ShopOrderSetAPI/Manager/GetConglomerationOrderView?orderid={conglomerationOrder.Id}&openid={receiverOpenId}";
                ShopOrderTemplateModel templateModel = new ShopOrderTemplateModel()
                {
                    touser = receiverOpenId,
                    template_id = wechatOptions.ServiceTemplateId,
                    url = detailUrl,
                    data = data
                };


                JsonSerializer jsonSerializer = new JsonSerializer();

                var responseWait = httpClient.PostAsync(url, new StringContent(JsonConvert.SerializeObject(templateModel)));
                responseWait.Wait();
                var response = responseWait.Result;

                if (response.IsSuccessStatusCode)
                {
                    var reContentWait = response.Content.ReadAsStringAsync();
                    reContentWait.Wait();
                    var reContent = reContentWait.Result;
                    if (JObject.Parse(reContent).TryGetValue("errcode", out JToken errcode))
                    {
                        if (errcode.Value<int>() == 0)
                        {
                            conglomerationOrder.IsSend = true;
                        }
                        else
                        {
                            conglomerationOrder.IsSend = false;
                        }
                    }
                    else
                    {
                        conglomerationOrder.IsSend = false;
                    }
                    return true;
                }
                else
                {
                    var reContentWait = response.Content.ReadAsStringAsync();
                    reContentWait.Wait();
                    var reContent = reContentWait.Result;
                    logger.LogInformation("模板消息出错：{0}", reContent);
                    return false;
                }
            }
            catch (Exception e)
            {
                logger.LogInformation("模板消息出错：{0}", e.Message);
                return false;
            }
        }

        public static void PrintOrder(PrintDbContext printDbContext, ShopDbContext db, ShopOrder shopOrder, string shopname, ILogger _logger)
        {

            _logger.LogInformation($"Remark{shopOrder.Remark}");
            if (shopOrder.IsPrint) return;
            try
            {
                List<ShopOrderItem> items = db.Query<ShopOrderItem>()
                    .Where(m => !m.IsDel)
                    .Where(m => m.ShopOrderId == shopOrder.Id)
                    .ToList();
                if (items.Count == 0) return;
                List<OrderInfo> orderInfo = items.Select(s => new OrderInfo()
                {
                    Name = $"{s.CommodityName}({s.SkuSummary})",
                    Price = Math.Round(s.SalePrice / 100d, 2),
                    Count = s.Count,
                    ComboConten = GetPrintComboContenParameter(db, s, _logger)
                }).ToList();
                string stringorderlist = JsonConvert.SerializeObject(orderInfo);

                List<Printer.Data.Printer> printers = printDbContext.Query<Printer.Data.Printer>()
                    .Where(s => !s.IsDel)
                    .Where(s => s.ShopID == shopOrder.ShopId && s.IsEnable).ToList();
                if (printers.Count == 0) return;

                PrintParameter parameter = GetPrintParameter(db, shopOrder);
                parameter.ShopName = shopname;
                parameter.List = orderInfo;
                parameter.IsTakeOut = shopOrder.IsTakeOut;
                //获取优惠
                if (shopOrder.MoneyOffRuleId != null && shopOrder.MoneyOffRuleId != 0)
                {
                    var shopOrderMoneyOffRules = db.ShopOrderMoneyOffRules.Find(shopOrder.MoneyOffRuleId);
                    parameter.ShopOrderMoneyOffRule = new ShopOrderMoneyOffRuleModel()
                    {
                        Discount = Math.Round(shopOrderMoneyOffRules.Discount / 100d, 2),
                        FullAmount = Math.Round(shopOrderMoneyOffRules.FullAmount / 100d, 2),
                        MoneyOffId = shopOrderMoneyOffRules.MoneyOffId,
                        MoneyOffName = db.ShopOrderMoneyOffs.Find(shopOrderMoneyOffRules.MoneyOffId).Name

                    };
                }
                //获取其他费用
                if (shopOrder.OtherFeeId != null && shopOrder.OtherFeeId != 0)
                {
                    var shopOrderMoneyOffs = db.ShopOrderOtherFees.Find(shopOrder.OtherFeeId);
                    parameter.ShopOrderOtherFee = new Dictionary<string, double>() {
                        { "餐盒费",Math.Round(shopOrderMoneyOffs.BoxFee / 100d, 2)},
                        { "配送费",Math.Round(shopOrderMoneyOffs.DeliveryFee / 100d, 2)}
                    };
                    if (TakeWay.自提.ToString().Equals(parameter.TakeWay))
                    {
                        parameter.ShopOrderOtherFee.Remove("配送费");
                    }
                }
                if (shopOrder.ShopOrderSelfHelpId.HasValue)  //自助点餐
                {
                    var selfHelp = db.GetSingle<ShopOrderSelfHelp>(shopOrder.ShopOrderSelfHelpId.Value);
                    if (selfHelp != null)
                    {
                        parameter.SelfHelpPrintParameter = new SelfHelpPrintParameter()
                        {
                            SelfHelpNumber = selfHelp.Number,
                            DingingWay = selfHelp.IsTakeOut ? "外带" : "堂食"
                        };
                    }
                }


                foreach (var item in printers)
                {
                    //StringBuilder postData = new StringBuilder("sn=" + item.SN);
                    parameter.SN = item.SN;
                    parameter.Times = item.Times + "";
                    PrintModel model = printDbContext.Query<PrintModel>().FirstOrDefault(s => s.ID == item.ModelID);
                    if (model == null) model = printDbContext.Set<PrintModel>().Find(2);
                    parameter.ModelContent = model.ModelContent;
                    PrinterBase @base = PrinterFactory.Create(item.PrinterType);//)
                    string temp = @base.PrinterRequest(parameter, item);
                    #region 数据库操作
                    PrintRecord record = new PrintRecord();
                    //处理接口返回数据
                    Dictionary<string, object> dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(temp);
                    //实体赋值
                    record.SN = item.SN;
                    record.OrderID = @base.GetOrderID(temp);// dic.ContainsKey("data") && dic["msg"].Equals("ok") ? dic["data"].ToString() : "未能成功打印";
                    record.Title = parameter.Title;
                    record.OrderList = stringorderlist;
                    record.TotalMoney = (float)parameter.TotalMoney;
                    record.Address = parameter.Address;
                    record.OrderName = parameter.OrderName;
                    record.Mobile = parameter.Mobile;
                    record.OrderTime = Convert.ToDateTime(parameter.OrderTime);
                    record.QRAddress = parameter.QRAddress;
                    record.Remark = shopOrder.Remark;
                    printDbContext.AddTo(record);
                    #endregion
                }
                shopOrder.IsPrint = true;
            }
            catch (Exception e)
            {
                _logger.LogError("打印机错误：{0}", e.Message);
            }
        }

        static PrintParameter GetPrintParameter(ShopDbContext db, ShopOrder shopOrder)
        {
            var shopMember = db.ShopMembers.FirstOrDefault(m => m.MemberId.Equals(shopOrder.MemberId) && !m.IsDel);
            PrintParameter parameter = new PrintParameter();
            parameter.PayTypeName = shopOrder.PayWay;
            if (shopMember != null)
            {
                parameter.Name = shopMember.Name;
                parameter.Phone = shopMember.Phone;
                parameter.Sex = shopMember.Sex;
            }
            parameter.PayTime = shopOrder.PayTime.Value;
            //是否外卖
            if (shopOrder.IsTakeOut)
            {
                parameter.Title = "外卖单";
                var takeout = db.Query<ShopOrderTakeout>()
                    .Where(m => !m.IsDel && m.ShopOrderId == shopOrder.Id)
                    .FirstOrDefault();
                parameter.TakeWay = takeout.TakeWay.ToString();
                parameter.PickupTime = takeout.PickupTime;
                if (takeout == null)
                {
                    parameter.Address = "用户未填写地址";
                    var memberPhone = db.Query<MemberPhone>()
                        .Where(m => !m.IsDel)
                        .Where(m => m.State == MemberPhoneState.已绑定)
                        .Where(m => m.MemberId == shopOrder.MemberId)
                        .FirstOrDefault();
                    parameter.Mobile = memberPhone?.Phone;
                }
                else
                {
                    parameter.Address = takeout.Address;
                    parameter.Mobile = takeout.Phone;
                    parameter.OrderName = takeout.Name;
                }
            }
            else if (shopOrder.ShopOrderSelfHelpId.HasValue)
            {
                parameter.Title = "自助点餐";
            }
            else
            {
                string shoppart;
                if (shopOrder.ShopPartId == null)
                    shoppart = "无桌号";
                else
                    shoppart = db.Query<ShopPart>()
                        .Where(m => m.Id == shopOrder.ShopPartId)
                        .FirstOrDefault()
                        .Title;
                parameter.Title = shoppart + "的点餐单";
            }
            parameter.ShopID = shopOrder.ShopId;
            parameter.TotalMoney = Math.Round(shopOrder.Amount / 100d, 2);
            if (shopOrder.Payment != null)
            {
                parameter.Payment = Math.Round((int)shopOrder.Payment / 100d, 2);
            }
            parameter.Remark = shopOrder.Remark;
            parameter.OrderID = shopOrder.OrderNumber;
            // parameter.takeDistributionType = shopOrder.takeDistributionType.ToString();
            return parameter;
        }

        /// <summary>
        /// 获取套餐内内容
        /// </summary>
        /// <param name="db"></param>
        /// <param name="shopOrderItem"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        static List<ComboConten> GetPrintComboContenParameter(ShopDbContext db, ShopOrderItem shopOrderItem, ILogger logger)
        {
            List<ComboConten> comboContens = new List<ComboConten>();
            try
            {
                var shopBrandCommodity = (
                                          from b in db.ShopCommoditySku
                                          where shopOrderItem.CommodityStockId == b.Id
                                          join c in db.ShopBrandCommoditySkus on b.SkuId equals c.Id
                                          join d in db.ShopBrandCommoditys on c.CommodityId equals d.Id
                                          where !b.IsDel && !d.IsDel
                                          select d).FirstOrDefault();
                if (shopBrandCommodity.CategoryId == 0)
                {

                    var shopOrderComboItem = db.ShopOrderComboItems.Where(m => m.Pid.Equals(shopBrandCommodity.Id) && !m.IsDel);
                    foreach (var item in shopOrderComboItem)
                    {
                        ComboConten comboConten = new ComboConten();
                        var shopBrandCommoditys = db.ShopBrandCommoditys.Find(item.CommodityId);
                        comboConten.Name = shopBrandCommoditys.Name;
                        comboConten.Count = 0;
                        comboContens.Add(comboConten);
                    }
                }

                return comboContens;
            }
            catch (Exception e)
            {
                logger.LogInformation($"获取套餐打印内容错误，错误信息：{e}");
                return comboContens;
            }
        }
    }


    public class ShopOrderTemplateModel
    {
        public string touser { get; set; }
        public string template_id { get; set; }
        public string url { get; set; }
        public TemplateData data { get; set; }
    }

    public class TemplateData : Dictionary<string, TemplateDataItem> { }

    public class TemplateDataItem
    {
        public string value { get; set; }
        public string color { get; set; }
    }

}
