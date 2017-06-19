using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using OrderStockManager.Infrastructure;
using OrderStockManager.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace OrderStockManager.Providers
{
    public class CustomOAuthProvider : OAuthAuthorizationServerProvider
    {
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId, clientSecret;
            if(context.TryGetFormCredentials(out clientId, out clientSecret))
            {
                // var typ = context.Parameters.Where(p => p.Key == "grant_type").Select(p => p.Value.FirstOrDefault()).FirstOrDefault();
                var secret = ConfigurationManager.AppSettings["as:AudienceSecret"].Split(',');
                if (secret.Contains(clientSecret))
                {
                    context.OwinContext.Set<string>("as:client_id", clientId);
                }
            }
            context.Validated();
            return Task.FromResult<object>(null);
        }

        public override Task ValidateTokenRequest(OAuthValidateTokenRequestContext context)
        {
            return base.ValidateTokenRequest(context);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var allowedOrigin = "*";
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();
            UserModel user = await userManager.FindAsync(context.UserName, context.Password);

            if (user == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }
#if false
            if (!user.EmailConfirmed)
            {
                context.SetError("invalid_grant", "User did not confirm email.");
                return;
            }
#endif
            await userManager.UpdateSecurityStampAsync(user.Id);
            ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager, "JWT");

            var props = new AuthenticationProperties(new Dictionary<string, string>
            {
                { "as:client_id", context.ClientId }
            });

            // 認証用のチケット発行
            var ticket = new AuthenticationTicket(oAuthIdentity, props);
            context.Validated(ticket);
        }

        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            var originalClient = context.Ticket.Properties.Dictionary["as:client_id"];
            var currentClient = context.OwinContext.Get<string>("as:client_id");

            if (originalClient != currentClient)
            {
                context.Rejected();
                return Task.FromResult<object>(null);
            }

            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();
            UserModel user = userManager.FindByName(context.Ticket.Identity.Name);
            ClaimsIdentity oAuthIdentity = user.GenerateUserIdentity(userManager, "JWT");

            var newTicket = new AuthenticationTicket(oAuthIdentity, context.Ticket.Properties);
            context.Validated(newTicket);

            return base.GrantRefreshToken(context);
        }
    }
}
