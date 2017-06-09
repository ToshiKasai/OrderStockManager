using Microsoft.AspNet.Identity;
using OrderStockManager.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace OrderStockManager.Infrastructure
{
    public class RoleStore : IRoleStore<RoleModel, int>, IQueryableRoleStore<RoleModel, int>
    {
        #region Internal
        private DataContext db;
        public RoleStore(DataContext applicationDbContext)
        {
            db = applicationDbContext;
        }
        private void CheckDisposed(RoleModel role)
        {
            this.ThrowIfDisposed();
            if (role == null)
                throw new ArgumentNullException("role");
        }
        private void ThrowIfDisposed()
        {
            if (this.disposedValue)
                throw new ObjectDisposedException(this.GetType().Name);
        }

        #endregion

        #region IRoleStore
        Task IRoleStore<RoleModel, int>.CreateAsync(RoleModel role)
        {
            CheckDisposed(role);
            db.RoleModels.Add(role);
            db.SaveChanges();
            return Task.FromResult(role);
        }

        Task IRoleStore<RoleModel, int>.DeleteAsync(RoleModel role)
        {
            CheckDisposed(role);
            role.Deleted = true;
            return Task.FromResult(default(object));
        }

        Task<RoleModel> IRoleStore<RoleModel, int>.FindByIdAsync(int roleId)
        {
            return Task.FromResult(db.RoleModels.Where(x => x.Id == roleId).SingleOrDefault());
        }

        Task<RoleModel> IRoleStore<RoleModel, int>.FindByNameAsync(string roleName)
        {
            return Task.FromResult(db.RoleModels.Where(x => x.Name == roleName).SingleOrDefault());
        }

        Task IRoleStore<RoleModel, int>.UpdateAsync(RoleModel role)
        {
            CheckDisposed(role);
            db.Entry(role).State = EntityState.Modified;
            db.SaveChanges();
            return Task.FromResult(default(object));
        }
        #endregion

        IQueryable<RoleModel> IQueryableRoleStore<RoleModel, int>.Roles
        {
            get
            {
                return db.RoleModels;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
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