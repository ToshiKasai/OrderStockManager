using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using OrderStockManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderStockManager.Infrastructure
{
    public class ApplicationRoleManager : RoleManager<RoleModel, int>
    {
        public ApplicationRoleManager(IRoleStore<RoleModel, int> roleStore)
            : base(roleStore)
        {
        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            var appRoleManager = new ApplicationRoleManager(new RoleStore(context.Get<DataContext>()));
            return appRoleManager;
        }
    }
}