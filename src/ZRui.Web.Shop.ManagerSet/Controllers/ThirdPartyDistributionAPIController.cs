using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ZRui.Web.BLL;
using ZRui.Web.BLL.Servers;
using ZRui.Web.BLL.Third;
using ZRui.Web.Common;
using ZRui.Web.Controllers.Base;
using ZRui.Web.Data;
using ZRui.Web.Models;
using ZRui.Web.Models.ThirdPartyModel;
using ZRui.Web.Utils;

namespace ZRui.Web.Controllers
{
    [Microsoft.AspNetCore.Cors.EnableCors("AllowTestOrigin")]
    [Route("api/[controller]/Manager/[action]")]
    public class ThirdPartyDistributionAPIController : ThirdShopApiControllerBase
    {
        ShopDbContext shopDb;
        ExThirdPartyDistributionParameter parameter;
        ThirdConfig thirdConfig;
        private IMapper _mapper { get; set; }
        public ThirdPartyDistributionAPIController(ShopDbContext shopDb, IOptions<ThirdConfig> poptions, IMapper mapper) : base(shopDb)
        {
            this.shopDb = shopDb;
            this.thirdConfig = poptions.Value;
            this.parameter = new ExThirdPartyDistributionParameter(thirdConfig);
            this._mapper = mapper;
        }
        private static readonly HttpClient httpClient = new HttpClient();
        /*
        测试域名：newopen.qa.imdada.cn
        线上域名：newopen.imdada.cn 
        */
        /// <summary>
        /// 达达开户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<APIResult> ThirdMerchantAdd([FromBody] MerchantModel model)
        {
            var shop = shopDb.Shops.FirstOrDefault(r => r.Id == model.ShopId && !r.IsDel);
            if (shop == null) throw new Exception("未找到商户");
            var parametermodel = new ThirdPartyDistributionParameterModel();
            parametermodel.source_id = "";
            var Data = JsonConvert.SerializeObject(model);
            parametermodel.body = Data;
            var data = parameter.ExThirdPartyDistributionParameterAction(parametermodel);
            var result = await httpClient.Post<ThirdResultModel>($"{thirdConfig.Url}/merchantApi/merchant/add", data);
            if (result.errorCode != 0 || result.status != "success")
            {
                throw new Exception(result.msg);
            }
            Merchant merchant = new Merchant();
            merchant.CityName = model.city_name;
            merchant.ContactName = model.contact_name;
            merchant.ContactPhone = model.contact_phone;
            merchant.DaDaShopId = result.result;
            merchant.Email = model.email;
            merchant.EnterpriseAddress = model.enterprise_address;
            merchant.EnterpriseName = model.enterprise_name;
            merchant.Mobile = model.mobile;
            merchant.ShopId = shop.Id;
            shopDb.Merchant.Add(merchant);
            shopDb.SaveChanges();
            return Success(result);
        }
        /// <summary>
        /// 达达商户详情
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public APIResult ThirdMerchantDetile([FromBody] ThirdShopIdModel model)
        {
            var merchant = shopDb.Merchant.FirstOrDefault(r => r.ShopId == model.ShopId);
            if (merchant == null) throw new Exception("未找到该商户");
            return Success(merchant);
        }
        /// <summary>
        /// 添加门店
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<APIResult> ThirdShopAdd([FromBody] ThirdShopArgs model)
        {
            if (model.ThirdShopModel == null) throw new Exception("请输入门店信息");
            double lat = model.ThirdShopModel.FirstOrDefault().lat;
            double lng = model.ThirdShopModel.FirstOrDefault().lng;
            var converResult = BaiduMapUtil.GetBdToGd(model.ThirdShopModel.FirstOrDefault().lng, model.ThirdShopModel.FirstOrDefault().lat);
            model.ThirdShopModel.FirstOrDefault().lng = converResult.x;
            model.ThirdShopModel.FirstOrDefault().lat = converResult.y;

            var thirdshopcount = shopDb.ThirdShop.Where(r => r.ShopId == model.ShopId && r.Status == ThirdShop.ShopStatus.门店激活).Count();
            if (thirdshopcount > 0) throw new Exception("暂不支持多门店");
            var parametermodel = new ThirdPartyDistributionParameterModel();
            parametermodel.source_id = GetMerchant(model.ShopId).DaDaShopId.ToString(); //"73753";// 
            var Data = JsonConvert.SerializeObject(model.ThirdShopModel);
            parametermodel.body = Data;
            var data = parameter.ExThirdPartyDistributionParameterAction(parametermodel);
            var result = await httpClient.Post<ThirdShopResultModel>($"{thirdConfig.Url}/api/shop/add", data);
            if (result.code != 0 || result.status != "success")
            {
                throw new Exception(result.msg + "错误信息：" + result.result.failedList.FirstOrDefault().msg);
            }
            foreach (var item in model.ThirdShopModel)
            {


                shopDb.ThirdShop.Add(new ThirdShop()
                {
                    ShopId = model.ShopId,
                    AreaName = item.area_name,
                    Business = item.business,
                    CityName = item.city_name,
                    ContactName = item.contact_name,
                    IdCard = item.id_card,
                    Lat = lat,
                    Lng = lng,
                    OriginShopId = item.origin_shop_id == null ? result.result.successList.FirstOrDefault().originShopId : item.origin_shop_id,
                    Phone = item.phone,
                    StationAddress = item.station_address,
                    StationName = item.station_name,
                    PassWord = item.password,
                    UserName = item.username,
                    Status = ThirdShop.ShopStatus.门店激活
                });
                shopDb.SaveChanges();

            }

            return Success(result);
        }
        /// <summary>
        /// 更新门店
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<APIResult> ThirdShopUpdate([FromBody] ThirdShopUpdateModel model)
        {

            double lat = model.lat;
            double lng = model.lng;

            var converResult = BaiduMapUtil.GetBdToGd(model.lng, model.lat);

            var parametermodel = new ThirdPartyDistributionParameterModel();

            parametermodel.source_id = GetMerchant(model.ShopId).DaDaShopId.ToString();
            model.lng = converResult.x;
            model.lat = converResult.y;

            var Data = JsonConvert.SerializeObject(model);
            parametermodel.body = Data;
            var data = parameter.ExThirdPartyDistributionParameterAction(parametermodel);
            var result = await httpClient.Post<ThirdResultModel>($"{thirdConfig.Url}/api/shop/update", data);
            if (result.errorCode != 0 || result.status != "success")
            {
                throw new Exception(result.msg);
            }
            var thirdshop = shopDb.ThirdShop.FirstOrDefault(r => r.OriginShopId == model.origin_shop_id);
            if (thirdshop == null) throw new Exception("找不到该门店");

            thirdshop.ShopId = model.ShopId;
            thirdshop.AreaName = model.area_name;
            thirdshop.Business = model.business;
            thirdshop.CityName = model.city_name;
            thirdshop.ContactName = model.contact_name;
            thirdshop.Lat = lat;
            thirdshop.Lng = lng;
            thirdshop.OriginShopId = model.new_shop_id == null ? model.origin_shop_id : model.new_shop_id;
            thirdshop.Phone = model.phone;
            thirdshop.StationAddress = model.station_address;
            thirdshop.StationName = model.station_name;
            // thirdshop.Status = model.status;
            shopDb.SaveChanges();
            return Success(result);
        }
        /// <summary>
        /// 门店详情
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<APIResult> ThirdShopDetile([FromBody] ThirdShopDetail model)
        {
            var parametermodel = new ThirdPartyDistributionParameterModel();
            parametermodel.source_id = GetMerchant(model.ShopId).DaDaShopId.ToString();
            var Data = JsonConvert.SerializeObject(model);
            parametermodel.body = Data;
            var data = parameter.ExThirdPartyDistributionParameterAction(parametermodel);
            var result = await httpClient.Post<ThirdShopInfoResult>($"{thirdConfig.Url}/api/shop/detail", data);
            return Success(result);
        }
        /// <summary>
        /// 门店列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult ThirdShopList([FromBody] GetPagedListBaseModel model)
        {
            var list = shopDb.ThirdShop
                .Where(r => r.ShopId == model.ShopId)
                .ToPagedList(model.PageIndex, model.PageSize);
            var result = _mapper.Map<PagedList<ThirdShopModel>>(list);
            result.PageIndex = list.PageIndex;
            result.PageSize = list.PageSize;
            result.TotalItemCount = list.TotalItemCount;

            return Success(new
            {
                PageIndex = list.PageIndex,
                PageSize = list.PageSize,
                TotalCount = list.TotalItemCount,
                Items = result.ToList()
            });
        }

