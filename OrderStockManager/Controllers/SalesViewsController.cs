using Newtonsoft.Json;
using OrderStockManager.Infrastructure;
using OrderStockManager.Models.Parameters;
using OrderStockManager.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace OrderStockManager.Controllers
{
    [Authorize]
    [RoutePrefix("api/salesviews")]
    public class SalesViewsController : BaseApiController
    {
        private ISalesViewService service = null;
        private ISalesViewExcelService excelService = null;

        public SalesViewsController(ISalesViewService service, ISalesViewExcelService excelService)
        {
            this.service = service;
            this.excelService = excelService;
        }

        [HttpGet]
        public IHttpActionResult Get([FromUri]CustomParameterModel param)
        {
            try
            {
                var result = service.GetSalesViewsForInterface(param);
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

        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult Get(int id, [FromUri]CustomParameterModel param)
        {
            try
            {
                var result = service.GetSalesViewByIdForInterface(id, param);
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
        public IHttpActionResult Post([FromBody]SalesViewInterfaceModel value)
        {
            try
            {
                var result = service.SetSalesView(value);

                if (result.Code != HttpStatusCode.OK)
                {
                    return Content(result.Code, result.message);
                }

                CustomParameterModel param = new CustomParameterModel();
                param.Year = value.SalesList.Select(x => x.detail_date).Min().Year;
                param.Year += 1;

                result = service.GetSalesViewByIdForInterface(value.Product.Id, param);
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

        [HttpPut]
        [ValidationRequired(prefix = "value")]
        [Route("{id}")]
        public IHttpActionResult Put(int? id, [FromBody]SalesViewInterfaceModel value)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }

        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }

        [HttpGet]
        [Route("{id}/current")]
        public IHttpActionResult GetCurrent(int id, [FromUri]CustomParameterModel param)
        {
            var result = service.GetCurrentStocks(id, param);
            if (result == null)
            {
                return BadRequest();
            }
            return Ok(result);
        }

        #region 販売動向
        [HttpGet]
        [Route("{id}/trends")]
        public IHttpActionResult GetSalesTrend(int id, [FromUri]CustomParameterModel param)
        {
            var result = service.GetSalesTrandsForInterface(id, param);
            //if (result == null)
            //{
            //    return BadRequest();
            //}
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}/trends/{tid}")]
        public IHttpActionResult GetSalesTrend(int id, int tid)
        {
            try
            {
                var trends = service.GetSalesTrandsByIdForInterface(id, tid);
                if (trends.Code == HttpStatusCode.OK)
                {
                    return Content(trends.Code, trends.message);
                }
                return Ok(trends.resultData);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [HttpPost]
        [ValidationRequired(prefix = "model")]
        [Route("{id}/trends")]
        public IHttpActionResult PostSalesTrend(int id, [FromBody]SalesTrendInterfaceModel model)
        {
            if (model == null || model.Id != 0)
            {
                return BadRequest();
            }

            var result = service.CreateSalesTrend(id, model);
            if (result.Code == HttpStatusCode.OK)
            {
                return Content(result.Code, result.message);
            }
            return Ok(result.resultData);
        }

        [HttpPut]
        [ValidationRequired(prefix = "model")]
        [Route("{id}/trends/{tid}")]
        public IHttpActionResult PutSalesTrend(int id, int tid, [FromBody]SalesTrendInterfaceModel model)
        {
            if (tid == 0 || model == null || model.Id == 0)
            {
                return BadRequest();
            }

            var result = service.ModifySalesTrend(id, tid, model);
            if (result.Code == HttpStatusCode.OK)
            {
                return Content(result.Code, result.message);
            }
            return Ok(result.resultData);
        }

        [HttpDelete]
        [Route("{id}/trends/{tid}")]
        public IHttpActionResult DeleteSalesTrend(int id, int tid)
        {
            if (tid == 0)
            {
                return BadRequest();
            }

            var result = service.DeleteSalesTrend(id, tid);
            if (result.Code == HttpStatusCode.OK)
            {
                return Content(result.Code, result.message);
            }
            return Content(HttpStatusCode.NoContent, "");
        }
        #endregion

        [HttpGet]
        [Route("download")]
        public HttpResponseMessage DownloadSalesViews([FromUri]CustomParameterModel param)
        {
            var exceldata = excelService.CreateXlsxOneSheetBySalesView(param);
            return ByteResponse(exceldata);
        }

        [HttpPost]
        [Route("upload")]
        public async Task<IHttpActionResult> UploadSalesViewsAsync()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                return StatusCode(HttpStatusCode.UnsupportedMediaType);
            }

            var rootPath = Path.GetTempPath();
            var provider = new MultipartFormDataStreamProvider(rootPath);

            await Request.Content.ReadAsMultipartAsync(provider);
            foreach (var file in provider.FileData)
            {
                var fileInfo = new FileInfo(file.LocalFileName);
                excelService.ReadXlsxToSalesView(fileInfo);
            }
            return Ok();
        }
    }
}
