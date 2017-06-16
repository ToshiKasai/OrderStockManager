using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;

namespace OrderStockManager.Infrastructure
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class CompressionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            var acceptEncoding = actionExecutedContext.Request.Headers.AcceptEncoding.Select(x=>x.Value);
            // var acceptEncoding = actionExecutedContext.Request.Content.Headers.Where(x => x.Key == "Accept-Encoding").Select(x => x.Value).FirstOrDefault();

            if (acceptEncoding.Contains("gzip"))
            {
                var content = actionExecutedContext.Response.Content;
                var bytes = content == null ? null : content.ReadAsByteArrayAsync().Result;
                var zlibbedContent = bytes == null ? new byte[0] : CompressionHelper.GzipByte(bytes);
                actionExecutedContext.Response.Content = new ByteArrayContent(zlibbedContent);
                actionExecutedContext.Response.Content.Headers.Remove("Content-Type");
                actionExecutedContext.Response.Content.Headers.Remove("Content-encoding");
                actionExecutedContext.Response.Content.Headers.Add("Content-encoding", "deflate");
                actionExecutedContext.Response.Content.Headers.Add("Content-Type", "application/json");
            }
            else if (acceptEncoding.Contains("deflate"))
            {
                var content = actionExecutedContext.Response.Content;
                var bytes = content == null ? null : content.ReadAsByteArrayAsync().Result;
                var zlibbedContent = bytes == null ? new byte[0] : CompressionHelper.DeflateByte(bytes);
                actionExecutedContext.Response.Content = new ByteArrayContent(zlibbedContent);
                actionExecutedContext.Response.Content.Headers.Remove("Content-Type");
                actionExecutedContext.Response.Content.Headers.Remove("Content-encoding");
                actionExecutedContext.Response.Content.Headers.Add("Content-encoding", "deflate");
                actionExecutedContext.Response.Content.Headers.Add("Content-Type", "application/json");
            }
            else
            {
                base.OnActionExecuted(actionExecutedContext);
            }
        }
    }

    public class CompressionHelper
    {
        public static byte[] DeflateByte(byte[] str)
        {
            if (str == null)
            {
                return null;
            }

            using (var output = new MemoryStream())
            using (var compressor = new DeflateStream(output, CompressionMode.Compress, CompressionLevel.BestSpeed))
            {
                compressor.Write(str, 0, str.Length);
                return output.ToArray();
            }
        }

        public static byte[] GzipByte(byte[] str)
        {
            if (str == null)
            {
                return null;
            }

            using (var output = new MemoryStream())
            using (var compressor = new GZipStream(output, CompressionMode.Compress, CompressionLevel.BestSpeed))
            {
                compressor.Write(str, 0, str.Length);
                return output.ToArray();
            }
        }
    }
}
