using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HuiChiHuiHe.Auth.Controllers
{
    [Produces("application/json")]
    [Route("api/Delivery/[action]")]
    public class DeliveryController : Controller
    {
        const string App_key = "dadaa3ced79e406f366";
        const string App_secret = "fb3597933e24b40a78823f6f424ee9c7";
        const string Domain = "http://newopen.qa.imdada.cn";

        [HttpPost]
        public ActionResult Order(DeliveryOrder order)
        {
            string url = "/api/order/addOrder";
            return null;
        }
    }
}