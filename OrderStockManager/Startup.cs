using Microsoft.AspNet.WebApi.Extensions.Compression.Server;
using Microsoft.AspNet.WebApi.Extensions.Compression.Server.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.DataHandler.Serializer;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Practices.Unity;
using Newtonsoft.Json.Serialization;
using OrderStockManager.Infrastructure;
using OrderStockManager.Providers;
using OrderStockManager.Repositories;
using OrderStockManager.Services;
using Owin;
using System;
using System.Configuration;
using System.Linq;
using System.Net.Http.Extensions.Compression.Core.Compressors;
using System.Net.Http.Formatting;
using System.Web.Http;

[assembly: OwinStartup(typeof(OrderStockManager.Startup))]

namespace OrderStockManager
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration httpConfig = new HttpConfiguration();
            
            ConfigureOAuthTokenGeneration(app);
            ConfigureOAuthTokenConsumption(app);

            ConfigureWebApi(httpConfig);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(httpConfig);

            httpConfig.MessageHandlers.Insert(0, new ServerCompressionHandler(new GZipCompressor(), new DeflateCompressor()));
            // httpConfig.MessageHandlers.Insert(0, new OwinServerCompressionHandler(new GZipCompressor(), new DeflateCompressor()));
        }

        private void ConfigureOAuthTokenGeneration(IAppBuilder app)
        {
            app.CreatePerOwinContext(DataContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);

            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/oauth/token"), // JWT生成パス
                AccessTokenExpireTimeSpan = TimeSpan.FromHours(8),   // Token期限
                Provider = new CustomOAuthProvider(),
                AccessTokenFormat = new CustomJwtFormat(ConfigurationManager.AppSettings["as:Issuer"]),
                RefreshTokenProvider = new CustomRefreshTokenProvider()
            };

            app.UseOAuthAuthorizationServer(OAuthServerOptions);
        }

        private void ConfigureWebApi(HttpConfiguration config)
        {
            WebApiConfig.Register(config);

            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }

        private void ConfigureOAuthTokenConsumption(IAppBuilder app)
        {
            string issuer = ConfigurationManager.AppSettings["as:Issuer"];
            string audienceId = ConfigurationManager.AppSettings["as:AudienceId"];
            byte[] audienceSecret = TextEncodings.Base64Url.Decode(ConfigurationManager.AppSettings["as:AudienceSecret"]);

            app.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    AuthenticationMode = AuthenticationMode.Active,
                    AllowedAudiences = new[] { audienceId },
                    IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]
                    {
                        new SymmetricKeyIssuerSecurityTokenProvider(issuer, audienceSecret),
                        new CustomIssuerSecurityTokenProvider(issuer)
                    },
                    Provider = new CustomOAuthBearerProvider()
                });
        }
    }
}
