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
    public class MakerRepository : BaseRepository, IMakerRepository, IDisposable
    {
        public MakerRepository() : base()
        {
        }

        public IEnumerable<MakerInterfaceModel> GetMakersForInterface(BaseParameterModel parameter)
        {
            try
            {
                using (DataContext dbContext = DataContext.Create())
                {
                    IQueryable<MakerModel> query = QueryMakerModel(dbContext, parameter, true);
                    var result = query.ProjectTo<MakerInterfaceModel>().ToList();
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

        public RepositoryResult<MakerInterfaceModel> GetMakerByIdForInterface(int makerId)
        {
            try
            {
                if (makerId <= 0)
                {
                    return new RepositoryResult<MakerInterfaceModel>(HttpStatusCode.BadRequest);
                }

                using (DataContext dbContext = DataContext.Create())
                {
                    var result = dbContext.MakerModels.Where(x => x.Id == makerId).ProjectTo<MakerInterfaceModel>().SingleOrDefault();
                    if (result == null)
                    {
                        return new RepositoryResult<MakerInterfaceModel>(HttpStatusCode.NotFound);
                    }

                    return new RepositoryResult<MakerInterfaceModel>(HttpStatusCode.OK, result);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public RepositoryResult<MakerInterfaceModel> ModifyMaker(int makerId, MakerInterfaceModel modifiedMaker)
        {
            try
            {
                if (makerId <= 0)
                {
                    return new RepositoryResult<MakerInterfaceModel>(HttpStatusCode.BadRequest);
                }

                using (DataContext dbContext = DataContext.Create())
                using (DbContextTransaction tx = dbContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    modifiedMaker.Id = makerId;
                    var maker = dbContext.MakerModels.Where(m => m.Id == makerId).SingleOrDefault();
                    if (maker == null)
                    {
                        tx.Rollback();
                        return new RepositoryResult<MakerInterfaceModel>(HttpStatusCode.NotFound);
                    }

                    maker.Enabled = modifiedMaker.Enabled;
                    dbContext.Entry(maker).State = EntityState.Modified;
                    if (dbContext.SaveChanges() == 0)
                    {
                        tx.Rollback();
                        return new RepositoryResult<MakerInterfaceModel>(HttpStatusCode.Conflict);
                    }
                    tx.Commit();
                    return new RepositoryResult<MakerInterfaceModel>(HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object GetMakersPages(BaseParameterModel parameter)
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
                    IQueryable<MakerModel> query = QueryMakerModel(dbContext, parameter);
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

        public IEnumerable<MakerInterfaceModel> GetMakersByUserIdForInterface(int userId)
        {
            try
            {
                using (DataContext dbContext = DataContext.Create())
                {
                    var result = dbContext.UserMakerModels.Where(um => um.UserModelId == userId).Where(um => um.Deleted == false).Select(um => um.MakerModel).OrderBy(m => m.Id).ProjectTo<MakerInterfaceModel>().ToList();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        #region Internal
        private IQueryable<MakerModel> QueryMakerModel(DataContext dbContext, BaseParameterModel parameter, bool pageControl = false)
        {
            IQueryable<MakerModel> query = dbContext.MakerModels.OrderBy(x => x.Id);
            if (!parameter.Deleted)
            {
                query = query.Where(x => x.Deleted == false);
            }
            if (parameter.Enabled)
            {
                query = query.Where(x => x.Enabled == true);
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
