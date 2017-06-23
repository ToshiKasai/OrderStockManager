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
        private IGroupRepository groupRepository = null;

        public MakersController(IMakerRepository repository, IGroupRepository groupRepository)
        {
            this.repository = repository;
            this.groupRepository = groupRepository;
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
                return Ok(model.resultData);
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

        #region グループ情報
        [HttpGet]
        [Route("{id}/groups")]
        public IHttpActionResult GetGroupList(int id, [FromUri]BaseParameterModel param)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest();
                }

                var maker = repository.GetMakerByIdForInterface(id);
                if (maker.Code != HttpStatusCode.OK)
                {
                    return BadRequest();
                }

                var parameter = new CustomParameterModel();
                parameter.MakerId = id;
                parameter.Deleted = param.Deleted;
                var result = groupRepository.GetGroupsForInterface(parameter);

                if (result == null || result.Count() == 0)
                    return NotFound();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        #endregion
    }
}
