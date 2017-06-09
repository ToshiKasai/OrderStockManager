using AutoMapper;
using AutoMapper.QueryableExtensions;
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

namespace OrderStockManager.Controllers
{
    /// <summary>
    /// ユーザー情報管理用WEBAPI
    /// </summary>
    [Authorize]
    public class UsersController : BaseApiController
    {
        private IUserRepository repository = null;

        public UsersController(IUserRepository repository)
        {
            this.repository = repository;
        }

        #region ユーザー情報
        [HttpGet]
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
        public async Task<IHttpActionResult> GetAsync(int id, [FromUri]BaseParameterModel param)
        {
            try
            {
                if (id <= 0)
                    return BadRequest();

                var result = await repository.GetUserByIdForInterfaceAsync(id);
                if (result == null)
                    return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [ValidationRequired(prefix = "value")]
        public async Task<IHttpActionResult> PostAsync([FromBody]UserInterfaceModel value)
        {
            try
            {
                var result = await repository.CreateUserAsync(value);
                if (!result.Succeeded)
                {
                    // GetErrorResult(result);
                    AddErrors(result);
                    return BadRequest(ModelState.GetErrorsDelprefix("value"));
                }
                var createdUser = repository.GetUserByNameForInterfaceAsync(value.UserName);
                return Created(Request.RequestUri + "/" + createdUser.Id, createdUser);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [ValidationRequired(prefix = "value")]
        public async Task<IHttpActionResult> PutAsync(int id, [FromBody]UserInterfaceModel value)
        {
            try
            {
                if (id <= 0)
                    return BadRequest();

                var result = await repository.ModifyUserAsync(id, value);
                if (!result.Succeeded)
                {
                    AddErrors(result);
                    return BadRequest(ModelState.GetErrorsDelprefix("value"));
                }
                var modifiedUser = repository.GetUserByIdForInterfaceAsync(id);
                return Ok(modifiedUser);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete]
        public async Task<IHttpActionResult> DeleteAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest();

                var result = await repository.DeleteUserAsync(id);
                if (!result.Succeeded)
                {
                    return InternalServerError();
                }
                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("api/Users/Pages")]
        public IHttpActionResult GetUserMaxPages([FromUri]BaseParameterModel param)
        {
            try
            {
                var result = repository.GetUsersPages(param);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        #endregion
#if false

        #region ユーザーロール情報
        [HttpGet]
        [Route("api/Users/{id}/Roles")]
        public async Task<IHttpActionResult> GetRoleList(int id, [FromUri]BaseApiParameterModel param)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(Messages.ApiIllegalParameter);

                IList<string> roleList = await UserManager.GetRolesAsync(id);
                if (roleList == null)
                    return NotFound();

                if (!User.IsInRole("admin"))
                    roleList = roleList.Where(rl => rl != "admin").ToList();

                return Ok(roleList);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("api/Users/{id}/Roles/{role}")]
        public async Task<IHttpActionResult> GetRole(int id, string role, [FromUri]BaseApiParameterModel param)
        {
            try
            {
                if (id <= 0 || UserManager.FindById(id) == null)
                    return BadRequest(Messages.ApiIllegalParameter);

                IList<string> roleList = await UserManager.GetRolesAsync(id);
                if (!User.IsInRole("admin"))
                    roleList = roleList.Where(rl => rl != "admin").ToList();

                return Ok(roleList.FirstOrDefault(r => r == role));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("api/Users/{id}/Roles")]
        [ValidationRequired(prefix = "roles")]
        public IHttpActionResult SetRoleList(int id, [FromBody]List<string> roles)
        {
            try
            {
                if (id <= 0 || UserManager.FindById(id) == null)
                    return BadRequest(Messages.ApiIllegalParameter);

                IList<string> roleList = UserManager.GetRoles(id);
                if (!User.IsInRole("admin"))
                    roleList = roleList.Where(rl => rl != "admin").ToList();

                IdentityResult result;
                result = UserManager.RemoveFromRoles(id, roleList.ToArray());
                if (!result.Succeeded)
                {
                    AddErrors(result);
                    return BadRequest(ModelState.GetErrorsDelprefix("value"));
                }

                result = UserManager.AddToRoles(id, roles.ToArray());
                if (!result.Succeeded)
                {
                    AddErrors(result);
                    return BadRequest(ModelState.GetErrorsDelprefix("value"));
                }

                return Ok(roles);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        #endregion

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

        #region サポート機能
        private bool UpdateUserModel(UserModel user, UserApiModel model)
        {
            try
            {
                // ログインＩＤ
                user.UserName = model.UserName;

                // ユーザー名
                user.Name = model.Name;

                // アカウント有効期限
                if (model.NewExpiration.HasValue)
                {
                    user.Expiration = ((DateTime)model.NewExpiration).ToLocalTime();
                }

                // パスワード変更スキップ回数
                user.PasswordSkipCnt = model.PasswordSkipCnt;

                // メールアドレス
                user.Email = model.Email;

                // メールアドレス確認済
                user.EmailConfirmed = model.EmailConfirmed;

                // ロックアウト終了日時
                if (model.LockoutEndData.HasValue)
                    user.LockoutEndData = (DateTime)model.LockoutEndData;

                // ロックアウト許可
                user.LockoutEnabled = model.LockoutEnabled;

                // ログイン失敗回数
                user.AccessFailedCount = model.AccessFailedCount;

                // ユーザー使用許可
                user.Enabled = model.Enabled;

                // 削除済
                user.Deleted = model.Deleted;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion
#endif
    }
}
