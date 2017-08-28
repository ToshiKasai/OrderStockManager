using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using OrderStockManager.Infrastructure;
using OrderStockManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace OrderStockManager.Controllers
{
    public class BaseApiController : ApiController
    {
        private ModelFactory _modelFactory;
        private ApplicationUserManager _AppUserManager = null;
        private ApplicationRoleManager _AppRoleManager = null;

        const string OCTET = "application/octet-stream";

        protected ApplicationUserManager AppUserManager
        {
            get
            {
                return _AppUserManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        protected ApplicationRoleManager AppRoleManager
        {
            get
            {
                return _AppRoleManager ?? Request.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            }
        }

        protected IAuthenticationManager AuthenticationManager
        {
            get
            {
                return Request.GetOwinContext().Authentication;
            }
        }

        protected int GetUserId()
        {
            return AuthenticationManager.User.Identity.GetUserId<int>();
        }

        protected string GetUserName()
        {
            return AuthenticationManager.User.Identity.GetUserName();
        }

        protected string GetClientIp()
        {
            try
            {
                IEnumerable<string> headerValues;
                string result = string.Empty;
                if (ControllerContext.Request.Headers.TryGetValues("X-Forwarded-For", out headerValues) == true)
                {
                    var xForwardedFor = headerValues.FirstOrDefault();
                    result = xForwardedFor.Split(',').GetValue(0).ToString().Trim();
                }
                else
                {
                    if (ControllerContext.Request.Properties.ContainsKey("MS_HttpContext"))
                    {
                        result = ((HttpContextWrapper)ControllerContext.Request.Properties["MS_HttpContext"]).Request.UserHostAddress;
                    }
                }

                if (result != "::1"/*localhost*/)
                {
                    result = result.Split(':').GetValue(0).ToString().Trim();
                }
                else
                {
                    result = "localhost";
                }
                return result;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public BaseApiController()
        {
        }

        protected override void Dispose(bool disposing)
        {
            _modelFactory = null;
            _AppUserManager = null;
            _AppRoleManager = null;

            // GC.Collect();
            base.Dispose(disposing);
        }

        protected ModelFactory TheModelFactory
        {
            get
            {
                if (_modelFactory == null)
                {
                    _modelFactory = new ModelFactory(this.Request, this.AppUserManager);
                }
                return _modelFactory;
            }
        }

        protected IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }
                return BadRequest(ModelState);
            }
            return null;
        }

        protected void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                foreach (var err in error.Split('。'))
                {
                    if (!string.IsNullOrWhiteSpace(err))
                        ModelState.AddModelError("", err.Trim(' ') + "。");
                }
            }
        }

        protected HttpResponseMessage ByteResponse(byte[] content, HttpStatusCode code = HttpStatusCode.OK)
        {
            var result = new HttpResponseMessage(code);
            // result.Content = new ByteArrayContent(content);
            result.Content = new StreamContent(new System.IO.MemoryStream(content));
            result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(OCTET);
            return result;
        }
    }
}
