using OrderStockManager.Models;
using OrderStockManager.Models.Parameters;
using OrderStockManager.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;

namespace OrderStockManager.Controllers
{
    [Authorize]
    [RoutePrefix("api/groups")]
    public class GroupsController : BaseApiController
    {
        private IGroupRepository repository = null;
        private IProductRepository productRepository = null;

        public GroupsController(IGroupRepository repository, IProductRepository productRepository)
        {
            this.repository = repository;
            this.productRepository = productRepository;
        }

        #region グループ情報
        [HttpGet]
        public IHttpActionResult Get([FromUri]CustomParameterModel param)
        {
            try
            {
                var result = repository.GetGroupsForInterface(param);
                //if (result == null)
                //    return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("{id}", Name = "GetGroupsById")]
        public IHttpActionResult Get(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest();
                }

                var result = repository.GetGroupByIdForInterface(id);
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
        public IHttpActionResult Post([FromBody]GroupInterfaceModel value)
        {
            try
            {
                var result = repository.CreateGroup(value);
                if (result.Code != HttpStatusCode.Created)
                {
                    return Content(result.Code, ModelState.GetErrorsDelprefix("value"));
                }

                result = repository.GetGroupByCodeForInterface(value.Code);
                var _UrlHelper = new UrlHelper(this.Request);
                var Url = _UrlHelper.Link("GetGroupsById", new { id = result.resultData.Id });
                return Created(Url, result.resultData);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [ValidationRequired(prefix = "value")]
        [Route("{id}")]
        public IHttpActionResult Put(int id, [FromBody]GroupInterfaceModel value)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest();
                }

                var result = repository.ModifyGroup(id, value);
                if (result.Code != HttpStatusCode.OK)
                {
                    return Content(result.Code, ModelState.GetErrorsDelprefix("value"));
                }
                var model = repository.GetGroupByIdForInterface(id);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }

        [HttpGet]
        [Route("pages")]
        public IHttpActionResult GetGroupMaxPages([FromUri]CustomParameterModel param)
        {
            try
            {
                var result = repository.GetGroupsPages(param);
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

        #region グループ商品情報
        [HttpGet]
        [Route("{id}/products")]
        public IHttpActionResult GetProductList(int id, [FromUri]CustomParameterModel param)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest();
                }

                var group = repository.GetGroupByIdForInterface(id);
                if (group.Code != HttpStatusCode.OK)
                {
                    return Content(group.Code, group.message);
                }

                param.GroupId = id;
                var result = productRepository.GetProductsForInterface(param);
                //if (result == null)
                //{
                //    return NotFound();
                //}
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("{id}/products")]
        [ValidationRequired(prefix = "ProductList")]
        public IHttpActionResult SetProductList(int id, [FromBody]List<ProductInterfaceModel> ProductList)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest();
                }

                var result = repository.GroupModifyProducts(id, ProductList);
                if (result.Code != HttpStatusCode.OK)
                {
                    return Content(result.Code, result.message);
                }

                CustomParameterModel param = new CustomParameterModel();
                param.GroupId = id;
                var products = productRepository.GetProductsForInterface(param);
                if (products == null)
                {
                    return Ok();
                }
                return Ok(products);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        #endregion
    }
}
