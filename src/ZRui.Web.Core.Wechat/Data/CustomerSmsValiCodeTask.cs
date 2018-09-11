using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZRui.Web.Core.Wechat
{
    /// <summary>
    /// 发送手机验证码类
    /// </summary>
    public class CustomerSmsValiCodeTask
    {
        public int Id { get; set; }

        /// <summary>
        /// 操作IP
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// 接收短信手机
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 任务状态
        /// </summary>
        public CustomerSmsValiCodeTaskState TaskState { get; set; }

        /// <summary>
        /// 任务类型
        /// </summary>
        public CustomerSmsValiCodeTaskType TaskType { get; set; }

        /// <summary>
        /// 发送任务时间
        /// </summary>
        public DateTime TaskTime { get; set; }

        /// <summary>
        /// 任务结束时间
        /// </summary>
        public DateTime TaskEndTime { get; set; }

    }

    public enum CustomerSmsValiCodeTaskState
    {
        未使用 = 1, 已使用 = 2, 已取消 = 3
    }

    public enum CustomerSmsValiCodeTaskType
    {
        手机绑定 = 1
    }


    /// <summary>
    /// 会员短信任务查询条件类
    /// </summary>
    public class CustomerSmsValiCodeTaskQueryCondition
    {

        /// <summary>
        /// 操作IP
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// 接收短信手机
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 任务状态
        /// </summary>
        public CustomerSmsValiCodeTaskState? TaskState { get; set; }

        /// <summary>
        /// 任务类型
        /// </summary>
        public CustomerSmsValiCodeTaskType? TaskType { get; set; }


    }

    /// <summary>
    /// 会员短信任务扩展类
    /// </summary>
    public static class CustomerSmsValiCodeTaskDbContextExtention
    {
        /// <summary>
        /// 通过Id获得一个会员短信任务对象
        /// </summary>
        public static CustomerSmsValiCodeTask GetSingleCustomerSmsValiCodeTask(this DbContext context, int id)
        {
            return context.Set<CustomerSmsValiCodeTask>().Where(m => m.Id == id).FirstOrDefault();
        }

        public static CustomerSmsValiCodeTask GetLastestCustomerSmsValiCodeTask(this DbContext context, string phone)
        {
            var memberSmsValiCodeTask = context.QueryCustomerSmsValiCodeTask(null)
                                   .Where(m => m.Phone == phone)
                                   .Where(m => m.TaskState == CustomerSmsValiCodeTaskState.未使用)
                                   .Where(m => m.TaskType == CustomerSmsValiCodeTaskType.手机绑定).FirstOrDefault();

            return memberSmsValiCodeTask;
        }

        /// <summary>
        /// 添加一个会员短信任务对象到数据库中
        /// </summary>
        public static CustomerSmsValiCodeTask AddToCustomerSmsValiCodeTask(this DbContext context, CustomerSmsValiCodeTask model)
        {
            context.Set<CustomerSmsValiCodeTask>().Add(model);
            return model;
        }

        /// <summary>
        /// 删除指定Id的会员短信任务记录
        /// </summary>
        public static CustomerSmsValiCodeTask DeleteCustomerSmsValiCodeTask(this DbContext context, int id)
        {
            var model = context.Set<CustomerSmsValiCodeTask>().Where(m => m.Id == id).FirstOrDefault();
            if (model != null)
            {
                context.Set<CustomerSmsValiCodeTask>().Remove(model);
            }
            return model;
        }

        /// <summary>
        /// 通过查询条件找出指定的会员短信任务记录
        /// </summary>
        public static IQueryable<CustomerSmsValiCodeTask> QueryCustomerSmsValiCodeTask(this DbContext context, CustomerSmsValiCodeTaskQueryCondition condition)
        {
            var query = context.Set<CustomerSmsValiCodeTask>().AsQueryable();
            if (condition != null)
            {
                if (!string.IsNullOrEmpty(condition.Phone))
                {
                    query = query.Where(m => m.Phone == condition.Phone);
                }

                if (!string.IsNullOrEmpty(condition.IP))
                {
                    query = query.Where(m => m.IP == condition.IP);
                }

                if (!string.IsNullOrEmpty(condition.Code))
                {
                    query = query.Where(m => m.Code == condition.Code);
                }

                if (condition.TaskState.HasValue)
                {
                    query = query.Where(m => m.TaskState == condition.TaskState);
                }

                if (condition.TaskType.HasValue)
                {
                    query = query.Where(m => m.TaskType == condition.TaskType);
                }
            }
            return query;
        }

        public static void CheckCustomerSmsValiCodeTask(this DbContext db, string phone, string operationIP, CustomerSmsValiCodeTaskType taskType)
        {
            var currentIP = operationIP;
            var currentTime = DateTime.Now;

            var currentStartTime = new DateTime(currentTime.Year, currentTime.Month, 1, 0, 0, 0);
            var currentEndTime = currentStartTime.AddDays(1).AddSeconds(-1);

            var query = db.QueryCustomerSmsValiCodeTask(null);

            var ismMore20 = query
                          .Where(m => m.TaskTime >= currentStartTime && m.TaskTime <= currentEndTime)
                          .Where(m => m.IP == currentIP)
                          .Where(m => m.TaskType == taskType).Count() > 20;

            //同一IP，同一操作类型一天不能超过20条信息
            if (ismMore20)
                throw new Exception("同一类型操作不能大于20次");

            //同一手机,同一类型30分钟内不能大于5次
            var queryForPhoneAndIP = query
                     .Where(m => m.TaskType == taskType)
                     .Where(m => m.Phone == phone).OrderByDescending(m => m.Id).Take(5).ToList();
            if (queryForPhoneAndIP.Count() == 5)
            {
                var taskModelForLast = queryForPhoneAndIP.LastOrDefault();
                var minutesFor30 = currentTime.Subtract(taskModelForLast.TaskTime).TotalMinutes;
                if (minutesFor30 <= 30)
                    throw new Exception("请在30分钟后再获取验证码");
            }

            // 同一手机1分钟只能获取一个验证码
            var taskModel = queryForPhoneAndIP.FirstOrDefault();
            if (taskModel != null)
            {
                var minutesFor1 = currentTime.Subtract(taskModel.TaskTime).TotalMinutes;
                if (minutesFor1 <= 1)
                    throw new Exception("请在1分钟后再获取验证码");

            }

            //把同一手机、同一类型的任务状态改为已取消
            var list = query
                       .Where(m => m.Phone == phone)
                       .Where(m => m.TaskState == CustomerSmsValiCodeTaskState.未使用)
                       .Where(m => m.TaskType == taskType).ToList();

            foreach (var item in list)
            {
                item.TaskState = CustomerSmsValiCodeTaskState.已取消;
            }
        }
    }
}