        /// <summary>
        /// 充值列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult ThirdRechargeList([FromBody] GetPagedListBaseModel model)
        {
            var list = shopDb.ThirdRecharge
                .Where(r => r.ShopId == model.ShopId)
                .OrderByDescending(r => r.AddTime)
                .ToPagedList(model.PageIndex, model.PageSize);
            var result = _mapper.Map<PagedList<ThirdShopRechargeDto>>(list);
            result.PageIndex = list.PageIndex;
            result.PageSize = list.PageSize;
            result.TotalItemCount = list.TotalItemCount;

            return Success(new
            {
                PageIndex = list.PageIndex,
                PageSize = list.PageSize,
                TotalCount = list.TotalItemCount,
                Items = result.ToList()
            });
        }

        /// <summary>
        /// 配送订单列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public APIResult ThirdOrderList([FromBody] GetPagedListBaseModel model)
        {
            var list = shopDb.ThirdOrder
                .Where(r => r.ShopId == model.ShopId).OrderByDescending(r => r.UpdateTime)
                .ToPagedList(model.PageIndex, model.PageSize);
            var result = _mapper.Map<PagedList<ThirdOrderModel>>(list);
            result.PageIndex = list.PageIndex;
            result.PageSize = list.PageSize;
            result.TotalItemCount = list.TotalItemCount;

            return Success(new
            {
                PageIndex = list.PageIndex,
                PageSize = list.PageSize,
                TotalCount = list.TotalItemCount,
                Items = result.ToList()
            });
        }

