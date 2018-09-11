using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Senparc.Weixin.Open.Containers;
using StackExchange.Redis;

namespace ThirdParty.Controllers
{  
    [Route("api/[controller]")]
    public class WeChatComponentController : Controller
    {
        [HttpGet]
        public string TryGetComponentAccessToken()
        {

            return  ComponentContainer.TryGetComponentAccessToken("wx99dc6b0ea873ba0c", "d4f8f1222a94562ac6df4349861086b6");
            
        }
        [HttpGet]
        public string Test()
        {
            return "ok";
        }


    }
}