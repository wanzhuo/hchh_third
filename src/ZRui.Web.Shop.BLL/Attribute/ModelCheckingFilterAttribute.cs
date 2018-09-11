using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace ZRui.Web.BLL.Attribute
{
    public class ModelCheckingFilterAttribute: ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var stateEntry = context.ModelState.Values.FirstOrDefault();
                if (stateEntry != null)
                {
                    var modelError = stateEntry.Errors.FirstOrDefault();
                    string errorMessage = modelError?.ErrorMessage;
                    if (string.IsNullOrEmpty(errorMessage))
                        errorMessage = modelError.Exception.Message;
                    throw new Exception(errorMessage);
                }
            }
            base.OnActionExecuting(context);
        }
      
    }
}
