using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;

namespace OrderStockManager.Providers
{
    public class RSAKeyProvider : IRSAKeyProvider
    {
        private CspParameters cp;

        public RSAKeyProvider()
        {
            cp = new CspParameters();
            cp.KeyContainerName = "myKeys";
            // cp.Flags = CspProviderFlags.UseMachineKeyStore;
        }

        public string CreateKey(bool includePrivateParameters = false)
        {
            using (RSACryptoServiceProvider myRSA = new RSACryptoServiceProvider(2048, cp))
            {
                return myRSA.ToXmlString(includePrivateParameters);
            }
        }

        public void DeleteKeys()
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(cp))
            {
                rsa.PersistKeyInCsp = false;
                rsa.Clear();
            }
        }

        public string GetPrivateKey()
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(cp))
            {
                if (rsa.KeySize != 2048)
                {
                    DeleteKeys();
                    return CreateKey(true);
                }
                return rsa.ToXmlString(true);
            }
        }

        public string GetPublicKey()
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(cp))
            {
                if (rsa.KeySize != 2048)
                {
                    DeleteKeys();
                    return CreateKey(true);
                }
                return rsa.ToXmlString(false);
            }
        }

    }
}
