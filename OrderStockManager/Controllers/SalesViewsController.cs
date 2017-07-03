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

        public SalesViewsController(ISalesViewService service)
        {
            this.service = service;
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

        [HttpPost]
        [Route("upload")]
        public async Task<IHttpActionResult> UploadSalesViewsAsync()
        {
            // multipart/form-data 以外は 415 を返す
            if (!Request.Content.IsMimeMultipartContent())
            {
                return StatusCode(HttpStatusCode.UnsupportedMediaType);
            }

            // マルチパートデータを一時的に保存する場所を指定
            var rootPath = Path.GetTempPath();
            var serverPath = System.Web.Hosting.HostingEnvironment.MapPath(rootPath);
            var provider = new MultipartFormDataStreamProvider(rootPath);

            // 実際に読み込む
            await Request.Content.ReadAsMultipartAsync(provider);

            // ファイルデータを読む
            foreach (var file in provider.FileData)
            {
                // ファイル情報を取得
                var fileInfo = new FileInfo(file.LocalFileName);
            }

            return Ok();
        }

        [HttpPost]
        [Route("up")]
        public async Task<IHttpActionResult> UpSalesViewsAsync()
        {
            var provider = await Request.Content.ReadAsMultipartAsync();
            var fileContent = provider.Contents.First(x => x.Headers.ContentDisposition.Name == JsonConvert.SerializeObject("buffer"));
            var buffer = await fileContent.ReadAsByteArrayAsync();
            var json = await provider.Contents.First(x => x.Headers.ContentDisposition.Name == JsonConvert.SerializeObject("fileName")).ReadAsStringAsync();
            var fileName = JsonConvert.DeserializeObject<string>(json);

            // var blob = this.container.GetBlockBlobReference(fileName);
            // blob.Properties.ContentType = fileContent.Headers.ContentType.MediaType;
            // await blob.UploadFromByteArrayAsync(buffer, 0, buffer.Length);

            return Ok();
        }
    }
}
