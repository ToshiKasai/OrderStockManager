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
                IQueryable<UserModel> query = this.AppUserManager.Users.OrderBy(x => x.Id);
                if (!parameter.Deleted)
                    query = query.Where(x => x.Deleted == false);
                if (parameter.Enabled)
                    query = query.Where(x => x.Enabled == true);

                if (parameter.Limit.HasValue)
                {
                    if (parameter.Page.HasValue)
                        query = query.Skip((int)parameter.Limit * (int)parameter.Page).Take((int)parameter.Limit);
                    else
                        query = query.Take((int)parameter.Limit);
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

        public async Task<UserInterfaceModel> GetUserByIdForInterfaceAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return null;

                var user = await AppUserManager.FindByIdAsync(id);
                if (user == null)
                    return null;

                return Mapper.Map<UserInterfaceModel>(user);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<UserInterfaceModel> GetUserByNameForInterfaceAsync(string userName)
        {
            try
            {
                if (string.IsNullOrEmpty(userName))
                    return null;

                var user = await AppUserManager.FindByNameAsync(userName);
                if (user == null)
                    return null;

                return Mapper.Map<UserInterfaceModel>(user);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<IdentityResult> CreateUserAsync(UserInterfaceModel createUser)
        {
            try
            {
                using (DataContext dbContext = DataContext.Create())
                using (DbContextTransaction tx = dbContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    if (await AppUserManager.FindByIdAsync(createUser.Id) != null)
                    {
                        tx.Rollback();
                        return new IdentityResult("Conflict");
                    }

                    var user = new UserModel();
                    user.UserName = createUser.UserName;
                    user.Name = createUser.Name;
                    user.Email = createUser.Email;
                    user.Enabled = createUser.Enabled;
                    IdentityResult result = await AppUserManager.CreateAsync(user, createUser.NewPassword);
                    if (!result.Succeeded)
                    {
                        tx.Rollback();
                    }
                    else
                    {
                        tx.Commit();
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IdentityResult> ModifyUserAsync(int id, UserInterfaceModel modifiedUser)
        {
            try
            {
                if (id <= 0)
                    return new IdentityResult("BadRequest");

                using (DataContext dbContext = DataContext.Create())
                using (DbContextTransaction tx = dbContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    modifiedUser.Id = id;
                    UserModel user = await AppUserManager.FindByIdAsync(modifiedUser.Id);
                    if (user == null)
                    {
                        tx.Rollback();
                        return new IdentityResult("NotFound");
                    }

                    IdentityResult result;
                    if (!string.IsNullOrEmpty(modifiedUser.NewPassword))
                    {
                        string code = await AppUserManager.GeneratePasswordResetTokenAsync(user.Id);
                        result = await AppUserManager.ResetPasswordAsync(user.Id, code, modifiedUser.NewPassword);
                        if (!result.Succeeded)
                        {
                            tx.Rollback();
                            return result;
                        }
                    }

                    ModifiedUserModel(user, modifiedUser);
                    result = await AppUserManager.UpdateAsync(user);
                    if (!result.Succeeded)
                    {
                        tx.Rollback();
                        return result;
                    }

                    tx.Commit();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IdentityResult> DeleteUserAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return new IdentityResult("BadRequest");

                using (DataContext dbContext = DataContext.Create())
                using (DbContextTransaction tx = dbContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    UserModel user = await AppUserManager.FindByIdAsync(id);
                    if (user == null)
                    {
                        tx.Rollback();
                        return new IdentityResult("NotFound");
                    }

                    IdentityResult result = await AppUserManager.DeleteAsync(user);
                    if (!result.Succeeded)
                    {
                        tx.Rollback();
                        return result;
                    }
                    tx.Commit();
                    return result;
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
                    maxpages = CountToPages(maxcount, parameter.Limit);
                    return new { count = maxcount, pages = maxpages };
                }
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
