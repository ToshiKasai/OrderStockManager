using OrderStockManager.Models;
using OrderStockManager.Models.Parameters;
using OrderStockManager.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OrderStockManager.Controllers
{
    [Authorize]
    [RoutePrefix("api/makers")]
    public class MakersController : BaseApiController
    {
        private IMakerRepository repository = null;

        public MakersController(IMakerRepository repository)
        {
            this.repository = repository;
        }

        #region メーカー情報
        [HttpGet]
        public IHttpActionResult Get([FromUri]BaseParameterModel param)
        {
            try
            {
                var result = repository.GetMakersForInterface(param);
                if (result == null)
                    return NotFound();
                return Ok(result);
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
            try
            {
                if (id <= 0)
                {
                    return BadRequest();
                }

                var result = repository.GetMakerByIdForInterface(id);
                if (result.Code != HttpStatusCode.OK)
                {
                    return Content(result.Code, result.message);
                }
                return Ok(result.resultData);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [ValidationRequired(prefix = "value")]
        public IHttpActionResult Post([FromBody]MakerInterfaceModel value)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }

        [HttpPut]
        [ValidationRequired(prefix = "value")]
        [Route("{id}")]
        public IHttpActionResult Put(int id, [FromBody]MakerInterfaceModel value)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest();
                }

                var result = repository.ModifyMaker(id, value);
                if (result.Code != HttpStatusCode.OK)
                {
                    return Content(result.Code, ModelState.GetErrorsDelprefix("value"));
                }
                var model = repository.GetMakerByIdForInterface(id);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult Delete(int id)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }

        [HttpGet]
        [Route("pages")]
        public IHttpActionResult GetMakerMaxPages([FromUri]BaseParameterModel param)
        {
            try
            {
                var result = repository.GetMakersPages(param);
                if (result == null)
                {
                    return BadRequest();
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        #endregion

#if false
        #region グループ情報
        [HttpGet]
        [Route("api/Makers/{id}/Groups")]
        public IHttpActionResult GetGroupList(int id, [FromUri]BaseApiParameterModel param)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(Messages.ApiIllegalParameter);

                List<GroupApiModel> result;
                IQueryable<GroupModel> query;
                query = dbContext.GroupModels.Where(g => g.MakerModelId == id);
                if (!param.Deleted)
                    query = query.Where(g => g.Deleted == false);
                result = query.OrderBy(g => g.Id).ProjectTo<GroupApiModel>().ToList();

                //if (result == null || result.Count == 0)
                //    return NotFound();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        #endregion
#endif
    }
}
