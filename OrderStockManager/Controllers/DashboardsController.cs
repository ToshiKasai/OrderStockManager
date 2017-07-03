using OrderStockManager.Infrastructure;
using OrderStockManager.Models;
using OrderStockManager.Models.Parameters;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace OrderStockManager.Controllers
{
    [Authorize]
    [RoutePrefix("api/dashboards")]
    public class DashboardsController : BaseApiController
    {
        public DashboardsController()
        {
        }

        [HttpGet]
        public IHttpActionResult Get([FromUri]BaseParameterModel param)
        {
            try
            {
                using (DataContext dbContext = DataContext.Create())
                {
                    IQueryable<DashboardModel> query = dbContext.DashboardModels.OrderBy(x => x.Id);
                    if (!param.Deleted)
                    {
                        query = query.Where(x => x.Deleted == false);
                    }
                    if (param.Enabled)
                    {
                        query = query.Where(x => x.StartDateTime <= DateTime.Now);
                        query = query.Where(x => x.EndDateTime >= DateTime.Now);
                    }
                    var result = query.ToList();
                    //if (result == null || result.Count == 0)
                    //{
                    //    return NotFound();
                    //}
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult Get(int id)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }

        [HttpPost]
        [ValidationRequired(prefix = "value")]
        public IHttpActionResult Post([FromBody]DashboardModel value)
        {
            try
            {
                using (DataContext dbContext = DataContext.Create())
                using (DbContextTransaction tx = dbContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    dbContext.DashboardModels.Add(value);
                    if (dbContext.SaveChanges() == 0)
                    {
                        tx.Rollback();
                        return BadRequest();
                    }
                    tx.Commit();
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPut]
        [Route("{id}")]
        [ValidationRequired(prefix = "value")]
        public IHttpActionResult Put(int id, [FromBody]string value)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }

        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult Delete(int id)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }
    }
}
