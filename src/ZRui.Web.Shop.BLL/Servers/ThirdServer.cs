using AutoMapper;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ZRui.Web.BLL.Third;
using ZRui.Web.Data;

namespace ZRui.Web.BLL.Servers
{
    public class ThirdServer
    {
        ShopDbContext shopDb;
        ExThirdPartyDistributionParameter parameter;
        ThirdConfig thirdoptions;
        public ThirdServer(ShopDbContext shopDb, ThirdConfig options)
        {
            this.shopDb = ZRui.Web.BLL.DbContextFactory.ShopDb;
            this.parameter = new ExThirdPartyDistributionParameter(options);
            this.thirdoptions = options;

        }
        private static readonly HttpClient httpClient = new HttpClient();
        public const string ONLINEURL = "http://newopen.imdada.cn";
        public const string TESTURL = "http://newopen.qa.imdada.cn";

        public async Task<ThirdAddOrderResult> ThirdAddOrder(ThirdShopAddOrderModel model, ShopOrder order)
        {
            try
            {

                var parametermodel = new ThirdPartyDistributionParameterModel();

                parametermodel.source_id = GetMerchant(model.ShopId).DaDaShopId.ToString();
                shopDb.ThirdApiData.Add(new ThirdApiData()
                {
                    ShopId = model.ShopId,
                    DaDaShopId = Convert.ToInt32(parametermodel.source_id),
                    AddTime = DateTime.Now,
                    JsonData = JsonConvert.SerializeObject(order),
                    ResultData = "ThirdAddOrder-Info",
                    OrderId = model.origin_id
                });
                //parametermodel.source_id = "73753";
                // model.origin_id = "THIRD" + DateTime.Now.Ticks + CommonUtil.CreateNoncestr(5);
                var Data = JsonConvert.SerializeObject(model);
                parametermodel.body = Data;
                var data = parameter.ExThirdPartyDistributionParameterAction(parametermodel);
                var result = await httpClient.Post<ThirdAddOrderResult>($"{thirdoptions.Url}/api/order/addOrder", data);
                if (result.errorCode == 0 || result.status == "success")
                {
                    shopDb.ThirdOrder.Add(new Data.ThirdOrder()
                    {
                        OrderNumber = order.OrderNumber,
                        ShopId = model.ShopId,
                        ThirdType = ThirdType.达达配送,
                        OrderStatus = OrderStatus.待接单

                    });

                    shopDb.ThirdMoneyReport.Add(new Data.ThirdMoneyReport()
                    {
                        ShopId = model.ShopId,
                        OrderNumber = order.OrderNumber,
                        Amount = result.result.fee,
                        AddTime = DateTime.Now,
                        ProduceType = ProduceType.发起订单
                    });
                }
                shopDb.ThirdApiData.Add(new ThirdApiData()
                {
                    ShopId = model.ShopId,
                    DaDaShopId = Convert.ToInt32(parametermodel.source_id),
                    AddTime = DateTime.Now,
                    JsonData = Data,
                    ResultData = JsonConvert.SerializeObject(result),
                    OrderId = model.origin_id
                });
                shopDb.SaveChanges();
                return result;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

        }
        /// <summary>
        /// 查询运费预发布订单
        /// </summary>
        /// <param name="model"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public PreThirdAddOrderResult PreThirdAddOrder(ThirdShopAddOrderModel model)
        {
            try
            {
                var parametermodel = new ThirdPartyDistributionParameterModel();
                parametermodel.source_id = GetMerchant(model.ShopId).DaDaShopId.ToString();

                var Data = JsonConvert.SerializeObject(model);
                parametermodel.body = Data;
                var data = parameter.ExThirdPartyDistributionParameterAction(parametermodel);
                var result = httpClient.Post<PreThirdAddOrderResult>($"{thirdoptions.Url}/api/order/queryDeliverFee", data).Result;
                if (result.errorCode == 0 || result.status == "success")
                {
                    var queryfee = shopDb.ThirdApiData.Where(r => r.OrderId == model.origin_id && r.JsonData == "QueryFee");
                    if (queryfee.Count() <= 0)
                    {
                        shopDb.ThirdApiData.Add(new ThirdApiData()
                        {
                            ShopId = model.ShopId,
                            DaDaShopId = Convert.ToInt32(parametermodel.source_id),
                            AddTime = DateTime.Now,
                            JsonData = "QueryFee",
                            ResultData = JsonConvert.SerializeObject(result),
                            OrderId = model.origin_id
                        });
                    }

                    //发起预订单成功后处理发起订单
                    //var afterresult = AddAfterQuery(new Servers.AddAfterQuery() { deliveryNo = result.result.deliveryNo, OrderNumber = model.origin_id, ShopId = model.ShopId, Fee = result.result.fee });

                }
                shopDb.ThirdApiData.Add(new ThirdApiData()
                {
                    ShopId = model.ShopId,
                    DaDaShopId = Convert.ToInt32(parametermodel.source_id),
                    AddTime = DateTime.Now,
                    JsonData = Data,
                    ResultData = JsonConvert.SerializeObject(result),
                    OrderId = model.origin_id
                });
                shopDb.SaveChanges();
                return result;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

        }
        /// <summary>
        /// 回调处理预订单发单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public AddAfterResult CallbackAfter(AddAfterQuery model)
        {
            try
            {
                var parametermodel = new ThirdPartyDistributionParameterModel();
                parametermodel.source_id = GetMerchant(model.ShopId).DaDaShopId.ToString();
                var Data = JsonConvert.SerializeObject(model);
                parametermodel.body = Data;
                var data = parameter.ExThirdPartyDistributionParameterAction(parametermodel);
                var result = httpClient.Post<AddAfterResult>($"{thirdoptions.Url}/api/order/addAfterQuery", data).Result;
                if (result.errorCode == 0 || result.status == "success")
                {

                    shopDb.ThirdApiData.Add(new ThirdApiData()
                    {
                        ShopId = model.ShopId,
                        DaDaShopId = Convert.ToInt32(parametermodel.source_id),
                        AddTime = DateTime.Now,
                        JsonData = "CallbackAfter",
                        ResultData = JsonConvert.SerializeObject(result),
                        OrderId = model.OrderNumber
                    });
                    //第三方配送订单记录
                    shopDb.ThirdOrder.Add(new Data.ThirdOrder()
                    {
                        OrderNumber = model.OrderNumber,
                        ShopId = model.ShopId,
                        ThirdType = ThirdType.达达配送,
                        OrderStatus = OrderStatus.待接单

                    });
                    //发起订单报表记录
                    shopDb.ThirdMoneyReport.Add(new Data.ThirdMoneyReport()
                    {
                        ShopId = model.ShopId,
                        OrderNumber = model.OrderNumber,
                        Amount = model.Fee,
                        AddTime = DateTime.Now,
                        ProduceType = ProduceType.发起订单
                    });

                }
                shopDb.SaveChanges();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }


        public async Task<CThirdInfoQuery> ThirdOrderQuery(COrderInfoQueryModel model)
        {
            var parametermodel = new ThirdPartyDistributionParameterModel();
            parametermodel.source_id = GetMerchant(model.ShopId).DaDaShopId.ToString();
            //parametermodel.source_id = "73753";//测试
            var Data = JsonConvert.SerializeObject(model);
            parametermodel.body = Data;
            var data = parameter.ExThirdPartyDistributionParameterAction(parametermodel);
            var result = await httpClient.Post<CThirdInfoQueryResult>($"{thirdoptions.Url}/api/order/status/query", data);
            if (result.errorCode != 0 || result.status != "success")
            {
                // throw new Exception(result.msg);
                return new CThirdInfoQuery();
            }
            return result.result;

        }

        /// <summary>
        /// 余额查询
        /// </summary>
        /// <returns></returns>
        public async Task<CThirdRechargeQueryResult> ThirdShopRechargeQuery(CThirdShopRechargeQueryModel model)
        {
            var parametermodel = new ThirdPartyDistributionParameterModel();
            parametermodel.source_id = GetMerchant(model.ShopId).DaDaShopId.ToString();
            var Data = JsonConvert.SerializeObject(model);
            parametermodel.body = Data;
            var data = parameter.ExThirdPartyDistributionParameterAction(parametermodel);
            var result = await httpClient.Post<CThirdRechargeQueryResult>($"{thirdoptions.Url}/api/balance/query", data);
            if (result.errorCode != 0 || result.status != "success")
            {
                throw new Exception(result.msg);
            }
            return result;
        }

        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<CThirdCancelResult> ThirdFormalCancel(CFormalCancel model)
        {
            var parametermodel = new ThirdPartyDistributionParameterModel();
            parametermodel.source_id = GetMerchant(model.ShopId).DaDaShopId.ToString();
            // parametermodel.source_id = "73753";//测试
            var Data = JsonConvert.SerializeObject(model);
            parametermodel.body = Data;
            var data = parameter.ExThirdPartyDistributionParameterAction(parametermodel);
            var result = await httpClient.Post<CThirdCancelResult>($"{thirdoptions.Url}/api/order/formalCancel", data);
            if (result.errorCode != 0 || result.status != "success")
            {
                throw new Exception(result.msg);
            }

            shopDb.ThirdMoneyReport.Add(new Data.ThirdMoneyReport()
            {
                OrderNumber = model.order_id,
                Amount = result.result.deduct_fee,
                AddTime = DateTime.Now,
                ProduceType = ProduceType.取消订单
            });
            shopDb.SaveChanges();


            return result;

        }

        /// <summary>
        /// 取消订单原因
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<CThirdCancelReasonsResult> ThirdFormalCancel()
        {
            var parametermodel = new ThirdPartyDistributionParameterModel();
            // parametermodel.source_id = "73753";//测试
            parametermodel.body = "{}";
            var data = parameter.ExThirdPartyDistributionParameterAction(parametermodel);
            var result = await httpClient.Post<CThirdCancelReasonsResult>($"{thirdoptions.Url}/api/order/cancel/reasons", data);
            if (result.errorCode != 0 || result.status != "success")
            {
                throw new Exception(result.msg);
            }
            return result;

        }


        public Merchant GetMerchant(int shopid)
        {
            var merchant = shopDb.Merchant.FirstOrDefault(r => r.ShopId == shopid);
            if (merchant == null)
            {
                throw new Exception("请先开户");
            }
            return merchant;
        }


    }
    /// <summary>
    /// 发单
    /// </summary>
    public class ThirdShopAddOrderModel
    {
        /// <summary>
        /// 商户Id
        /// </summary>
        public int ShopId { get; set; }

        /// <summary>
        /// 门店编号，门店创建后可在门店列表和单页查看
        /// </summary>
        public string shop_no { get; set; }
        /// <summary>
        /// 第三方订单ID
        /// </summary>
        public string origin_id { get; set; }
        /// <summary>
        /// 订单所在城市的code
        /// </summary>
        public string city_code { get; set; }
        /// <summary>
        /// 订单金额
        /// </summary>
        public double cargo_price { get; set; }
        /// <summary>
        /// 是否需要垫付 1:是 0:否 (垫付订单金额，非运费)
        /// </summary>
        public int is_prepay { get; set; }
        /// <summary>
        /// 收货人姓名
        /// </summary>
        public string receiver_name { get; set; }
        /// <summary>
        /// 收货人地址
        /// </summary>
        public string receiver_address { get; set; }
        /// <summary>
        /// 收货人地址维度（高德坐标系）
        /// </summary>
        public double receiver_lat { get; set; }
        /// <summary>
        /// 收货人地址经度（高德坐标系）
        /// </summary>
        public double receiver_lng { get; set; }
        /// <summary>
        /// 回调URL
        /// </summary>
        public string callback { get; set; }
        /// <summary>
        /// 收货人手机号（手机号和座机号必填一项）
        /// </summary>
        public string receiver_phone { get; set; }
        /// <summary>
        /// 收货人座机号（手机号和座机号必填一项）
        /// </summary>
        public string receiver_tel { get; set; }
        /// <summary>
        /// 小费（单位：元，精确小数点后一位）
        /// </summary>
        public double tips { get; set; }
        /// <summary>
        /// 订单备注
        /// </summary>
        public string info { get; set; }
        /// <summary>
        /// 订单商品类型：食品小吃-1,饮料-2,鲜花-3,文印票务-8,便利店-9,水果生鲜-13,同城电商-19, 医药-20,蛋糕-21,酒品-24,小商品市场-25,服装-26,汽修零配-27,数码-28,小龙虾-29, 其他-5
        /// </summary>
        public int cargo_type { get; set; }
        /// <summary>
        /// 订单重量（单位：Kg）
        /// </summary>
        public double cargo_weight { get; set; }
        /// <summary>
        /// 订单商品数量
        /// </summary>
        public int cargo_num { get; set; }
        /// <summary>
        /// 发票抬头
        /// </summary>
        public string invoice_title { get; set; }
        /// <summary>
        /// 订单来源标示（该字段可以显示在达达app订单详情页面，只支持字母，最大长度为10）
        /// </summary>
        public string origin_mark { get; set; }
        /// <summary>
        /// 订单来源编号（该字段可以显示在达达app订单详情页面，支持字母和数字，最大长度为30）
        /// </summary>
        public string origin_mark_no { get; set; }
        /// <summary>
        /// 是否使用保价费（0：不使用保价，1：使用保价； 同时，请确保填写了订单金额（cargo_price））
        // 商品保价费(当商品出现损坏，可获取一定金额的赔付)
        //保费=配送物品实际价值* 费率（5‰），配送物品价值及最高赔付不超过10000元， 最高保费为50元（物品价格最小单位为100元，不足100元部分按100元认定，保价费向上取整数， 如：物品声明价值为201元，保价费为300元*5‰=1.5元，取整数为2元。）
        //若您选择不保价，若物品出现丢失或损毁，最高可获得平台30元优惠券。 （优惠券直接存入用户账户中）。
        /// </summary>
        public int is_use_insurance { get; set; }
        /// <summary>
        /// 收货码（0：不需要；1：需要。收货码的作用是：骑手必须输入收货码才能完成订单妥投）
        /// </summary>
        public int is_finish_code_needed { get; set; }
        /// <summary>
        /// 预约发单时间（预约时间unix时间戳(10位),精确到分;整10分钟为间隔，并且需要至少提前20分钟预约。）
        /// </summary>
        public int delay_publish_time { get; set; }
        /// <summary>
        /// 是否选择直拿直送（0：不需要；1：需要。选择直拿只送后，同一时间骑士只能配送此订单至完成，同时，也会相应的增加配送费用）
        /// </summary>
        public int is_direct_delivery { get; set; }
    }

    public class AddAfterQuery
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderNumber { get; set; }
        /// <summary>
        /// 达达平台订单号 发单用
        /// </summary>
        public string deliveryNo { get; set; }
        public int ShopId { get; set; }

        public double Fee { get; set; }
    }

    /// <summary>
    /// 发单
    /// </summary>
    public class ThirdAddOrderResult
    {
        public string status { get; set; }
        public int errorCode { get; set; }
        public int code { get; set; }
        public string msg { get; set; }
        /// <summary>
        /// 发单业务参数
        /// </summary>
        public ThirdAddOrder result { get; set; }


    }

    /// <summary>
    /// 回调发单
    /// </summary>
    public class CallbackAfterOrder
    {
        public string status { get; set; }
        public int errorCode { get; set; }
        public int code { get; set; }
        public string msg { get; set; }
        /// <summary>
        /// 预发单业务参数
        /// </summary>
        public PreThirdAddOrder result { get; set; }


    }

    /// <summary>
    /// 预发单
    /// </summary>
    public class PreThirdAddOrderResult
    {
        public string status { get; set; }
        public int errorCode { get; set; }
        public int code { get; set; }
        public string msg { get; set; }
        /// <summary>
        /// 预发单业务参数
        /// </summary>
        public PreThirdAddOrder result { get; set; }


    }

    /// <summary>
    /// 预发单
    /// </summary>
    public class AddAfterResult
    {
        public string status { get; set; }
        public int errorCode { get; set; }
        public int code { get; set; }
        public string msg { get; set; }
        public string result { get; set; }


    }

    public enum ExSource
    {
        发起支付 = 0,
        支付成功 = 1
    }
    /// <summary>
    /// 发单返回业务参数
    /// </summary>
    public class ThirdAddOrder
    {
        /// <summary>
        /// 配送距离（米）
        /// </summary>
        public double distance { get; set; }
        /// <summary>
        /// 实际运费（元）
        /// </summary>
        public double fee { get; set; }
        /// <summary>
        /// 运费（元）
        /// </summary>
        public double deliverFee { get; set; }
        /// <summary>
        /// 优惠券费用(单位：元)
        /// </summary>
        public double couponFee { get; set; }
        /// <summary>
        /// 小费(单位：元)
        /// </summary>
        public double tips { get; set; }
        /// <summary>
        /// 保价费(单位：元)
        /// </summary>
        public double insuranceFee { get; set; }

    }

    /// <summary>
    /// 预发单返回业务参数
    /// </summary>
    public class PreThirdAddOrder
    {
        /// <summary>
        /// 配送距离（米）
        /// </summary>
        public double distance { get; set; }

        public string deliveryNo { get; set; }
        /// <summary>
        /// 实际运费（元）
        /// </summary>
        public double fee { get; set; }
        /// <summary>
        /// 运费（元）
        /// </summary>
        public double deliverFee { get; set; }
        /// <summary>
        /// 优惠券费用(单位：元)
        /// </summary>
        public double couponFee { get; set; }
        /// <summary>
        /// 小费(单位：元)
        /// </summary>
        public double tips { get; set; }
        /// <summary>
        /// 保价费(单位：元)
        /// </summary>
        public double insuranceFee { get; set; }

    }





}
