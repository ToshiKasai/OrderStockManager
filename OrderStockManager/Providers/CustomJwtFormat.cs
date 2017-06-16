using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using System;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Security.Cryptography;
using Thinktecture.IdentityModel.Tokens;

namespace OrderStockManager.Providers
{
    /// <summary>
    /// JWT生成用カスタムクラス
    /// </summary>
    public class CustomJwtFormat : ISecureDataFormat<AuthenticationTicket>
    {
        /// <summary>
        /// JWT発行者
        /// </summary>
        private readonly string _issuer = string.Empty;
        private readonly string _algorithm = Microsoft.IdentityModel.Tokens.SecurityAlgorithms.RsaSha512;
        private IRSAKeyProvider _rsaProvider = null;

        public CustomJwtFormat(string issuer)
        {
            _issuer = issuer;
        }

        public CustomJwtFormat(string issuer, string algorithm) : this(issuer)
        {
            _algorithm = algorithm;
        }

        private SigningCredentials CreateHmac()
        {
            string audienceId = ConfigurationManager.AppSettings["as:AudienceId"];
            string symmetricKeyAsBase64 = ConfigurationManager.AppSettings["as:AudienceSecret"];

            var keyByteArray = TextEncodings.Base64Url.Decode(symmetricKeyAsBase64);
            var signingKey = new HmacSigningCredentials(keyByteArray);  // 32:HmacSha256 / 48:HmacSha384 / 64:HmacSha512

            // var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(keyByteArray);
            // var signingCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256Signature);

            return signingKey;
        }

        private SigningCredentials CreateRsa()
        {
            this._rsaProvider = new RSAKeyProvider();
            string rsaKey = _rsaProvider.GetPrivateKey();
            if (rsaKey == null)
            {
                throw new Exception();
            }
            var rsaProvider = new RSACryptoServiceProvider();
            rsaProvider.FromXmlString(rsaKey);

            // var signingKey = new SigningCredentials(new RsaSecurityKey(rsaProvider), Microsoft.IdentityModel.Tokens.SecurityAlgorithms.RsaSha256Signature, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.Sha256Digest);
            // var signingKey = new SigningCredentials(new RsaSecurityKey(rsaProvider), Microsoft.IdentityModel.Tokens.SecurityAlgorithms.RsaSha384Signature, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.Sha384Digest);
            var signingKey = new SigningCredentials(new RsaSecurityKey(rsaProvider), Microsoft.IdentityModel.Tokens.SecurityAlgorithms.RsaSha512Signature, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.Sha512Digest);

            return signingKey;
        }

        public string Protect(AuthenticationTicket data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            try
            {
                SigningCredentials signingKey;
                if (_algorithm== Microsoft.IdentityModel.Tokens.SecurityAlgorithms.RsaSha512)
                {
                    signingKey = CreateRsa();
                }
                else
                {
                    signingKey = CreateHmac();
                }

                string audienceId = ConfigurationManager.AppSettings["as:AudienceId"];
                var issued = data.Properties.IssuedUtc;
                var expires = data.Properties.ExpiresUtc;
                var token = new JwtSecurityToken(_issuer, audienceId, data.Identity.Claims, issued.Value.UtcDateTime, expires.Value.UtcDateTime, signingKey);
                var handler = new JwtSecurityTokenHandler();

                var jwt = handler.WriteToken(token);
                return jwt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public AuthenticationTicket Unprotect(string protectedText)
        {
            throw new NotImplementedException();
        }
    }
}