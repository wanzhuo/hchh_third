using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZRui.Web.ShopManager.ShopWechatOpenSetModels
{
    public class ShopIdArgsModel
    {
        public int? ShopId { get; set; }
    }

    public class GetQRCodeArgsModel: ShopIdArgsModel
    {
        public string Path { get; set; }
    }


}