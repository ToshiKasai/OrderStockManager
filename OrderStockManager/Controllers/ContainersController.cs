using AutoMapper;
using AutoMapper.QueryableExtensions;
using OrderStockManager.Infrastructure;
using OrderStockManager.Models;
using OrderStockManager.Models.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OrderStockManager.Controllers
{
    [Authorize]
    [RoutePrefix("api/containers")]
    public class ContainersController : BaseApiController
    {
        public ContainersController()
        {
            Mapper.Initialize(cfg =>
                cfg.CreateMap<ContainerModel, ContainerInterfaceModel>()
            );
        }

        [HttpGet]
        public IHttpActionResult Get([FromUri]BaseParameterModel param)
        {
            try
            {
                using (DataContext dbContext = DataContext.Create())
                {
                    IQueryable<ContainerModel> query = dbContext.ContainerModels.OrderBy(x => x.Id);
                    if (!param.Deleted)
                    {
                        query = query.Where(x => x.Deleted == false);
                    }

                    if (param.Limit.HasValue)
                    {
                        if (param.Page.HasValue)
                        {
                            query = query.Skip(param.Limit.GetValueOrDefault() * param.Page.GetValueOrDefault()).Take(param.Limit.GetValueOrDefault());
                        }
                        else
                        {
                            query = query.Take(param.Limit.GetValueOrDefault());
                        }
                    }
                    var result = query.ProjectTo<ContainerInterfaceModel>().ToList();
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
        public IHttpActionResult Post([FromBody]string value)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }

        [HttpPut]
        [Route("{id}")]
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
