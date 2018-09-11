using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using ZRui.Web.BLL;
using ZRui.Web.BLL.Log;
using ZRui.Web.Core.Finance.SwiftpassPay;

namespace ZRui.Web
{
    public class RefundLog<T> where T : class
    {
        HchhLogDbContext hchh;
        public RefundLog(HchhLogDbContext hchh)
        {
            this.hchh = hchh;
        }

        public void RefundAction(string name, PayOrRefundType payorrefundtype, int orderid, OrderType ordertype, T initiateparameter, SwiftpassPayResponseHandler notifyparameter, string errormsg = "")
        {


            hchh.PayOrRefundLog.Add(new BLL.Log.PayOrRefundLog()
            {
                Name = name,
                PayOrRefundType = payorrefundtype,
                OrderId = orderid,
                OrderType = ordertype,
                InitiateParameter = initiateparameter == null ? "" : JsonConvert.SerializeObject(initiateparameter),
                NotifyParameter = notifyparameter == null ? "" : JsonConvert.SerializeObject(notifyparameter.parameters),
                AddTime = DateTime.Now,
                ErrorMsg = errormsg
            });
            hchh.SaveChanges();
        }

    }
}