        /// <summary>
        /// 费用报表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult ThirdMoneyReport([FromBody] GetPagedListBaseModel model)
        {
            var list = shopDb.ThirdMoneyReport
                .Where(r => r.ShopId == model.ShopId)
                .OrderByDescending(r => r.AddTime)
                .ToPagedList(model.PageIndex, model.PageSize);
            var result = _mapper.Map<PagedList<ThirdMoneyReportModel>>(list);
            result.PageIndex = list.PageIndex;
            result.PageSize = list.PageSize;
            result.TotalItemCount = list.TotalItemCount;
            return Success(new
            {
                PageIndex = list.PageIndex,
                PageSize = list.PageSize,
                TotalCount = list.TotalItemCount,
                Items = result.ToList()
            });
        }

        /// <summary>
        /// 生成充值链接
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<APIResult> ThirdShopRecharge([FromBody]ThirdShopRechargeModel model)
        {
            if (model == null) throw new Exception("参数有误");
            if (model.amount <= 0) throw new Exception("充值金额必须大于0");

            var RCode = "RCODE" + DateTime.Now.Ticks + CommonUtil.CreateNoncestr(5);
            var parametermodel = new ThirdPartyDistributionParameterModel();
            parametermodel.source_id = GetMerchant(model.ShopId).DaDaShopId.ToString();
            model.notify_url = string.Format(thirdConfig.RechargeUrl, model.ShopId, parametermodel.source_id, model.amount, RCode);
            var Data = JsonConvert.SerializeObject(model);
            parametermodel.body = Data;
            var data = parameter.ExThirdPartyDistributionParameterAction(parametermodel);
            var result = await httpClient.Post<ThirdRechargeResult>($"{thirdConfig.Url}/api/recharge", data);
            if (result.errorCode != 0 || result.status != "success")
            {
                throw new Exception(result.msg);
            }
            return Success(result);
        }
        [HttpGet]
        [Authorize]
        public async Task ThirdRechargeCallback(int ShopId, int DaDaShopId, decimal Amount, string RCode)
        {
            string html = "<!doctype html><html lang = \"en\" ><head><meta charset = \"UTF-8\"><meta name = \"viewport\" content = \"width=device-width,initial-scale=1 user-scalable=0\" /><title></title><script>setTimeout('window.close()', 2000);</script></head><body><p style=\"font - size: 36px;margin: 50px;\">{0}</p></body></html>";
            var thirdrecharge = shopDb.ThirdRecharge.FirstOrDefault(r => r.VerificationCode == RCode);
            if (thirdrecharge == null)
            {
                shopDb.ThirdRecharge.Add(new ThirdRecharge()
                {
                    ShopId = ShopId,
                    DaDaShopId = DaDaShopId,
                    Amount = Amount,
                    AddTime = DateTime.Now,
                    RechargeType = RechargeType.运费账户,
                    VerificationCode = RCode

                });
                shopDb.SaveChanges();
                //   Response.Redirect($"http://managertest.91huichihuihe.com/api/ThirdPartyDistributionAPI/Manager/ThirdRechargeSuccess?RCode={RCode}");
            }
            //else
            //{
            //  //  Response.Redirect($"http://managertest.91huichihuihe.com/api/ThirdPartyDistributionAPI/Manager/ThirdRechargeAlreadySuccess?RCode={RCode}");

            //}
            string resultStr = string.Format(html, "充值成功");
            Response.ContentType = "text/html";
            var data = Encoding.UTF8.GetBytes(resultStr);
            await Response.Body.WriteAsync(data, 0, data.Length);
        }
        [HttpGet]
        public APIResult ThirdRechargeSuccess(string RCode)
        {
            return Success($"充值成功");
        }

