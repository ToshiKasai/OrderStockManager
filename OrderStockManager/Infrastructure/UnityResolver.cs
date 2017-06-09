using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Dependencies;

namespace OrderStockManager.Infrastructure
{
    public class UnityResolver : IDependencyResolver
    {
        protected IUnityContainer container;

        public UnityResolver(IUnityContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            this.container = container;
        }

        // IDependencyResolver
        public IDependencyScope BeginScope()
        {
            var child = container.CreateChildContainer();
            return new UnityResolver(child);
        }

        // IDependencyScope
        public object GetService(Type serviceType)
        {
            try
            {
                return container.Resolve(serviceType);
            }
            catch (ResolutionFailedException)
            {
#if UNITY_DEBUG
                System.Diagnostics.Debug.WriteLine("GetService : " + serviceType.FullName);
#endif
                return null;
            }
        }

        // IDependencyScope
        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return container.ResolveAll(serviceType);
            }
            catch (ResolutionFailedException)
            {
#if UNITY_DEBUG
                System.Diagnostics.Debug.WriteLine("GetService : " + serviceType.FullName);
#endif
                return new List<object>();
            }
        }

        #region IDisposable Support
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            container.Dispose();
        }
        #endregion
    }
}
