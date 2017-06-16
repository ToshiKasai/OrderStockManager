using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNet.Identity;
using OrderStockManager.Infrastructure;
using OrderStockManager.Models;
using OrderStockManager.Models.Parameters;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace OrderStockManager.Repositories
{
    public class UserRepository : BaseRepository, IUserRepository, IDisposable
    {
        public UserRepository()
        {
            Mapper.Initialize(cfg =>
            cfg.CreateMap<UserModel, UserInterfaceModel>()
                .ForMember(d => d.NewExpiration, o => o.Ignore())
                .ForMember(d => d.NewPassword, o => o.Ignore())
                );
            // Mapper.AssertConfigurationIsValid();
        }

        public IEnumerable<UserInterfaceModel> GetUsersForInterface(BaseParameterModel parameter)
        {
            try
            {
                IQueryable<UserModel> query = AppUserManager.Users.OrderBy(x => x.Id);
                if (!parameter.Deleted)
                {
                    query = query.Where(x => x.Deleted == false);
                }
                if (parameter.Enabled)
                {
                    query = query.Where(x => x.Enabled == true);
                }

                if (parameter.Limit.HasValue)
                {
                    if (parameter.Page.HasValue)
                    {
                        query = query.Skip((int)parameter.Limit * (int)parameter.Page).Take((int)parameter.Limit);
                    }
                    else
                    {
                        query = query.Take((int)parameter.Limit);
                    }
                }

                var result = query.ProjectTo<UserInterfaceModel>().ToList();
                if (result == null || result.Count == 0)
                    return null;

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<RepositoryResult<UserInterfaceModel>> GetUserByIdForInterfaceAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return new RepositoryResult<UserInterfaceModel>(HttpStatusCode.BadRequest);
                }

                var user = await AppUserManager.FindByIdAsync(id);
                if (user == null)
                {
                    return new RepositoryResult<UserInterfaceModel>(HttpStatusCode.NotFound);
                }

                return new RepositoryResult<UserInterfaceModel>(HttpStatusCode.OK, Mapper.Map<UserInterfaceModel>(user));
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<RepositoryResult<UserInterfaceModel>> GetUserByNameForInterfaceAsync(string userName)
        {
            try
            {
                if (string.IsNullOrEmpty(userName))
                {
                    return new RepositoryResult<UserInterfaceModel>(HttpStatusCode.BadRequest);
                }

                var user = await AppUserManager.FindByNameAsync(userName);
                if (user == null)
                {
                    return new RepositoryResult<UserInterfaceModel>(HttpStatusCode.NotFound);
                }

                return new RepositoryResult<UserInterfaceModel>(HttpStatusCode.OK, Mapper.Map<UserInterfaceModel>(user));
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<RepositoryResult<UserInterfaceModel>> CreateUserAsync(UserInterfaceModel createUser)
        {
            try
            {
                if (await AppUserManager.FindByIdAsync(createUser.Id) != null)
                {
                    return new RepositoryResult<UserInterfaceModel>(HttpStatusCode.Conflict);
                }

                if(string.IsNullOrWhiteSpace(createUser.NewPassword))
                {
                    return new RepositoryResult<UserInterfaceModel>(HttpStatusCode.BadRequest, "パスワードは必須項目です。");
                }

                var user = new UserModel();
                user.UserName = createUser.UserName;
                user.Name = createUser.Name;
                user.Email = createUser.Email;
                user.Enabled = createUser.Enabled;

                var result = new RepositoryResult<UserInterfaceModel>(HttpStatusCode.Created);
                result.identityResult = await AppUserManager.CreateAsync(user, createUser.NewPassword);
                if (!result.identityResult.Succeeded)
                {
                    result.Code = HttpStatusCode.BadRequest;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<RepositoryResult<UserInterfaceModel>> ModifyUserAsync(int id, UserInterfaceModel modifiedUser)
        {
            try
            {
                if (id <= 0)
                {
                    return new RepositoryResult<UserInterfaceModel>(HttpStatusCode.BadRequest);
                }

                using (DataContext dbContext = DataContext.Create())
                using (DbContextTransaction tx = dbContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    modifiedUser.Id = id;
                    UserModel user = await AppUserManager.FindByIdAsync(modifiedUser.Id);
                    if (user == null)
                    {
                        tx.Rollback();
                        return new RepositoryResult<UserInterfaceModel>(HttpStatusCode.NotFound);
                    }

                    IdentityResult result;
                    if (!string.IsNullOrEmpty(modifiedUser.NewPassword))
                    {
                        string code = await AppUserManager.GeneratePasswordResetTokenAsync(user.Id);
                        result = await AppUserManager.ResetPasswordAsync(user.Id, code, modifiedUser.NewPassword);
                        if (!result.Succeeded)
                        {
                            tx.Rollback();
                            return new RepositoryResult<UserInterfaceModel>(HttpStatusCode.BadRequest) { identityResult = result };
                        }
                    }

                    ModifiedUserModel(user, modifiedUser);
                    result = await AppUserManager.UpdateAsync(user);
                    if (!result.Succeeded)
                    {
                        tx.Rollback();
                        return new RepositoryResult<UserInterfaceModel>(HttpStatusCode.BadRequest) { identityResult = result };
                    }

                    tx.Commit();
                    return new RepositoryResult<UserInterfaceModel>(HttpStatusCode.OK) { identityResult = result };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<RepositoryResult<UserInterfaceModel>> DeleteUserAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return new RepositoryResult<UserInterfaceModel>(HttpStatusCode.BadRequest);
                }

                using (DataContext dbContext = DataContext.Create())
                using (DbContextTransaction tx = dbContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    UserModel user = await AppUserManager.FindByIdAsync(id);
                    if (user == null)
                    {
                        tx.Rollback();
                        return new RepositoryResult<UserInterfaceModel>(HttpStatusCode.NotFound);
                    }

                    IdentityResult result = await AppUserManager.DeleteAsync(user);
                    if (!result.Succeeded)
                    {
                        tx.Rollback();
                        return new RepositoryResult<UserInterfaceModel>(HttpStatusCode.BadRequest) { identityResult = result };
                    }
                    tx.Commit();
                    return new RepositoryResult<UserInterfaceModel>(HttpStatusCode.OK) { identityResult = result };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public object GetUsersPages(BaseParameterModel parameter)
        {
            try
            {
                if (parameter == null)
                {
                    return null;
                }
                if (!parameter.Limit.HasValue)
                {
                    return null;
                }
                else if (parameter.Limit.GetValueOrDefault() <= 0)
                {
                    return null;
                }

                using (DataContext dbContext = DataContext.Create())
                {
                    int maxcount = 0;
                    int maxpages = 0;
                    IQueryable<UserModel> query;
                    query = dbContext.UserModels.OrderBy(x => x.Id);
                    if (!parameter.Deleted)
                        query = query.Where(x => x.Deleted == false);
                    if (parameter.Enabled)
                        query = query.Where(x => x.Enabled == true);
                    maxcount = query.Count();
                    maxpages = CountToPages(maxcount, parameter.Limit.GetValueOrDefault());
                    return new { count = maxcount, pages = maxpages };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<RepositoryResult<IList<string>>> GetRoleByUserIdAsync(int userId, bool isAdmin = false)
        {
            try
            {
                if (userId <= 0)
                {
                    return new RepositoryResult<IList<string>>(HttpStatusCode.BadRequest);
                }

                IList<string> roleList = await AppUserManager.GetRolesAsync(userId);
                if (roleList == null)
                {
                    return new RepositoryResult<IList<string>>(HttpStatusCode.NotFound);
                }

                if (!isAdmin)
                {
                    roleList = roleList.Where(rl => rl != "admin").ToList();
                }

                return new RepositoryResult<IList<string>>(HttpStatusCode.OK, roleList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<RepositoryResult<string>> GetRoleByUserIdAndRoleNameAsync(int userId, string roleName, bool isAdmin = false)
        {
            try
            {
                if (userId <= 0 || string.IsNullOrWhiteSpace(roleName))
                {
                    return new RepositoryResult<string>(HttpStatusCode.BadRequest);
                }
                var user = AppUserManager.FindById(userId);
                if (user == null)
                {
                    return new RepositoryResult<string>(HttpStatusCode.NotFound);
                }

                IList<string> roleList = await AppUserManager.GetRolesAsync(user.Id);
                if (!isAdmin)
                {
                    roleList = roleList.Where(rl => rl != "admin").ToList();
                }

                return new RepositoryResult<string>(HttpStatusCode.OK, roleList.FirstOrDefault(r => r == roleName));
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<RepositoryResult<IList<string>>> SetRoleByUserIdAsync(int userId, IList<string> roleList, bool isAdmin = false)
        {
            try
            {
                if (userId <= 0 || roleList == null)
                {
                    return new RepositoryResult<IList<string>>(HttpStatusCode.BadRequest);
                }
                var user = AppUserManager.FindById(userId);
                if (user == null)
                {
                    return new RepositoryResult<IList<string>>(HttpStatusCode.NotFound);
                }

                var currentRoles = await AppUserManager.GetRolesAsync(user.Id);
                if (!isAdmin)
                    currentRoles = currentRoles.Where(rl => rl != "admin").ToList();

                var rolesNotExists = roleList.Except(AppRoleManager.Roles.Select(x => x.Name)).ToArray();
                if (rolesNotExists.Count() > 0)
                {
                    return new RepositoryResult<IList<string>>(HttpStatusCode.BadRequest, string.Format("ロール'{0}'は存在していません。", string.Join(",", rolesNotExists)));
                }

                IdentityResult removeResult = await AppUserManager.RemoveFromRolesAsync(user.Id, currentRoles.ToArray());
                if (!removeResult.Succeeded)
                {
                    return new RepositoryResult<IList<string>>(HttpStatusCode.BadRequest, "権限の剥奪に失敗しました。") { identityResult = removeResult };
                }

                IdentityResult addResult = await AppUserManager.AddToRolesAsync(user.Id, roleList.ToArray());
                if (!addResult.Succeeded)
                {
                    return new RepositoryResult<IList<string>>(HttpStatusCode.BadRequest, "権限の付与に失敗しました。") { identityResult = addResult };
                }

                return new RepositoryResult<IList<string>>(HttpStatusCode.OK, roleList);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        #region Internal
        private bool ModifiedUserModel(UserModel user, UserInterfaceModel model)
        {
            try
            {
                user.UserName = model.UserName;
                user.Name = model.Name;

                if (model.NewExpiration.HasValue)
                {
                    user.Expiration = ((DateTime)model.NewExpiration).ToLocalTime();
                }

                user.PasswordSkipCnt = model.PasswordSkipCnt;

                user.Email = model.Email;
                user.EmailConfirmed = model.EmailConfirmed;

                if (model.LockoutEndData.HasValue)
                    user.LockoutEndData = (DateTime)model.LockoutEndData;

                user.LockoutEnabled = model.LockoutEnabled;
                user.AccessFailedCount = model.AccessFailedCount;

                user.Enabled = model.Enabled;
                user.Deleted = model.Deleted;
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false;

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposedValue)
            {
                if (disposing)
                {
                }
                disposedValue = true;
            }
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