        [HttpGet]
        public APIResult ThirdRechargeAlreadySuccess(string RCode)
        {
            return Success($"充值失败{RCode}");
        }

        /// <summary>
        /// 余额查询
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<APIResult> ThirdShopRechargeQuery([FromBody]ThirdShopRechargeQueryModel model)
        {
            var parametermodel = new ThirdPartyDistributionParameterModel();
            parametermodel.source_id = GetMerchant(model.ShopId).DaDaShopId.ToString();
            var Data = JsonConvert.SerializeObject(model);
            parametermodel.body = Data;
            var data = parameter.ExThirdPartyDistributionParameterAction(parametermodel);
            var result = await httpClient.Post<ThirdRechargeQueryResult>($"{thirdConfig.Url}/api/balance/query", data);
            if (result.errorCode != 0 || result.status != "success")
            {
                throw new Exception(result.msg);
            }
            return Success(result.result);
        }


        /// <summary>
        /// 订单详情查询
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<APIResult> ThirdShopOrderInfoQuery([FromBody]OrderInfoQueryModel model)
        {
            var parametermodel = new ThirdPartyDistributionParameterModel();
            parametermodel.source_id = GetMerchant(model.ShopId).DaDaShopId.ToString();
            //parametermodel.source_id = "73753";//测试
            var Data = JsonConvert.SerializeObject(model);
            parametermodel.body = Data;
            var data = parameter.ExThirdPartyDistributionParameterAction(parametermodel);
            var result = await httpClient.Post<ThirdInfoQueryResult>($"{thirdConfig.Url}/api/order/status/query", data);
            if (result.errorCode != 0 || result.status != "success")
            {
                throw new Exception(result.msg);
            }
            return Success(result.result);
        }
        /// <summary>
        /// 达达发单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<APIResult> ThirdAddOrder([FromBody] Models.ThirdPartyModel.ThirdShopAddOrderModel model)
        {
            var parametermodel = new ThirdPartyDistributionParameterModel();
            //
            parametermodel.source_id = GetMerchant(model.ShopId).DaDaShopId.ToString();
            parametermodel.source_id = "73753";
            model.origin_id = "THIRD" + DateTime.Now.Ticks + CommonUtil.CreateNoncestr(5);
            var Data = JsonConvert.SerializeObject(model);
            parametermodel.body = Data;
            var data = parameter.ExThirdPartyDistributionParameterAction(parametermodel);
            var result = await httpClient.Post<Models.ThirdPartyModel.ThirdAddOrderResult>($"{thirdConfig.Url}/api/order/addOrder", data);
            if (result.errorCode != 0 || result.status != "success")
            {
                throw new Exception(result.msg);
            }

            shopDb.ThirdApiData.Add(new ThirdApiData()
            {
                ShopId = model.ShopId,
                DaDaShopId = 73753,
                AddTime = DateTime.Now,
                JsonData = Data,
                ResultData = JsonConvert.SerializeObject(result),
                OrderId = model.origin_id
            });
            shopDb.SaveChanges();
            return Success(result);
        }
        /// <summary>
        /// 模拟接口
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<APIResult> OrderAccept([FromBody] OrderInfoQueryModel model)
        {
            var parametermodel = new ThirdPartyDistributionParameterModel();
            parametermodel.source_id = GetMerchant(model.ShopId).DaDaShopId.ToString();
            parametermodel.source_id = "73753";//测试
            var Data = JsonConvert.SerializeObject(model);
            parametermodel.body = Data;
            var data = parameter.ExThirdPartyDistributionParameterAction(parametermodel);
            var result = await httpClient.Post<ThirdObjectResult>($"{thirdConfig.Url}/api/order/accept", data);//模拟接单
                                                                                                               //var result = await httpClient.Post<ThirdObjectResult>($"{thirdConfig.Url}/api/order/fetch", data);//模拟取货
                                                                                                               // var result = await httpClient.Post<ThirdObjectResult>($"{thirdConfig.Url}/api/order/finish", data);//模拟完成订单
                                                                                                               //var result = await httpClient.Post<ThirdObjectResult>($"{thirdConfig.Url}/api/order/cancel", data);//模拟取消订单
                                                                                                               //var result = await httpClient.Post<ThirdObjectResult>($"{thirdConfig.Url}/api/order/expire", data);//模拟订单过期
            return Success(result);
        }



        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<APIResult> ThirdFormalCancel([FromBody] FormalCancel model)
        {
            var parametermodel = new ThirdPartyDistributionParameterModel();
            parametermodel.source_id = GetMerchant(model.ShopId).DaDaShopId.ToString();
            // parametermodel.source_id = "73753";//测试
            var Data = JsonConvert.SerializeObject(model);
            parametermodel.body = Data;
            var data = parameter.ExThirdPartyDistributionParameterAction(parametermodel);
            var result = await httpClient.Post<ThirdCancelResult>($"{thirdConfig.Url}/api/order/formalCancel", data);
            if (result.errorCode != 0 || result.status != "success")
            {
                throw new Exception(result.msg);
            }

            shopDb.ThirdMoneyReport.Add(new Data.ThirdMoneyReport()
            {
                ShopId = model.ShopId,
                OrderNumber = model.order_id,
                Amount = result.result.deduct_fee,
                AddTime = DateTime.Now,
                ProduceType = ProduceType.取消订单
            });
            shopDb.SaveChanges();


            return Success(result);

        }

