using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ZRui.Web.Core.Wechat
{
    public class RobotMessage
    {
        public long Id { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public bool IsDel { get; set; }
        public RobotMessageQuestionType QuestionType { get; set; }
        public RobotMessageStatus Status { get; set; }
    }


    public enum RobotMessageStatus
    {
        正常 = 0,
        停用 = -1
    }

    public enum RobotMessageQuestionType
    {
        文本 = 0,
        正则 = 1
    }


    public class RobotMessageQueryCondition
    {
    }

    public static class RobotMessageDbContextExtention
    {
        public static RobotMessage AddToRobotMessage(this DbContext context, RobotMessage model)
        {
            context.Set<RobotMessage>().Add(model);
            return model;
        }

        public static RobotMessage GetSingleRobotMessageForWelcome(this DbContext context)
        {
            return context.Set<RobotMessage>().Where(m => m.Question == "welcome").FirstOrDefault();
        }

        public static RobotMessage GetSingleRobotMessage(this DbContext context, int id)
        {
            return context.Set<RobotMessage>().Where(m => m.Id == id).FirstOrDefault();
        }

        public static EntityEntry<RobotMessage> DeleteRobotMessage(this DbContext context, int id)
        {
            var model = context.GetSingleRobotMessage(id);
            if (model != null)
            {
                return context.Set<RobotMessage>().Remove(model);
            }
            else
            {
                return null;
            }
        }

        public static IQueryable<RobotMessage> QueryRobotMessage(this DbContext context)
        {
            return context.Set<RobotMessage>().AsQueryable();
        }

        public static DbSet<RobotMessage> RobotMessageDbSet(this DbContext context)
        {
            return context.Set<RobotMessage>();
        }
    }
}