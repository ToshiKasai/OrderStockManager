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
        public MakerRepository()
        {
            Mapper.Initialize(cfg =>
                cfg.CreateMap<MakerModel, MakerInterfaceModel>()
            );
            // Mapper.AssertConfigurationIsValid();
        }

        public IEnumerable<MakerInterfaceModel> GetMakersForInterface(BaseParameterModel parameter)
        {
            try
            {
                using (DataContext dbContext = DataContext.Create())
                using (DbContextTransaction tx = dbContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    IQueryable<MakerModel> query = dbContext.MakerModels.OrderBy(x => x.Id);
                    if (!parameter.Deleted)
                        query = query.Where(x => x.Deleted == false);
                    if (parameter.Enabled)
                        query = query.Where(x => x.Enabled == true);

                    if (parameter.Limit.HasValue)
                    {
                        if (parameter.Page.HasValue)
                            query = query.Skip((int)parameter.Limit * (int)parameter.Page).Take((int)parameter.Limit);
                        else
                            query = query.Take((int)parameter.Limit);
                    }
                    var result = query.ProjectTo<MakerInterfaceModel>().ToList();
                    if (result == null || result.Count == 0)
                        return null;

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
                using (DbContextTransaction tx = dbContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
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
                    IQueryable<MakerModel> query;

                    query = dbContext.MakerModels.OrderBy(x => x.Id);
                    if (!parameter.Deleted)
                        query = query.Where(x => x.Deleted == false);
                    if (parameter.Enabled)
                        query = query.Where(x => x.Enabled == true);
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
