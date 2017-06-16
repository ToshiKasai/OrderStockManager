using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace OrderStockManager.Services
{
    public class CryptographService
    {
        public static string CreateMacCode(string target, string key)
        {
            byte[] data = Encoding.UTF8.GetBytes(target);
            byte[] keyData = Encoding.UTF8.GetBytes(key);

            // using (var hmac = new HMACSHA512(keyData))
            using (var hmac = new HMACSHA384(keyData))
            {
                var bs = hmac.ComputeHash(data);
                hmac.Clear();
                return Convert.ToBase64String(bs);
            }
        }
    }
}
