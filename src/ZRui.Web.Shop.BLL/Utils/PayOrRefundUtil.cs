using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using ZRui.Web.BLL.Log;

namespace ZRui.Web.BLL.Utils
{
    public class PayOrRefundUtil<T> where T : class
    {
        HchhLogDbContext hchh;
        public PayOrRefundUtil(HchhLogDbContext hchh)
        {
            this.hchh = hchh;
        }
        public void PayAction(string name, PayOrRefundType payorrefundtype, int orderid, OrderType ordertype, T initiateparameter, T notifyparameter, string errormsg = "")
        {
            

            hchh.PayOrRefundLog.Add(new BLL.Log.PayOrRefundLog()
            {
                Name = name,
                PayOrRefundType = payorrefundtype,
                OrderId = orderid,
                OrderType = ordertype,
                InitiateParameter = initiateparameter == null ? "" : JsonConvert.SerializeObject(initiateparameter),
                
                NotifyParameter = notifyparameter == null ? "" : notifyparameter.ToString(),
                AddTime = DateTime.Now,
                ErrorMsg = errormsg
            });
            hchh.SaveChanges();
        }

    }
}
