using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.ServiceProcess;

namespace HuiChiHuiHe.WinService
{
    /// <summary>
    /// 服务基类
    /// </summary>
    public partial class BaseService : ServiceBase
    {
     
        /// <summary>
        /// 内部 消息传送
        /// </summary>
        protected string Msg { get; set; }
        protected TaskConfig ServiceConfigInfo { get; set; }


        public BaseService(TaskConfig m_serviceConfigInfo)
        {
            InitializeComponent();

            ServiceConfigInfo = m_serviceConfigInfo;         
            BaseServiceTimer.Interval = 1000 * 30;
            ServiceName = this.GetType().Name;
            int ServiceInternal = 30;//默认30秒
            if (ServiceConfigInfo != null)
            {
                ServiceInternal = ServiceConfigInfo.ServiceInternal;
            }
            BaseServiceTimer.Interval = 1000 * ServiceInternal;
        }

        protected override void OnStart(string[] args)
        {
            // TODO: 在此处添加代码以启动服务。
      
        }

        protected override void OnStop()
        {
            // TODO: 在此处添加代码以执行停止服务所需的关闭操作。
        }

        private void BaseServiceTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            BaseServiceTimer.Stop();
            BaseServiceTimer.Enabled = false;
            try
            {
                if (CheckExecute())
                {
                    ExecuteProcess();
                }
                else
                {
                    //  AddLog(ServiceLogType.服务警告, Msg);
                }
            }
            catch (Exception ex)
            {
                // AddLog(ServiceLogType.服务错误, Msg + "\r\n" + ex.ToString());
              
            }
            finally
            {
                BaseServiceTimer.Start();
                BaseServiceTimer.Enabled = true;
            }
        }

        #region 服务主体内容执行-需要子类重写
        /// <summary>
        /// 服务主体内容执行
        /// </summary>
        /// <returns></returns>
        protected virtual void ExecuteProcess()
        {
            throw new Exception("请实现父类！");
        }
        #endregion

        #region 执行前的检测
        /// <summary>
        /// 执行前的检测
        /// </summary>
        /// <returns></returns>
        protected bool CheckExecute()
        {
            if (ServiceConfigInfo != null && !string.IsNullOrEmpty(ServiceConfigInfo.TimeBucket))
            {
                string[] timehour = ServiceConfigInfo.TimeBucket.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (!IsOpenHour(timehour))
                {
                    Msg = "服务不在执行时间段";
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region 内部辅助方法

        #region 当前的时刻是否为开放时间
        /// <summary>
        /// 当前的时刻是否为开放时间
        /// </summary>
        /// <returns></returns>
        private bool IsOpenHour(string[] timehour)
        {
            bool ishour = false;
            for (int i = 0; i < timehour.Length; i = i + 2)
            {
                if (DateTime.Now >= DateTime.Parse(DateTime.Now.Date.ToString().Replace("0:00:00", timehour[i])) &&
                                    DateTime.Now <= DateTime.Parse(DateTime.Now.Date.ToString().Replace("0:00:00", timehour[i + 1])))
                {
                    ishour = true;
                    break;
                }
            }
            return ishour;
        }
        #endregion

        #region 添加日志
        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <param name="logEnum">日志类型</param>
        /// <param name="logContent">日志内容</param>
        //protected void AddLog(ServiceLogType logEnum, string logContent = "", string dataId = "",string logRemark="")
        //{
        //    try
        //    {
        //        ServiceLog log = new ServiceLog()
        //        {
        //            Id=Guid.NewGuid().ToString(),
        //            CreationDate=DateTime.Now,
        //            DataId=dataId,
        //            LogContent=logContent,
        //            LogType=(int)logEnum,
        //            LogRemark= logRemark,
        //            ServiceName= ServiceName,
        //            LogTypeName= logEnum.ToString()
        //        };
        //        //_IServiceLogBLL.Add(log);
        //        //_IServiceLogBLL.SaveChanges();

        //    }
        //    catch (Exception ex)
        //    {
        //        //Logs.WriteOperateErrorLog(ServiceName, "插入日志报错", ex.Message);
        //      //  Log.Error(ex);
        //    }
        //}
        /// <summary>
        /// 添加警告日志
        /// </summary>
        /// <param name="logContent"></param>
        /// <param name="dataId"></param>
        protected void AddWarnLog(string logContent, string dataId = "")
        {
            // AddLog(ServiceLogType.服务警告, logContent, dataId);
        }
        /// <summary>
        /// 添加备注日志
        /// </summary>
        /// <param name="logContent"></param>
        /// <param name="dataId"></param>
        protected void AddRemarkLog(string logContent, string dataId = "")
        {
            //  AddLog(ServiceLogType.服务备注, logContent, dataId);
        }
        /// <summary>
        /// 添加异常日志（一般为程序报错）
        /// </summary>
        /// <param name="logContent"></param>
        /// <param name="dataId"></param>
        protected void AddExceptionLog(string logContent, string dataId = "")
        {
            // AddLog(ServiceLogType.服务错误, logContent, dataId);
        }
        #endregion

        #region 发送邮件
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="title">邮件标题</param>
        /// <param name="content">邮件内容</param>
        /// <param name="address">收信人地址</param>
        protected void SendEmail(string title, string content, string address)
        {
            //if (!string.IsNullOrEmpty(address))
            //{
            //    //插入发送邮箱队列
            //    var serviceQueue = WinLink.IBLL.Container.Resolve<IServiceQueueBLL>().CreateEmailQueue("", "Windows服务", new EmailModel() { Title = title, Address = address, Content = content });
            //    DapperHelper.GetDbWinLink.Insert<ServiceQueue>(serviceQueue);
            //}
        }
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="content">邮件内容</param>
        protected void SendEmail(string content)
        {

            //  SendEmail(string.Format("Windows服务异常,{0}",ServiceName), content, _ExceptionEmailAddress);
        }
        #endregion
        #endregion
    }

    public class AutoTask : BaseService
    {
        public AutoTask(TaskConfig taskConfig):base(taskConfig)
        {

        }

        protected override void ExecuteProcess()
        {
          ApiResult result =  WebApiHelper.Invoke(HttpMethod.Get, ServiceConfigInfo.Api, null, null);
        }
    }
}
