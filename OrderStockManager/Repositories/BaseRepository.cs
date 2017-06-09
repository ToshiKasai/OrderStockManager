using Microsoft.AspNet.Identity.Owin;
using OrderStockManager.Infrastructure;
using OrderStockManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace OrderStockManager.Repositories
{
    public class BaseRepository : IDisposable
    {
        private ApplicationUserManager _AppUserManager = null;
        private ApplicationRoleManager _AppRoleManager = null;

        protected ApplicationUserManager AppUserManager
        {
            get
            {
                return _AppUserManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        protected ApplicationRoleManager AppRoleManager
        {
            get
            {
                return _AppRoleManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            }
        }

        protected int CountToPages(int count, int? countPerPage)
        {
            int result = 0;
            if (countPerPage.HasValue)
            {
                result = (int)(count / (int)countPerPage);
                if ((int)(result % (int)countPerPage) > 0)
                {
                    result += 1;
                }
            }
            return result;
        }

        public BaseRepository() { }
        
        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _AppUserManager = null;
                    _AppRoleManager = null;
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
