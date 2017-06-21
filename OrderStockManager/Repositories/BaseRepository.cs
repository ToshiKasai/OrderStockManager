using AutoMapper;
using Microsoft.AspNet.Identity.Owin;
using OrderStockManager.Infrastructure;
using OrderStockManager.Models;
using OrderStockManager.Models.Parameters;
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

        public BaseRepository()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<GroupModel, GroupInterfaceModel>()
                    .ForMember(d => d.MakerCode, o => o.MapFrom(s => s.MakerModel.Code))
                    .ForMember(d => d.MakerName, o => o.MapFrom(s => s.MakerModel.Name))
                    .ForMember(d => d.ContainerName, o => o.MapFrom(s => s.ContainerModel.Name));
                cfg.CreateMap<MakerModel, MakerInterfaceModel>();
                cfg.CreateMap<ProductModel, ProductInterfaceModel>()
                    .ForMember(d => d.MakerCode, o => o.MapFrom(s => s.MakerModel.Code))
                    .ForMember(d => d.MakerName, o => o.MapFrom(s => s.MakerModel.Name));
                cfg.CreateMap<RoleModel, RoleInterfaceModel>();
                cfg.CreateMap<UserModel, UserInterfaceModel>()
                    .ForMember(d => d.NewExpiration, o => o.Ignore())
                    .ForMember(d => d.NewPassword, o => o.Ignore());
            }
            );
            Mapper.AssertConfigurationIsValid();
        }

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
