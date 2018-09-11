using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinLink.WinService
{
    /// <summary>
    /// 测试 Windows服务
    /// </summary>
    public class TestService:BaseService
    {
        /// <summary>
        /// 如果没有指定配置，默认30秒后执行
        /// </summary>
        protected override void ExecuteProcess()
        {
            var a = 1 + 1;
            AddRemarkLog(string.Format("测试添加服务日志，运行结果为：{0}",DateTime.Now));
            var testEx = new Exception("测试");
            //Log.Error(testEx);
            Log.Info(testEx.ToString());
        }
    }
}
