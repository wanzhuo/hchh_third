using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZRui.Web
{
    /// <summary>
    /// 发送手机验证码类
    /// </summary>
    public class MemberSMSValiCodeTask : EntityBase
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
        public MemberSMSValiCodeTaskState TaskState { get; set; }

        /// <summary>
        /// 任务类型
        /// </summary>
        public string TaskType { get; set; }

        /// <summary>
        /// 发送任务时间
        /// </summary>
        public DateTime TaskTime { get; set; }

        /// <summary>
        /// 任务结束时间
        /// </summary>
        public DateTime TaskEndTime { get; set; }

    }

    public enum MemberSMSValiCodeTaskState
    {
        未使用 = 1, 已使用 = 2, 已取消 = 3
    }

    public static class MemberSMSValiCodeTaskDbContextExtention
    {
        public static void AddToMemberSMSValiCodeTask(this DbContext db, MemberSMSValiCodeTask model)
        {
            if (string.IsNullOrEmpty(model.Phone)) throw new ArgumentNullException("Phone");
            if (string.IsNullOrEmpty(model.Code)) throw new ArgumentNullException("Code");

            //将之前提交的未使用的短信设置为取消
            var smslist = db.Query<MemberSMSValiCodeTask>()
                          .Where(m => m.Phone == model.Phone)
                          .Where(m => m.TaskState == MemberSMSValiCodeTaskState.未使用)
                          .Where(m => m.TaskType == model.TaskType).ToList();

            foreach (var item in smslist)
            {
                item.TaskState = MemberSMSValiCodeTaskState.已取消;
            }

            db.Add<MemberSMSValiCodeTask>(model);
        }

        public static void SetMemberSMSValiCodeTaskFinished(this DbContext db, string phone, string code, string taskType)
        {
            var model = db.Query<MemberSMSValiCodeTask>()
                                        .Where(m => m.Phone == phone)
                                        .Where(m => m.TaskState == MemberSMSValiCodeTaskState.未使用)
                                        .Where(m => m.TaskType == taskType).FirstOrDefault();

            if (model == null)
                throw new Exception("请获取短信验证码");

            if (model.Code != code)
                throw new Exception("短信验证码不正确");

            if (DateTime.Now > model.TaskEndTime)
            {
                model.TaskState = MemberSMSValiCodeTaskState.已取消;
                db.SaveChanges();
                throw new Exception("验证码已过期，请重新获取");
            }

            model.TaskState = MemberSMSValiCodeTaskState.已使用;
        }

        /// <summary>
        /// 验证手机短信任务
        /// </summary>
        /// <param name="db"></param>
        /// <param name="phone">手机</param>
        /// <param name="operationIP">操作IP</param>
        /// <param name="taskType">任务类型</param>
        public static void CheckMemberSMSValiCodeTaskLimitRule(this DbContext db, string phone, string operationIP, string taskType)
        {
            var currentIP = operationIP;
            var currentTime = DateTime.Now;

            var currentStartTime = new DateTime(currentTime.Year, currentTime.Month, 1, 0, 0, 0);
            var currentEndTime = currentStartTime.AddDays(1).AddSeconds(-1);

            var query = db.Query<MemberSMSValiCodeTask>();

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
                       .Where(m => m.TaskState == MemberSMSValiCodeTaskState.未使用)
                       .Where(m => m.TaskType == taskType).ToList();

            foreach (var item in list)
            {
                item.TaskState = MemberSMSValiCodeTaskState.已取消;
            }
        }
    }
}
