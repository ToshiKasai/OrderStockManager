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
    public class RoleRepository : BaseRepository, IRoleRepository, IDisposable
    {
        public RoleRepository()
        {
            Mapper.Initialize(cfg =>
                cfg.CreateMap<RoleModel, RoleInterfaceModel>()
            );
            // Mapper.AssertConfigurationIsValid();
        }

        public IEnumerable<RoleInterfaceModel> GetRolesForInterface(BaseParameterModel parameter, bool isAdmin = false)
        {
            try
            {
                IQueryable<RoleModel> query = AppRoleManager.Roles.OrderBy(x => x.Id);
                if (!isAdmin)
                {
                    query = query.Where(r => r.Name != "admin");
                }
                if (!parameter.Deleted)
                {
                    query = query.Where(x => x.Deleted == false);
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

                var result = query.ProjectTo<RoleInterfaceModel>().ToList();
                if (result == null || result.Count == 0)
                    return null;

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<RepositoryResult<RoleInterfaceModel>> GetRoleByIdForInterfaceAsync(int roleId)
        {
            try
            {
                if (roleId <= 0)
                {
                    return new RepositoryResult<RoleInterfaceModel>(HttpStatusCode.BadRequest);
                }

                var role = await AppRoleManager.FindByIdAsync(roleId);
                if (role == null)
                {
                    return new RepositoryResult<RoleInterfaceModel>(HttpStatusCode.NotFound);
                }

                return new RepositoryResult<RoleInterfaceModel>(HttpStatusCode.OK, Mapper.Map<RoleInterfaceModel>(role));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region Internal
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
