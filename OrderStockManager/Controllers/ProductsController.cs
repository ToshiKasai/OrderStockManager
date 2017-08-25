using OrderStockManager.Models;
using OrderStockManager.Models.Parameters;
using OrderStockManager.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OrderStockManager.Controllers
{
    [Authorize]
    [RoutePrefix("api/products")]
    public class ProductsController : BaseApiController
    {
        private IProductRepository repository = null;

        public ProductsController(IProductRepository repository)
        {
            this.repository = repository;
        }

        #region 商品情報
        [HttpGet]
        public IHttpActionResult Get([FromUri]CustomParameterModel param)
        {
            try
            {
                var result = repository.GetProductsForInterface(param);
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
        [Route("{id}")]
        public IHttpActionResult Get(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest();
                }

                var result = repository.GetProductByIdForInterface(id);
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
        public IHttpActionResult Post([FromBody]ProductInterfaceModel value)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }

        [HttpPut]
        [ValidationRequired(prefix = "value")]
        [Route("{id}")]
        public IHttpActionResult Put(int id, [FromBody]ProductInterfaceModel value)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest();
                }

                var result = repository.ModifyProduct(id, value);
                if (result.Code != HttpStatusCode.OK)
                {
                    return Content(result.Code, ModelState.GetErrorsDelprefix("value"));
                }
                var model = repository.GetProductByIdForInterface(id);
                return Ok(model.resultData);
                // return Ok();
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
        public IHttpActionResult GetProductMaxPages([FromUri]CustomParameterModel param)
        {
            try
            {
                var result = repository.GetProductsPages(param);
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

        #region 商品別データ
        [HttpGet]
        [Route("{id}/salesviews/{year}")]
        public IHttpActionResult GetSalesViews(int id, int year, [FromUri]BaseParameterModel param)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }

        [HttpPost]
        [Route("{id}/salesviews/")]
        [ValidationRequired(prefix = "item")]
        public IHttpActionResult SetSalesViews([FromBody]SalesViewInterfaceModel item)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }
        #endregion
    }
}
