using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Threading.Tasks;

namespace ZRui.Web.Controllers
{
    public class ApiControllerBase : ControllerBase
    {
        protected APIResult Success()
        {
            return APIResult.CreateSuccess();
        }

        protected APIResult Success(string message)
        {
            return APIResult.CreateSuccess(message);
        }

        protected APIResult<T> Success<T>(T content, string message)
        {
            return APIResult.CreateSuccess<T>(content, message);
        }

        protected APIResult<T> Success<T>(T content)
        {
            return APIResult.CreateSuccess<T>(content);
        }

        protected APIResult Error(int errorCode, string message)
        {
            return APIResult.CreateError(errorCode, message);
        }

        protected APIResult Error(string message)
        {
            return APIResult.CreateError(0, message);
        }

        protected APIResult<T> Error<T>(int errorCode, string message)
        {
            return APIResult.CreateError<T>(errorCode, message);
        }

        protected APIResult<T> Error<T>(string message)
        {
            return APIResult.CreateError<T>(0, message);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (!context.ExceptionHandled && context.Exception != null)
            {
                var error = context.Exception.Message;
                var e = context.Exception;
                while (e.InnerException != null)
                {
                    e = e.InnerException;
                    error += "|" + e.Message;
                }
                if (e.Message == "未登录")
                {
                    context.HttpContext.Response.StatusCode = 401;
                }
                context.Result = Json(Error(error));
                context.ExceptionHandled = true;
            }
            base.OnActionExecuted(context);
        }
    }
}
