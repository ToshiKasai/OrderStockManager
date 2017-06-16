using Microsoft.Practices.Unity;
using OrderStockManager.Controllers;
using OrderStockManager.Infrastructure;
using OrderStockManager.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace OrderStockManager
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // ContainerControlledLifetimeManager : singleton instance with dispose
            // HierarchicalLifetimeManager : singleton instance per container with dispose
            // TransientLifetimeManager : empty manager, always returns new object by resolve, no dispose!
            // PerRequestLifetimeManager(Unity.MVC) : singleton instance per http request with dispose
            // ExternallyControlledLifetimeManager : code must handle lifetime management
            // PerResolveLifetimeManager : like TransientLifetimeManager expect when in same object graph
            // PerThreadLifetimeManager : A LifetimeManager that holds the instances given to it, keeping one instance per thread.
            var container = new UnityContainer();
            container.RegisterType<IUserRepository, UserRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IRoleRepository, RoleRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IMakerRepository, MakerRepository>(new HierarchicalLifetimeManager());
            config.DependencyResolver = new UnityResolver(container);

            // Web API ルート
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
