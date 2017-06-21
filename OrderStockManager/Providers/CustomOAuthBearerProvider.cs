using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNet.Identity.Owin;
using OrderStockManager.Infrastructure;
using OrderStockManager.Models;
using OrderStockManager.Services;
using Microsoft.AspNet.Identity;
using System.IdentityModel.Tokens;

namespace OrderStockManager.Providers
{
    public class CustomOAuthBearerProvider : OAuthBearerAuthenticationProvider
    {
        public override Task RequestToken(OAuthRequestTokenContext context)
        {
            if (!string.IsNullOrEmpty(context.Token) && context.Token != "null")
            {
                var handler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwt = (JwtSecurityToken)handler.ReadToken(context.Token);

                if (!jwt.Header.ContainsKey(JwtHeaderParameterNames.Typ))
                {
                    context.Token = string.Empty;
                }
                string typ = (string)jwt.Header[JwtHeaderParameterNames.Typ];
                if (typ != "JWT" && typ != "urn:ietf:params:oauth:token-type:jwt")
                {
                    context.Token = string.Empty;
                }
                string alg = (string)jwt.Header[JwtHeaderParameterNames.Alg];

                bool hmacMode = false;
                if (hmacMode && alg != Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha512Signature && alg != Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha512)
                {
                    context.Token = string.Empty;
                }
                if (!hmacMode && alg != Microsoft.IdentityModel.Tokens.SecurityAlgorithms.RsaSha256Signature && alg!= Microsoft.IdentityModel.Tokens.SecurityAlgorithms.RsaSha256)
                {
                    context.Token = string.Empty;
                }
            }
            return base.RequestToken(context);
        }

        public override Task ValidateIdentity(OAuthValidateIdentityContext context)
        {
            ClaimsIdentity claimIdentity = context.Ticket.Identity;
            var claims = claimIdentity.Claims;

            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();
            var user = userManager.FindByName(claimIdentity.Name);
            var sts = userManager.GetSecurityStamp(user.Id);

            var owin = HttpContext.Current.GetOwinContext();
            var stsid = claims.Where(c => c.Type == "sts:id").Select(c => c.Value).SingleOrDefault();
            var stsdata = claims.Where(c => c.Type == "sts:ds").Select(c => c.Value).SingleOrDefault();

            if (string.IsNullOrEmpty(sts) || string.IsNullOrEmpty(stsid) || string.IsNullOrEmpty(stsdata))
            {
                context.Rejected();
            }
            else if (stsdata != CryptographService.CreateMacCode(sts, stsid))
            {
                context.Rejected();
            }

            return base.ValidateIdentity(context);
        }
    }
}
