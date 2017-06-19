using OrderStockManager.Models.Parameters;
using OrderStockManager.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace OrderStockManager.Controllers
{
    /// <summary>
    /// ユーザー情報管理用WEBAPI
    /// </summary>
    [Authorize]
    [RoutePrefix("api/roles")]
    public class RolesController : BaseApiController
    {
        private IRoleRepository repository = null;

        public RolesController(IRoleRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        [Authorize(Roles = "admin,user")]
        public IHttpActionResult Get([FromUri]BaseParameterModel param)
        {
            try
            {
                var result = repository.GetRolesForInterface(param, User.IsInRole("admin"));
                if (result == null)
                    return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IHttpActionResult> GetAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest();
                }

                var result = await repository.GetRoleByIdForInterfaceAsync(id);
                if (result.Code == HttpStatusCode.OK)
                {
                    return Content(result.Code, result.message);
                }
                return Ok(result.resultData);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [ValidationRequired(prefix = "value")]
        public IHttpActionResult Post([FromBody]RoleInterfaceModel value)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }

        [HttpPut]
        [Authorize(Roles = "admin")]
        [ValidationRequired(prefix = "value")]
        [Route("{id}")]
        public IHttpActionResult Put(int id, [FromBody]RoleInterfaceModel value)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }

        [HttpDelete]
        [Authorize(Roles = "admin")]
        [Route("{id}")]
        public IHttpActionResult Delete(int id)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }
    }
}
