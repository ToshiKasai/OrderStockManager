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
                if (User.Identity.IsAuthenticated)
                {
                    var authtyp = User.Identity.AuthenticationType;
                    var isAdmin = User.IsInRole("admin");
                    var isUser = User.IsInRole("user");
                }

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

#if false
        #region ユーザーメーカー情報
        [HttpGet]
        [Route("api/Users/{id}/Makers")]
        public IHttpActionResult GetMakerList(int id, [FromUri]BaseApiParameterModel param)
        {
            try
            {
                if (id <= 0 || UserManager.FindById(id) == null)
                    return BadRequest(Messages.ApiIllegalParameter);

                IList<MakerApiModel> makerList = dbContext.UserMakerModels
                    .Where(um => um.UserModelId == id).Where(um => um.Deleted == false)
                    .Select(um => um.MakerModel).OrderBy(m => m.Id).ProjectTo<MakerApiModel>().ToList();
                return Ok(makerList);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("api/Users/{id}/Makers")]
        [ValidationRequired(prefix = "MakerList")]
        public IHttpActionResult SetMakerList(int id, [FromBody]List<MakerApiModel> MakerList)
        {
            try
            {
                int count = 0;

                if (id <= 0 || UserManager.FindById(id) == null)
                    return BadRequest(Messages.ApiIllegalParameter);

                dbContext.Database.ExecuteSqlCommand(ContextResources.IncrementResetUserMaker);

                using (DbContextTransaction tx =
                    dbContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {

                    List<int> setMakerList = MakerList.Select(m => m.Id).OrderBy(x => x).ToList();
                    IQueryable<UserMakerModel> userMakers = dbContext.UserMakerModels.Where(x => x.UserModelId == id).OrderBy(x => x.MakerModelId);
                    List<int> makerIdList = dbContext.MakerModels.Select(x => x.Id).OrderBy(x => x).ToList<int>();

                    foreach (int item in makerIdList)
                    {
                        UserMakerModel check = userMakers.Where(x => x.MakerModelId == item).FirstOrDefault();
                        if (check == null && setMakerList.Contains(item))
                        {
                            count++;
                            check = new UserMakerModel();
                            check.UserModelId = id;
                            check.MakerModelId = item;
                            dbContext.UserMakerModels.Add(check);
                        }
                        else if (check != null && setMakerList.Contains(item) && check.Deleted == true)
                        {
                            count++;
                            check.Deleted = false;
                            dbContext.Entry(check).State = EntityState.Modified;
                        }
                        else if (check != null && !setMakerList.Contains(item) && check.Deleted == false)
                        {
                            count++;
                            check.Deleted = true;
                            dbContext.Entry(check).State = EntityState.Modified;
                        }
                    }
#if false
                    if (dbContext.SaveChanges() == count)
                    {
                        tx.Rollback();
                        return Conflict();
                    }
#endif
                    dbContext.SaveChanges();
                    tx.Commit();
                }
                dbContext.Database.ExecuteSqlCommand(ContextResources.IncrementResetUserMaker);

                return Ok(MakerList);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        #endregion
#endif
    }
}