        /// <summary>
        /// 取消订单原因
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<APIResult> ThirdFormalCancel()
        {
            var parametermodel = new ThirdPartyDistributionParameterModel();
            parametermodel.source_id = "10264";//测试
            parametermodel.body = "{}";
            var data = parameter.ExThirdPartyDistributionParameterAction(parametermodel);
            var result = await httpClient.Post<ThirdCancelReasonsResult>($"{thirdConfig.Url}/api/order/cancel/reasons", data);
            if (result.errorCode != 0 || result.status != "success")
            {
                throw new Exception(result.msg);
            }
            return Success(result);

        }
        /// <summary>
        /// 回调
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public APIResult CallBack([FromBody] ThirdOrdersModel model)
        {
            //回调查询订单
            var order = shopDb.ShopOrders.FirstOrDefault(r => r.OrderNumber == model.order_id);
            if (order != null)
            {
                shopDb.ThirdApiData.Add(new ThirdApiData()
                {
                    ShopId = order.ShopId,
                    DaDaShopId = 73753,
                    AddTime = DateTime.Now,
                    JsonData = "CallBack-OrderInfo",
                    ResultData = JsonConvert.SerializeObject(order),
                    OrderId = model.order_id
                });
                order.Status = ExThirdOrderResultStatus.ExStatus(model.order_status);

                //if (model.order_status == OrderStatus.已取消)
                //{
                //    shopDb.ThirdMoneyReport.Add(new Data.ThirdMoneyReport()
                //    {
                //        ShopId = order.ShopId,
                //        OrderNumber = model.order_id,
                //        Amount = 0,
                //        AddTime = DateTime.Now,
                //        ProduceType = ProduceType.取消订单
                //    });
                //}

                var thirdorder = shopDb.ThirdOrder.FirstOrDefault(r => r.OrderNumber == model.order_id);
                if (thirdorder == null)
                {
                    shopDb.ThirdOrder.Add(new ThirdOrder()
                    {
                        ClientId = model.client_id,
                        OrderNumber = model.order_id,
                        OrderStatus = model.order_status,
                        CancelFrom = model.cancel_from,
                        CancelReason = model.cancel_reason,
                        Signature = model.signature,
                        UpdateTime = model.update_time,
                        DmId = model.dm_id,
                        DmName = model.dm_name,
                        DmMobile = model.dm_mobile,
                        ShopId = order.ShopId,
                        ThirdType = ThirdType.达达配送
                    });
                }
                else
                {
                    thirdorder.ClientId = model.client_id;
                    thirdorder.OrderNumber = model.order_id;
                    thirdorder.OrderStatus = model.order_status;
                    thirdorder.CancelFrom = model.cancel_from;
                    thirdorder.CancelReason = model.cancel_reason;
                    thirdorder.Signature = model.signature;
                    thirdorder.UpdateTime = model.update_time;
                    thirdorder.DmId = model.dm_id;
                    thirdorder.DmName = model.dm_name;
                    thirdorder.DmMobile = model.dm_mobile;
                    thirdorder.ThirdType = ThirdType.达达配送;
                }
            }
            shopDb.ThirdApiData.Add(new ThirdApiData()
            {
                ShopId = order.ShopId,
                DaDaShopId = 73753,
                AddTime = DateTime.Now,
                JsonData = "CallBack",
                ResultData = JsonConvert.SerializeObject(model),
                OrderId = model.order_id
            });
            shopDb.SaveChanges();
            return Success();
        }


