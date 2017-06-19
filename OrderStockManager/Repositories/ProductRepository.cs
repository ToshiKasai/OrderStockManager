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
using System.Threading.Tasks;
using System.Web;

namespace OrderStockManager.Repositories
{
    public class ProductRepository : BaseRepository, IProductRepository, IDisposable
    {
        public ProductRepository()
        {
            Mapper.Initialize(cfg =>
                cfg.CreateMap<ProductModel, ProductInterfaceModel>()
                    .ForMember(d => d.MakerCode, o => o.MapFrom(s => s.MakerModel.Code))
                    .ForMember(d => d.MakerName, o => o.MapFrom(s => s.MakerModel.Name))
            );
            // Mapper.AssertConfigurationIsValid();
        }

        public IEnumerable<ProductInterfaceModel> GetProductsForInterface(CustomParameterModel parameter)
        {
            try
            {
                using (DataContext dbContext = DataContext.Create())
                using (DbContextTransaction tx = dbContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    IQueryable<ProductModel> query = QueryProductModel(dbContext, parameter, true);
                    var result = query.ProjectTo<ProductInterfaceModel>().ToList();
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

        public RepositoryResult<ProductInterfaceModel> GetProductByIdForInterface(int productId)
        {
            try
            {
                if (productId <= 0)
                {
                    return new RepositoryResult<ProductInterfaceModel>(HttpStatusCode.BadRequest);
                }

                using (DataContext dbContext = DataContext.Create())
                {
                    var result = dbContext.ProductModels.Where(x => x.Id == productId).ProjectTo<ProductInterfaceModel>().SingleOrDefault();
                    if (result == null)
                    {
                        return new RepositoryResult<ProductInterfaceModel>(HttpStatusCode.NotFound);
                    }
                    return new RepositoryResult<ProductInterfaceModel>(HttpStatusCode.OK, result);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public RepositoryResult<ProductInterfaceModel> ModifyProduct(int productId, ProductInterfaceModel modifiedProduct)
        {
            try
            {
                if (productId <= 0)
                {
                    return new RepositoryResult<ProductInterfaceModel>(HttpStatusCode.BadRequest);
                }

                using (DataContext dbContext = DataContext.Create())
                using (DbContextTransaction tx = dbContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    modifiedProduct.Id = productId;
                    var product = dbContext.ProductModels.Where(m => m.Id == productId).SingleOrDefault();
                    if (product == null)
                    {
                        tx.Rollback();
                        return new RepositoryResult<ProductInterfaceModel>(HttpStatusCode.NotFound);
                    }

                    product.PaletteQuantity = modifiedProduct.PaletteQuantity;
                    product.CartonQuantity = modifiedProduct.CartonQuantity;
                    product.CaseHeight = modifiedProduct.CaseHeight;
                    product.CaseWidth = modifiedProduct.CaseWidth;
                    product.CaseDepth = modifiedProduct.CaseDepth;
                    product.CaseCapacity = modifiedProduct.CaseCapacity;
                    product.LeadTime = modifiedProduct.LeadTime;
                    product.OrderInterval = modifiedProduct.OrderInterval;
                    product.OldProductModelId = modifiedProduct.OldProductModelId;
                    product.Magnification = modifiedProduct.Magnification;
                    product.MinimumOrderQuantity = modifiedProduct.MinimumOrderQuantity;
                    product.Enabled = modifiedProduct.Enabled;
                    dbContext.Entry(product).State = EntityState.Modified;

                    if (dbContext.SaveChanges() == 0)
                    {
                        tx.Rollback();
                        return new RepositoryResult<ProductInterfaceModel>(HttpStatusCode.Conflict);
                    }
                    tx.Commit();
                    return new RepositoryResult<ProductInterfaceModel>(HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object GetProductsPages(CustomParameterModel parameter)
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
                    IQueryable<ProductModel> query = QueryProductModel(dbContext, parameter);
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

        #region Internal
        private IQueryable<ProductModel> QueryProductModel(DataContext dbContext, CustomParameterModel parameter, bool pageControl = false)
        {
            IQueryable<ProductModel> query = dbContext.ProductModels.OrderBy(x => x.Id);
            if (parameter.GroupId.HasValue)
            {
                query = dbContext.GroupProductModels.Where(gp => gp.Deleted == false).Where(gp => gp.GroupModelId == parameter.GroupId.GetValueOrDefault()).Select(gp => gp.ProductModel).OrderBy(p => p.Id);
            }
            else if (parameter.MakerId.HasValue)
            {
                query = dbContext.ProductModels.Where(p => p.MakerModelId == parameter.MakerId.GetValueOrDefault()).OrderBy(p => p.Id);
            }
            else
            {
                query = dbContext.ProductModels.OrderBy(p => p.Id);
            }

            if (!parameter.Deleted)
            {
                query = query.Where(p => p.Deleted == false);
            }
            if (parameter.Enabled)
            {
                query = query.Where(p => p.Enabled == true);
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
