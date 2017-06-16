using OrderStockManager.Providers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OrderStockManager.Controllers
{
    [RoutePrefix("api/osmsup")]
    public class SupportsController : BaseApiController
    {
        [HttpGet]
        [Route("rsa")]
        public IHttpActionResult RsaGet(string id)
        {
            string checker = ConfigurationManager.AppSettings["mg:Token"];
            if (id != checker)
                return StatusCode(HttpStatusCode.NotImplemented);

            try
            {
                IRSAKeyProvider _prov = new RSAKeyProvider();
                return Ok(_prov.GetPublicKey());
            }
            catch
            {
                return StatusCode(HttpStatusCode.NoContent);
            }
        }

        [HttpPost]
        [Route("rsa")]
        public IHttpActionResult Rsa(string id)
        {
            string checker = ConfigurationManager.AppSettings["mg:Token"];
            if (id != checker)
                return StatusCode(HttpStatusCode.NotImplemented);

            try
            {
                IRSAKeyProvider _prov = new RSAKeyProvider();
                _prov.DeleteKeys();
                _prov.CreateKey();
                return Ok();
            }
            catch
            {
                return InternalServerError();
            }
        }

        [HttpPost]
        [Route("hmac")]
        public IHttpActionResult Hmac(string id)
        {
            string checker = ConfigurationManager.AppSettings["mg:Token"];
            if (id != checker)
                return StatusCode(HttpStatusCode.NotImplemented);

            try
            {
                AddUpdateAppSettings("as:AudienceId", Guid.NewGuid().ToString("N"));
                AddUpdateAppSettings("as:AudienceId", Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(System.Web.Security.Membership.GeneratePassword(40, 0))).TrimEnd('='));
                var key = new byte[64];
                System.Security.Cryptography.RNGCryptoServiceProvider.Create().GetBytes(key);
                AddUpdateAppSettings("as:AudienceSecret", Microsoft.Owin.Security.DataHandler.Encoder.TextEncodings.Base64Url.Encode(key));
                return Ok();
            }
            catch
            {
                return InternalServerError();
            }
        }

        private void AddUpdateAppSettings(string key, string value)
        {
            // System.Web.HttpContext.Current.Request.ApplicationPath
            System.Configuration.Configuration config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");

            if (config.AppSettings.Settings[key] == null)
            {
                config.AppSettings.Settings.Add(key, value);
            }
            else
            {
                config.AppSettings.Settings[key].Value = value;
                // config.AppSettings.Settings.Remove(key);
                // config.AppSettings.Settings.Add(key, value);
            }
            config.Save();
        }

    }
}
