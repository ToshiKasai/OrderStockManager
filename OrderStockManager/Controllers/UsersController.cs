using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNet.Identity;
using OrderStockManager.Models;
using OrderStockManager.Models.Parameters;
using OrderStockManager.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Routing;

namespace OrderStockManager.Controllers
{
    /// <summary>
    /// ユーザー情報管理用WEBAPI
    /// </summary>
    [Authorize]
    [RoutePrefix("api/users")]
    public class UsersController : BaseApiController
    {
        private IUserRepository repository = null;

        public UsersController(IUserRepository repository)
        {
            this.repository = repository;
        }

        #region ユーザー情報
        [HttpGet]
        [Authorize(Roles = "admin,user")]
        public IHttpActionResult Get([FromUri]BaseParameterModel param)
        {
            try
            {
                var result = repository.GetUsersForInterface(param);
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
        [Route("{id}", Name = "GetUsersById")]
        public async Task<IHttpActionResult> GetAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest();

                var result = await repository.GetUserByIdForInterfaceAsync(id);
                if (result.Code != HttpStatusCode.OK)
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
        [ValidationRequired(prefix = "value")]
        [Authorize(Roles = "admin,user")]
        public async Task<IHttpActionResult> PostAsync([FromBody]UserInterfaceModel value)
        {
            try
            {
                var result = await repository.CreateUserAsync(value);
                if (result.Code != HttpStatusCode.Created)
                {
                    // GetErrorResult(result);
                    AddErrors(result.identityResult);
                    return Content(result.Code, ModelState.GetErrorsDelprefix("value"));
                }

                result = await repository.GetUserByNameForInterfaceAsync(value.UserName);

                var _UrlHelper = new UrlHelper(this.Request);
                var Url = _UrlHelper.Link("GetUsersById", new { id = result.resultData.Id });
                return Created(Url, result.resultData);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [ValidationRequired(prefix = "value")]
        [Route("{id}")]
        public async Task<IHttpActionResult> PutAsync(int id, [FromBody]UserInterfaceModel value)
        {
            try
            {
                if (id <= 0)
                    return BadRequest();

                var result = await repository.ModifyUserAsync(id, value);
                if (result.Code != HttpStatusCode.OK)
                {
                    AddErrors(result.identityResult);
                    return Content(result.Code, ModelState.GetErrorsDelprefix("value"));
                }
                var modifiedUser = await repository.GetUserByIdForInterfaceAsync(id);
                return Ok(modifiedUser.resultData);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "admin,user")]
        public async Task<IHttpActionResult> DeleteAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest();

                var result = await repository.DeleteUserAsync(id);
                if (result.Code != HttpStatusCode.OK)
                {
                    return Content(result.Code, result.message);
                    // return InternalServerError();
                }
                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("pages")]
        public IHttpActionResult GetUserMaxPages([FromUri]BaseParameterModel param)
        {
            try
            {
                var result = repository.GetUsersPages(param);
                if (result == null)
                {
                    return BadRequest();
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        #endregion

        #region ユーザーロール情報
        [HttpGet]
        [Route("{id}/roles")]
        public async Task<IHttpActionResult> GetRoleListAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest();
                }

                var result = await repository.GetRoleByUserIdAsync(id, User.IsInRole("admin"));
                if (result.Code != HttpStatusCode.OK)
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

        [HttpGet]
        [Route("{id}/roles/{role}")]
        [ValidationRequired(prefix = "roles")]
        public async Task<IHttpActionResult> GetRoleAsync(int id, string role)
        {
            try
            {
                if (id <= 0 || string.IsNullOrWhiteSpace(role))
                {
                    return BadRequest();
                }

                var result = await repository.GetRoleByUserIdAndRoleNameAsync(id, role, User.IsInRole("admin"));
                if (result.Code != HttpStatusCode.OK)
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
        [Route("{id}/roles")]
        [Authorize(Roles = "admin,user")]
        [ValidationRequired(prefix = "roles")]
        public async Task<IHttpActionResult> SetRoleListAsync(int id, [FromBody]List<string> roles)
        {
            try
            {
                if (id <= 0 || roles == null)
                {
                    return BadRequest();
                }

                var result = await repository.SetRoleByUserIdAsync(id, roles, User.IsInRole("admin"));
                if (result.Code != HttpStatusCode.OK)
                {
                    AddErrors(result.identityResult);
                    return Content(result.Code, ModelState.GetErrorsDelprefix("roles"));
                }
                return Ok(result.resultData);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        #endregion

        #region ユーザーメーカー情報
        [HttpGet]
        [Route("{id}/makers")]
        public async Task<IHttpActionResult> GetMakerListAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest();
                }

                var result = await repository.GetMakersByUserIdAsync(id);
                if (result.Code != HttpStatusCode.OK)
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
        [Route("{id}/makers")]
        [ValidationRequired(prefix = "MakerList")]
        public async Task<IHttpActionResult> SetMakerListAsync(int id, [FromBody]List<MakerInterfaceModel> MakerList)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest();
                }

                var result = await repository.SetMakersByUSerIdAsync(id, MakerList);
                if (result.Code != HttpStatusCode.OK)
                {
                    return Content(result.Code, result.message);
                }

                result = await repository.GetMakersByUserIdAsync(id);
                if (result.Code != HttpStatusCode.OK)
                {
                    return Ok();
                }
                return Ok(result.resultData);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        #endregion
    }
}
