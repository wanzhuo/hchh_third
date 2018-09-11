using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.ShopPayInfoSetAPIModels
{



    public class RowItem : ShopPayInfo
    {

    }

    public class GetSingleArgsModel
    {
        public int id { get; set; }
    }

    public class GetPayWayArgsModel
    {
        public int id { get; set; }
        public PayWay payway { get; set; }

    }


    public class SetPayInfoArgsModel
    {
        public int? shopid { get; set; }
        public string mchid { get; set; }
        public PayWay payway { get; set; }
        public string secretkey { get; set; }

        public bool isenable { get; set; }
        /// <summary>
        /// 公钥
        /// </summary>
        public string publicKey { get; set; }
        /// <summary>
        /// 私钥
        /// </summary>
        public string prviateKey { get; set; }
        /// <summary>
        /// 请求地址
        /// </summary>
        public string reqUrl { get; set; }
        /// <summary>
        /// 回调地址
        /// </summary>
        public string notify { get; set; }

    }

    public class SetPayInfoSwiftpassArgsModel
    {

        public string mchid { get; set; }
        public bool isenable { get; set; }
        public string secretkey { get; set; }
        public string flage { get; set; }
        /// <summary>
        /// 公钥
        /// </summary>
        public string publicKey { get; set; }
        /// <summary>
        /// 私钥
        /// </summary>
        public string prviateKey { get; set; }
        /// <summary>
        /// 请求地址
        /// </summary>
        public string reqUrl { get; set; }
        /// <summary>
        /// 回调地址
        /// </summary>
        public string notify { get; set; }

    }
}
