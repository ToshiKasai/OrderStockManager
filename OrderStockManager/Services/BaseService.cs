using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using OrderStockManager.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderStockManager.Services
{
    public class BaseService : MappingConfig, IDisposable
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

        protected IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().Authentication;
            }
        }

        protected int CountToPages(int count, int countPerPage)
        {
            int result = 0;
            if (count > 0 || countPerPage > 0)
                result = (int)(count / countPerPage);
            if ((int)(result % countPerPage) > 0)
            {
                result += 1;
            }
            return result;
        }

        public BaseService() { }

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
