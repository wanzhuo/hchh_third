using Hangfire;
using Hangfire.Annotations;
using System;
using System.Linq.Expressions;

namespace ZRui.Web.BLL.Utils
{
    public class BackgroundUtil
    {
        public static void Enqueue([InstantHandle][NotNull] Expression<Action> methodCall)
        {
            BackgroundJob.Enqueue(methodCall);
        }
    }
}
