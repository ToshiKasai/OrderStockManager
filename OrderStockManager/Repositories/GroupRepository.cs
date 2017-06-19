using AutoMapper;
using AutoMapper.QueryableExtensions;
using OrderStockManager.Infrastructure;
using OrderStockManager.Models;
using OrderStockManager.Models.Parameters;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;

namespace OrderStockManager.Repositories
{
    public class GroupRepository : BaseRepository, IGroupRepository, IDisposable
    {
        public GroupRepository()
        {
            Mapper.Initialize(cfg =>
                cfg.CreateMap<GroupModel, GroupInterfaceModel>()
                    .ForMember(d => d.MakerCode, o => o.MapFrom(s => s.MakerModel.Code))
                    .ForMember(d => d.MakerName, o => o.MapFrom(s => s.MakerModel.Name))
                    .ForMember(d => d.ContainerName, o => o.MapFrom(s => s.ContainerModel.Name))
            );
            // Mapper.AssertConfigurationIsValid();
        }

        public IEnumerable<GroupInterfaceModel> GetGroupsForInterface(CustomParameterModel parameter)
        {
            try
            {
                using (DataContext dbContext = DataContext.Create())
                {
                    IQueryable<GroupModel> query = QueryGroupModel(dbContext, parameter, true);
                    var result = query.ProjectTo<GroupInterfaceModel>().ToList();
                    if (result == null || result.Count == 0)
                    {
                        return null;
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public RepositoryResult<GroupInterfaceModel> GetGroupByIdForInterface(int groupId)
        {
            try
            {
                if (groupId <= 0)
                {
                    return new RepositoryResult<GroupInterfaceModel>(HttpStatusCode.BadRequest);
                }

                using (DataContext dbContext = DataContext.Create())
                {
                    var result = dbContext.GroupModels.Where(x => x.Id == groupId).ProjectTo<GroupInterfaceModel>().SingleOrDefault();
                    if (result == null)
                    {
                        return new RepositoryResult<GroupInterfaceModel>(HttpStatusCode.NotFound);
                    }

                    return new RepositoryResult<GroupInterfaceModel>(HttpStatusCode.OK, result);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public RepositoryResult<GroupInterfaceModel> GetGroupByCodeForInterface(string groupCode)
        {
            try
            {
                if (string.IsNullOrEmpty(groupCode))
                {
                    return new RepositoryResult<GroupInterfaceModel>(HttpStatusCode.BadRequest);
                }

                using (DataContext dbContext = DataContext.Create())
                {
                    var result = dbContext.GroupModels.Where(x => x.Code == groupCode).ProjectTo<GroupInterfaceModel>().SingleOrDefault();
                    if (result == null)
                    {
                        return new RepositoryResult<GroupInterfaceModel>(HttpStatusCode.NotFound);
                    }

                    return new RepositoryResult<GroupInterfaceModel>(HttpStatusCode.OK, result);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public RepositoryResult<GroupInterfaceModel> CreateGroup(GroupInterfaceModel createGroup)
        {
            try
            {
                using (DataContext dbContext = DataContext.Create())
                using (DbContextTransaction tx = dbContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    var group = dbContext.GroupModels.Where(m => m.Code == createGroup.Code).SingleOrDefault();
                    if (group != null)
                    {
                        tx.Rollback();
                        return new RepositoryResult<GroupInterfaceModel>(HttpStatusCode.Conflict);
                    }

                    group = new GroupModel();
                    CopyToGroupModel(group, createGroup);

                    dbContext.GroupModels.Add(group);
                    if (dbContext.SaveChanges() == 0)
                    {
                        tx.Rollback();
                        return new RepositoryResult<GroupInterfaceModel>(HttpStatusCode.BadRequest);
                    }
                    tx.Commit();
                    return new RepositoryResult<GroupInterfaceModel>(HttpStatusCode.Created);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public RepositoryResult<GroupInterfaceModel> ModifyGroup(int groupId, GroupInterfaceModel modifiedGroup)
        {
            try
            {
                if (groupId <= 0)
                {
                    return new RepositoryResult<GroupInterfaceModel>(HttpStatusCode.BadRequest);
                }

                using (DataContext dbContext = DataContext.Create())
                using (DbContextTransaction tx = dbContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    modifiedGroup.Id = groupId;
                    var group = dbContext.GroupModels.Where(m => m.Id == groupId).SingleOrDefault();
                    if (group == null)
                    {
                        tx.Rollback();
                        return new RepositoryResult<GroupInterfaceModel>(HttpStatusCode.NotFound);
                    }

                    CopyToGroupModel(group, modifiedGroup);
                    dbContext.Entry(group).State = EntityState.Modified;
                    if (dbContext.SaveChanges() == 0)
                    {
                        tx.Rollback();
                        return new RepositoryResult<GroupInterfaceModel>(HttpStatusCode.Conflict);
                    }
                    tx.Commit();
                    return new RepositoryResult<GroupInterfaceModel>(HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object GetGroupsPages(CustomParameterModel parameter)
        {
            try
            {
                if (parameter == null)
                {
                    return null;
                }
                if (!parameter.Limit.HasValue)
                {
                    return null;
                }
                else if (parameter.Limit.GetValueOrDefault() <= 0)
                {
                    return null;
                }

                using (DataContext dbContext = DataContext.Create())
                {
                    int maxcount = 0;
                    int maxpages = 0;
                    IQueryable<GroupModel> query = QueryGroupModel(dbContext, parameter);
                    maxcount = query.Count();
                    maxpages = CountToPages(maxcount, parameter.Limit.GetValueOrDefault());
                    return new { count = maxcount, pages = maxpages };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public RepositoryResult<IEnumerable<ProductInterfaceModel>> GroupModifyProducts(int groupId, List<ProductInterfaceModel> ProductList)
        {
            try
            {
                if (groupId <= 0)
                {
                    return new RepositoryResult<IEnumerable<ProductInterfaceModel>>(HttpStatusCode.BadRequest);
                }

                using (DataContext dbContext = DataContext.Create())
                {
                    var group = dbContext.GroupModels.Where(g => g.Id == groupId).SingleOrDefault();
                    if (group == null)
                    {
                        return new RepositoryResult<IEnumerable<ProductInterfaceModel>>(HttpStatusCode.NotFound);
                    }

                    dbContext.Database.ExecuteSqlCommand(RepositoryResource.IncrementResetGroupProduct);

                    using (DbContextTransaction tx = dbContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                    {
                        List<int> setProductList = ProductList.Select(m => m.Id).OrderBy(x => x).ToList();
                        IQueryable<GroupProductModel> groupProducts = dbContext.GroupProductModels.Where(x => x.GroupModelId == groupId).OrderBy(x => x.ProductModelId);
                        List<int> productIdList = dbContext.ProductModels.Where(x => x.MakerModelId == group.MakerModelId).Select(x => x.Id).OrderBy(x => x).ToList<int>();

                        foreach (int item in productIdList)
                        {
                            GroupProductModel check = groupProducts.Where(x => x.ProductModelId == item).FirstOrDefault();
                            if (check == null && setProductList.Contains(item))
                            {
                                check = new GroupProductModel();
                                check.GroupModelId = groupId;
                                check.ProductModelId = item;
                                dbContext.GroupProductModels.Add(check);
                            }
                            else if (check != null && setProductList.Contains(item) && check.Deleted == true)
                            {
                                check.Deleted = false;
                                dbContext.Entry(check).State = EntityState.Modified;
                            }
                            else if (check != null && !setProductList.Contains(item) && check.Deleted == false)
                            {
                                check.Deleted = true;
                                dbContext.Entry(check).State = EntityState.Modified;
                            }
                        }
                        dbContext.SaveChanges();
                        tx.Commit();
                    }
                    dbContext.Database.ExecuteSqlCommand(RepositoryResource.IncrementResetGroupProduct);

                    return new RepositoryResult<IEnumerable<ProductInterfaceModel>>(HttpStatusCode.NotFound, ProductList);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region Internal
        private IQueryable<GroupModel> QueryGroupModel(DataContext dbContext, CustomParameterModel parameter, bool pageControl = false)
            {
                IQueryable<GroupModel> query = dbContext.GroupModels.OrderBy(x => x.Id);
                if (!parameter.Deleted)
                {
                    query = query.Where(x => x.Deleted == false);
                }
                if (parameter.MakerId.HasValue)
                {
                    query = query.Where(x => x.MakerModelId == parameter.MakerId.GetValueOrDefault());
                }

                if (pageControl)
                {
                    if (parameter.Limit.HasValue && parameter.Limit.GetValueOrDefault() > 0)
                    {
                        if (parameter.Page.HasValue && parameter.Page.GetValueOrDefault() >= 0)
                        {
                            query = query.Skip(parameter.Limit.GetValueOrDefault() * parameter.Page.GetValueOrDefault()).Take(parameter.Limit.GetValueOrDefault());
                        }
                        else
                        {
                            query = query.Take(parameter.Limit.GetValueOrDefault());
                        }
                    }
                }
                return query;
            }

        private bool CopyToGroupModel(GroupModel group, GroupInterfaceModel model)
        {
            try
            {
                group.Code = model.Code;
                group.Name = model.Name;
                group.MakerModelId = model.MakerModelId;
                group.ContainerModelId = model.ContainerModelId;
                group.IsCapacity = model.IsCapacity;
                group.ContainerCapacityBt20Dry = model.ContainerCapacityBt20Dry;
                group.ContainerCapacityBt20Reefer = model.ContainerCapacityBt20Reefer;
                group.ContainerCapacityBt40Dry = model.ContainerCapacityBt40Dry;
                group.ContainerCapacityBt40Reefer = model.ContainerCapacityBt40Reefer;
                group.Deleted = model.Deleted;
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
