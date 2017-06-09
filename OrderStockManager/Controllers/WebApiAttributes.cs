using OrderStockManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace OrderStockManager.Controllers
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class ValidationRequiredAttribute : ActionFilterAttribute
    {
        public bool IsNullable { get; set; }
        public string prefix { get; set; }

        public ValidationRequiredAttribute()
        {
            IsNullable = false;
            prefix = "";
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!IsNullable && actionContext.ActionArguments.Any(x => x.Value == null))
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "No Parameters");
            }
            else if (!actionContext.ModelState.IsValid)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest, actionContext.ModelState.GetErrorsDelprefix(prefix));
            }

            base.OnActionExecuting(actionContext);
        }
    }
}