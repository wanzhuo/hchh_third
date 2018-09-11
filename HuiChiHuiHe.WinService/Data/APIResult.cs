using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuiChiHuiHe.WinService
{
    public class ApiResult
    {
        /// <summary>
        /// 返回状态 
        /// </summary>
        public APIStatusCode StatusCode { get; set; }
        /// <summary>
        /// 调用结果提示信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 调用返回数据
        /// </summary>
        public object Data { get; set; }
    }

    public enum APIStatusCode
    {
        OK = 1,
        /// <summary>
        /// 失败
        /// </summary>
        Failure = 2,
    }
}
