using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZRui.Web.BLL.ServerDto;

namespace ZRui.Web.BLL.Servers
{

    /// <summary>
    /// 拼团活动处理
    /// </summary>
    public class ShopConglomerationActivityOptions
    {

        /// <summary>
        /// 参团请求队列 （Key :参团Id Value：参团信息）
        /// </summary>
        //public static Dictionary<int, Queue<ConglomerationOrder>> RequestQueue = null;
        private static readonly object Padlock = new object();

        /// <summary>
        /// 参团请求队列 （Key :参团Id Value：用户Id）
        /// </summary>
        public static Dictionary<int, List<RequestModel>> SetupIdAndMemberId = null;
        private ShopConglomerationActivityOptions()
        {
        }

        /// <summary>
        /// 获取参团中人员
        /// </summary>
        /// <returns></returns>
        public static List<RequestModel> GetSetupIdAndMemberId(int conglomerationSetUpId, ILogger _logger)
        {
            _logger.LogInformation($"==========获取参团人员请求集合方法GetSetupIdAndMemberId开始============");

            lock (Padlock)
            {
                if (SetupIdAndMemberId == null)
                {

                    SetupIdAndMemberId = new Dictionary<int, List<RequestModel>>();
                }

                var key = SetupIdAndMemberId.Keys.FirstOrDefault(m => m.Equals(conglomerationSetUpId));
                if (key == 0)
                {
                    key = conglomerationSetUpId;
                    var add = new List<RequestModel>();
                    SetupIdAndMemberId.Add(key, add);
                    _logger.LogInformation($"key:{key}创建请求集合");
                    _logger.LogInformation($"==========获取参团人员请求集合方法GetSetupIdAndMemberId结束============");
                    return SetupIdAndMemberId[key];

                }
                else
                {
                    _logger.LogInformation($"key:{key}开始移除请求集合 当前集合数量SetupIdAndMemberId[key]:{SetupIdAndMemberId[key].Count}");
                    _logger.LogInformation($"==========获取参团人员请求集合方法GetSetupIdAndMemberId结束============");
                    SetupIdAndMemberId[key].RemoveAll(m => (DateTime.Now - m.CreateTime).TotalSeconds > 30); //移除过期
                    return SetupIdAndMemberId[key];

                }

            }

        }

        /// <summary>
        /// 添加到集合
        /// </summary>
        /// <param name="requestModel"></param>
        public static void AddList(List<RequestModel> list, RequestModel requestModel)
        {
            lock (Padlock)
            {
                list.Add(requestModel);
            }
        }


        /// <summary>
        /// 移除拼团请求
        /// </summary>
        /// <param name="list"></param>
        /// <param name="requestModel"></param>
        public static void RemoveSetup(int conglomerationSetUpId)
        {
            if (SetupIdAndMemberId != null)
            {

                SetupIdAndMemberId.Remove(conglomerationSetUpId);
            }
        }

        /// <summary>
        /// 请求记录实体
        /// </summary>
        public class RequestModel
        {
            public int MmeberId
            {
                get; set;
            }
            public DateTime CreateTime { get; set; }
        }



        /// <summary>
        /// 回调成功更新集合
        /// </summary>
        /// <param name="rechange"></param>
        public static void NotifyOkRemoveList(MemberTradeForRechange rechange, ShopDbContext shopdb, ILogger _logger)
        {
          
            if (rechange.ConglomerationOrderId.HasValue && rechange.ConglomerationOrderId.Value != 0)
            {
                _logger.LogInformation($"==========回调成功更新集合方法NotifyOkRemoveList开始============");
                _logger.LogInformation($"rechange.ConglomerationOrderId.HasValue：{rechange.ConglomerationOrderId.HasValue}=rechange.ConglomerationOrderId.Value:{rechange.ConglomerationOrderId.Value}");
                var orderId = rechange.ConglomerationOrderId.Value;
                var order = shopdb.ConglomerationOrder.Find(orderId);
                _logger.LogInformation($"orderID：{order.Id}");

                var ConglomerationSetUp = shopdb.ConglomerationSetUp.Find(order.ConglomerationSetUpId);
                _logger.LogInformation($"ConglomerationSetUpId：{ConglomerationSetUp.Id}");

                if (ConglomerationSetUp.Status == ConglomerationSetUpStatus.已经成团)
                {
                    SetupIdAndMemberId.Remove(ConglomerationSetUp.Id);
                    _logger.LogInformation($"移除拼团拼团ID:{ConglomerationSetUp.Id}");

                }
                else
                {
                    var list = GetSetupIdAndMemberId(ConglomerationSetUp.Id, _logger);
                    _logger.LogInformation($"listCount：{list.Count}");
                    list.RemoveAll(m => m.MmeberId.Equals(order.MemberId));
                    _logger.LogInformation($"移除拼团拼团集合项order.MemberId:{order.MemberId}");

                }
                _logger.LogInformation($"==========回调成功更新集合方法NotifyOkRemoveList结束============");
            }
         

        }

        /// <summary>
        /// 用户取消支付，主动更新集合
        /// </summary>
        /// <param name="rechange"></param>
        public static void NotifyOkRemoveList(int ConglomerationOrderId, ShopDbContext shopdb, ILogger _logger)
        {
           
            if (ConglomerationOrderId != 0)
            {
                _logger.LogInformation($"==========用户取消支付，主动更新集合方法NotifyOkRemoveList开始============");
                _logger.LogInformation($"ConglomerationOrderId：{ConglomerationOrderId}");
                var orderId = ConglomerationOrderId;
                var order = shopdb.ConglomerationOrder.Find(orderId);
                _logger.LogInformation($"orderID：{order.Id}");

                var ConglomerationSetUp = shopdb.ConglomerationSetUp.Find(order.ConglomerationSetUpId);
                _logger.LogInformation($"ConglomerationSetUpId：{ConglomerationSetUp.Id}");

                if (ConglomerationSetUp.Status == ConglomerationSetUpStatus.已经成团)
                {
                    SetupIdAndMemberId.Remove(ConglomerationSetUp.Id);
                    _logger.LogInformation($"移除拼团拼团ID:{ConglomerationSetUp.Id}");

                }
                else
                {
                    var list = GetSetupIdAndMemberId(ConglomerationSetUp.Id, _logger);
                    _logger.LogInformation($"移除前请求集合数量：{list.Count}");
                    list.RemoveAll(m => m.MmeberId.Equals(order.MemberId));
                    _logger.LogInformation($"order.MemberId:{order.MemberId}====移除后请求集合数量：{list.Count}");


                }
                _logger.LogInformation($"==========用户取消支付，主动更新集合方法NotifyOkRemoveList结束============");
            }
          

        }

        




    }
}
