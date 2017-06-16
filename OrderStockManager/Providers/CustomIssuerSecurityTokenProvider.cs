using Microsoft.Owin.Security.Jwt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace OrderStockManager.Providers
{
    public class CustomIssuerSecurityTokenProvider : IIssuerSecurityTokenProvider
    {
        private string issuer = string.Empty;

        public CustomIssuerSecurityTokenProvider(string issuer)
        {
            this.issuer = issuer;
        }

        public string Issuer
        {
            get
            {
                return this.issuer;
            }
        }

        public IEnumerable<SecurityToken> SecurityTokens
        {
            get
            {
                IRSAKeyProvider _providor = new RSAKeyProvider();
                using (var rsaProvider = new RSACryptoServiceProvider())
                {
                    rsaProvider.FromXmlString(_providor.GetPrivateKey());
                    yield return new RsaSecurityToken(rsaProvider);
                }
            }
        }
    }
}