        [HttpPost]
        public APIResult TestThird([FromBody] ThirdPartyDistributionModel model)
        {
            // RechargeModel rechargeModel = new RechargeModel() { amount = 10, category = "pc", notify_url = "" };
            var parametermodel = new ThirdPartyDistributionParameterModel();
            switch (model.ApiUrl)
            {
                case "/merchantApi/merchant/add":
                    parametermodel.source_id = "";
                    var merchant = new MerchantModel()
                    {
                        mobile = "15812808736",
                        city_name = "东莞",
                        contact_name = "万卓",
                        contact_phone = "15812808736",
                        email = "1321913529@qq.com",
                        enterprise_address = "百安中心A栋1006",
                        enterprise_name = "惠吃惠喝"
                    };
                    model.Data = JsonConvert.SerializeObject(merchant);
                    break;
                case "/api/shop/add":
                    parametermodel.source_id = GetMerchant(model.ShopId).DaDaShopId.ToString();
                    parametermodel.body = model.Data;
                    break;
                case "/api/cityCode/list":
                    parametermodel.source_id = "73753";
                    parametermodel.body = model.Data;
                    break;

            }
            parametermodel.body = model.Data;
            var data = parameter.ExThirdPartyDistributionParameterAction(parametermodel);
            var result = httpClient.Post<object>($"{thirdConfig.Url + model.ApiUrl}", data);
            return Success(result);
        }

        public APIResult PreThirdAddOrder()
        {
            ThirdServer thirdServer = new ThirdServer(shopDb, thirdConfig);
            var model = shopDb.ShopOrders.FirstOrDefault(r => r.Id == 2651);
            var shopordertakeout = shopDb.ShopOrderTakeouts.FirstOrDefault(r => r.ShopOrderId == model.Id && !r.IsDel);
            var tradeno = DateTime.Now.Ticks + CommonUtil.CreateNoncestr(5);
            //处理达达配送
            var thirddshopaddmodel = new BLL.Servers.ThirdShopAddOrderModel()
            {
                ShopId = 11,
                origin_id = tradeno,//
                                    //shop_no = "11047059",//测试
                shop_no = "9896-126130",
                cargo_type = -1,
                cargo_price = 0.01,
                city_code = "0769",
                is_prepay = 0,
                origin_mark = "HCHH",
                origin_mark_no = tradeno,
                receiver_lng = 113.76031219944932,
                receiver_lat = 23.012808108215836,
                receiver_phone = "13266274421",
                receiver_address = "第一国际百安中心A1006",
                receiver_name = "t",
                callback = "callback",
            };
            var result = thirdServer.PreThirdAddOrder(thirddshopaddmodel);
            return Success(result);
        }
    }
}
