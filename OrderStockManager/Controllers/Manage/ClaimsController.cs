using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace OrderStockManager.Controllers
{
    [RoutePrefix("api/claims")]
    public class ClaimsController : BaseApiController
    {
        [Authorize]
        [Route("")]
        public IHttpActionResult GetClaims()
        {
            var identity = User.Identity as ClaimsIdentity;
            var claims = identity.Claims.Select(c => new { subject = c.Subject.Name, type = c.Type, value = c.Value });
#if false
            var claims2 = from c in identity.Claims
                            select new
                            {
                                subject = c.Subject.Name,
                                type = c.Type,
                                value = c.Value
                            };
#endif
            return Ok(claims);
        }
    }
}
