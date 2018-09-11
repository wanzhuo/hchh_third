using System;
using System.Collections;
using System.Text;

namespace ZRui.Web.Core.Finance.PayBase
{
    public class PayRequestBaseHandler
    {
        IPayOption options;

        public PayRequestBaseHandler(IPayOption options)
        {
            this.options = options;
            parameters = new Hashtable();
        }
        /// <summary>
        /// 请求的参数
        /// </summary>
        protected Hashtable parameters;

        /// <summary>
        /// debug信息
        /// </summary>
        private string debugInfo;

        /// <summary>
        /// 初始化函数
        /// </summary>
        public virtual void init()
        {
            //nothing to do
        }


        public void CreateSign(Func<Hashtable, string> signMethod)
        {
            string sign = signMethod(parameters);
            setParameter("sign", sign);
        }


        /// <summary>
        /// 获取参数值
        /// </summary>
        /// <param name="parameter">参数名</param>
        /// <returns></returns>
        public string getParameter(string parameter)
        {
            string s = (string)parameters[parameter];
            return (null == s) ? "" : s;
        }

        /// <summary>
        /// 设置参数值
        /// </summary>
        /// <param name="parameter">参数名</param>
        /// <param name="parameterValue">参数值</param>
        public void setParameter(string parameter, string parameterValue)
        {
            if (parameter != null && parameter != "")
            {
                if (parameters.Contains(parameter))
                {
                    parameters.Remove(parameter);
                }

                parameters.Add(parameter, parameterValue);
            }
        }

        /// <summary>
        /// 获取debug信息
        /// </summary>
        /// <returns></returns>
        public String getDebugInfo()
        {
            return debugInfo;
        }

        /// <summary>
        /// 设置debug信息
        /// </summary>
        /// <param name="debugInfo"></param>
        public void setDebugInfo(String debugInfo)
        {
            this.debugInfo = debugInfo;
        }

        /// <summary>
        /// 获取所有参数
        /// </summary>
        /// <returns></returns>
        public Hashtable getAllParameters()
        {
            return this.parameters;
        }
    }
}
