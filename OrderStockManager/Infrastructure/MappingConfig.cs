using AutoMapper;
using OrderStockManager.Models;
using OrderStockManager.Models.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderStockManager.Infrastructure
{
    public class MappingConfig
    {
        public static void MappingSetting()
        {
            Mapper.Initialize(cfg =>
            {
                // グループ
                cfg.CreateMap<GroupModel, GroupInterfaceModel>()
                    .ForMember(d => d.MakerCode, o => o.MapFrom(s => s.MakerModel.Code))
                    .ForMember(d => d.MakerName, o => o.MapFrom(s => s.MakerModel.Name))
                    .ForMember(d => d.ContainerName, o => o.MapFrom(s => s.ContainerModel.Name));
                // メーカー
                cfg.CreateMap<MakerModel, MakerInterfaceModel>();
                // プロダクト
                cfg.CreateMap<ProductModel, ProductInterfaceModel>()
                    .ForMember(d => d.MakerCode, o => o.MapFrom(s => s.MakerModel.Code))
                    .ForMember(d => d.MakerName, o => o.MapFrom(s => s.MakerModel.Name));
                // ロール
                cfg.CreateMap<RoleModel, RoleInterfaceModel>();
                //  ユーザー
                cfg.CreateMap<UserModel, UserInterfaceModel>()
                    .ForMember(d => d.NewExpiration, o => o.Ignore())
                    .ForMember(d => d.NewPassword, o => o.Ignore());
                // 販売動向
                cfg.CreateMap<SalesTrendModel, SalesTrendInterfaceModel>()
                    .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                    .ForMember(d => d.Product_id, o => o.MapFrom(s => s.ProductModelId))
                    .ForMember(d => d.Detail_date, o => o.MapFrom(s => s.TargetDate))
                    .ForMember(d => d.Quantity, o => o.MapFrom(s => s.Sales))
                    .ForMember(d => d.Comments, o => o.MapFrom(s => s.Comments))
                    .ForMember(d => d.User_id, o => o.MapFrom(s => s.UserModelId))
                    .ForMember(d => d.User_name, o => o.MapFrom(s => s.UserModel.Name));
                // コンテナ
                cfg.CreateMap<ContainerModel, ContainerInterfaceModel>();
            }
            );

            Mapper.AssertConfigurationIsValid();
        }

        public MappingConfig()
        {
            MappingSetting();
        }
    }
}
