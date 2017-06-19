using OrderStockManager.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace OrderStockManager.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class EnableTagAttribute : ActionFilterAttribute
    {
        private static ConcurrentDictionary<string, EntityTagHeaderValue> etags = new ConcurrentDictionary<string, EntityTagHeaderValue>();

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext != null)
            {
                HttpRequestMessage request = actionContext.Request;
                if (request.Method == HttpMethod.Get)
                {
                    if (IfNoneMatchContainsStoredEtagValue(request))
                    {
                        actionContext.Response = new HttpResponseMessage(HttpStatusCode.NotModified);
                        SetCacheControl(actionContext.Response);
                    }
                }
                if (request.Method == HttpMethod.Put)
                {
                    if (!IfMatchContainsStoredEtagValue(request))
                    {
                        actionContext.Response = new HttpResponseMessage(HttpStatusCode.PreconditionFailed);
                        SetCacheControl(actionContext.Response);
                    }
                }
            }
            // base.OnActionExecuting(actionContext);
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            HttpRequestMessage request = actionExecutedContext.Request;
            string key = GetKey(request);

            EntityTagHeaderValue etag;
            if (actionExecutedContext.Response != null)
            {
                if (actionExecutedContext.Response.StatusCode == HttpStatusCode.OK || actionExecutedContext.Response.StatusCode == HttpStatusCode.NoContent)
                {
                    if (!etags.TryGetValue(key, out etag) || request.Method == HttpMethod.Put || request.Method == HttpMethod.Post)
                    {
                        etag = new EntityTagHeaderValue("\"" + ShortGuid.NewGuid().ToString() + "\"");
                        etags.AddOrUpdate(key, etag, (k, val) => etag);
                    }
                    actionExecutedContext.Response.Headers.ETag = etag;
                }
                SetCacheControl(actionExecutedContext.Response);
            }
            // base.OnActionExecuted(actionExecutedContext);
        }

        #region 補助機能
        private static void SetCacheControl(HttpResponseMessage response)
        {
            response.Headers.CacheControl = new CacheControlHeaderValue()
            {
                MaxAge = TimeSpan.FromSeconds(60),
                MustRevalidate = true,
                Private = true
            };
        }

        private static string GetKey(HttpRequestMessage request)
        {
            return request.RequestUri.ToString();
        }

        private bool IfNoneMatchContainsStoredEtagValue(HttpRequestMessage request)
        {
            if (request.Headers.IfNoneMatch.Count == 0)
                return false;

            EntityTagHeaderValue etag;
            if (etags.TryGetValue(GetKey(request), out etag))
                return request.Headers.IfNoneMatch.Select(v => v.Tag).Contains(etag.Tag);

            return false;
        }

        private bool IfMatchContainsStoredEtagValue(HttpRequestMessage request)
        {
            if (request.Headers.IfMatch.Count == 0)
                return false;

            EntityTagHeaderValue etag;
            if (etags.TryGetValue(GetKey(request), out etag))
                return request.Headers.IfMatch.Select(v => v.Tag).Contains(etag.Tag);

            return false;
        }
        #endregion
    }
}
