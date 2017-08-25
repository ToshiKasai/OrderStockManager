using AutoMapper;
using AutoMapper.QueryableExtensions;
using OrderStockManager.Infrastructure;
using OrderStockManager.Models;
using OrderStockManager.Models.Parameters;
using OrderStockManager.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;

namespace OrderStockManager.Services
{
    public class SalesViewService : BaseService, ISalesViewService, IDisposable
    {
        // private IProductRepository productRepository = null;
        enum SalesViewMode
        {
            NONE,
            YEAR,
            FromTo
        }

        public SalesViewService()
        {
        }

        public RepositoryResult<IEnumerable<SalesViewInterfaceModel>> GetSalesViewsForInterface(CustomParameterModel parameter, int months = 12)
        {
            try
            {
                SalesViewMode mode = SalesViewMode.NONE;
                if (parameter.GroupId.HasValue == false && parameter.MakerId.HasValue == false)
                {
                    return new RepositoryResult<IEnumerable<SalesViewInterfaceModel>>(HttpStatusCode.BadRequest);
                }
                if (parameter.Year.HasValue)
                {
                    mode = SalesViewMode.YEAR;
                }
                if(parameter.Begin.HasValue && parameter.End.HasValue)
                {
                    mode = SalesViewMode.FromTo;
                }
                if (mode == SalesViewMode.NONE)
                {
                    return new RepositoryResult<IEnumerable<SalesViewInterfaceModel>>(HttpStatusCode.BadRequest);
                }

                using (DataContext dbContext = DataContext.Create())
                using(var productRepository = new ProductRepository())
                {
                    var products = productRepository.GetProductsForInterface(parameter);
                    var productIds = products.Select(p => p.Id).ToList();

                    ICollection<OfficeModel> office = dbContext.OfficeModels.Where(o => o.Deleted == false).OrderBy(o => o.Code).ToList();

                    string productList = string.Join(",", productIds);
                    string sql = ServiceResource.SelectSalesViews.Replace("@p2", productList);

                    // 全体の販売実績データ
                    List<SalesInterfaceModel> salesList;
                    DateTime startDate = DateTime.Now;
                    DateTime endDate = DateTime.Now;
                    if (mode == SalesViewMode.YEAR)
                    {
                        // 在庫予測計算用に１ヶ月多めに取得（貿易のみ）
                        startDate = DateTime.Parse((parameter.Year - 1).ToString() + "/9/1");
                        endDate = startDate.AddMonths(months + 1);
                    }
                    else if (mode == SalesViewMode.FromTo)
                    {
                        startDate = parameter.Begin.GetValueOrDefault();
                        endDate = startDate.AddMonths(months);
                    }
                    salesList = dbContext.Database.SqlQuery<SalesInterfaceModel>(sql, startDate, endDate).ToList<SalesInterfaceModel>();

                    // 事業所別
                    List<SalesOfficeInterfaceModel> salesOfficeList;
                    sql = ServiceResource.SelectOfficesSalesViews.Replace("@p2", productList);
                    salesOfficeList = dbContext.Database.SqlQuery<SalesOfficeInterfaceModel>(sql, startDate.AddYears(-1), endDate).ToList<SalesOfficeInterfaceModel>();

                    // 返却用の準備
                    var resultData = new List<SalesViewInterfaceModel>();
                    foreach (var product in products)
                    {
                        // 在庫予測計算用に１ヶ月多めに取得（貿易のみ）
                        // DateTime check = startDate.AddMonths(-1);
                        DateTime check = startDate;
                        SalesViewInterfaceModel addModel = new SalesViewInterfaceModel();
                        addModel.Product = product;
                        addModel.SalesList = new List<SalesInterfaceModel>();
                        addModel.OfficeSales = new List<ICollection<SalesOfficeInterfaceModel>>();

                        for (; check <= endDate; check = check.AddMonths(1))
                        {
                            var work = salesList.Where(x => x.product_id == addModel.Product.Id).Where(x => x.detail_date == check).SingleOrDefault();
                            if (work == null)
                            {
                                var tempModel = new SalesInterfaceModel();
                                tempModel.product_id = addModel.Product.Id;
                                tempModel.detail_date = check;
                                addModel.SalesList.Add(tempModel);
                            }
                            else
                            {
                                addModel.SalesList.Add(work);
                            }
                        }

                        foreach (var ofs in office)
                        {
                            var workOffice = new List<SalesOfficeInterfaceModel>();
                            check = startDate;
                            for (; check <= endDate; check = check.AddMonths(1))
                            {
                                var ofsData = salesOfficeList.Where(x => x.product_id == addModel.Product.Id).Where(x => x.detail_date == check).Where(x => x.office_id == ofs.Id).SingleOrDefault();
                                if (ofsData == null)
                                {
                                    var tempModel = new SalesOfficeInterfaceModel();
                                    tempModel.product_id = product.Id;
                                    tempModel.detail_date = check;
                                    tempModel.office_id = ofs.Id;
                                    tempModel.office_name = ofs.Name;
                                    workOffice.Add(tempModel);
                                }
                                else
                                {
                                    ofsData.office_id = ofs.Id;
                                    ofsData.office_name = ofs.Name;
                                    workOffice.Add(ofsData);
                                }
                            }
                            addModel.OfficeSales.Add(workOffice);
                        }
                        resultData.Add(addModel);
                    }
                    return new RepositoryResult<IEnumerable<SalesViewInterfaceModel>>(HttpStatusCode.OK, resultData);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public RepositoryResult<SalesViewInterfaceModel> GetSalesViewByIdForInterface(int productId, CustomParameterModel parameter, int months = 12)
        {
            try
            {
                SalesViewMode mode = SalesViewMode.NONE;
                if (productId <= 0)
                {
                    return new RepositoryResult<SalesViewInterfaceModel>(HttpStatusCode.BadRequest);
                }
                if (parameter.Year.HasValue)
                {
                    mode = SalesViewMode.YEAR;
                }
                if (parameter.Begin.HasValue && parameter.End.HasValue)
                {
                    mode = SalesViewMode.FromTo;
                }
                if (mode == SalesViewMode.NONE)
                {
                    return new RepositoryResult<SalesViewInterfaceModel>(HttpStatusCode.BadRequest);
                }

                using (DataContext dbContext = DataContext.Create())
                using (var productRepository = new ProductRepository())
                {
                    var product = productRepository.GetProductByIdForInterface(productId);
                    if (product.Code != HttpStatusCode.OK)
                    {
                        return new RepositoryResult<SalesViewInterfaceModel>(HttpStatusCode.BadRequest);
                    }

                    // 全体の販売実績データ
                    List<SalesInterfaceModel> salesList;
                    DateTime startDate = DateTime.Now;
                    DateTime endDate = DateTime.Now;
                    if (mode == SalesViewMode.YEAR)
                    {
                        // 在庫予測計算用に１ヶ月多めに取得（貿易のみ）
                        startDate = DateTime.Parse((parameter.Year - 1).ToString() + "/9/1");
                        endDate = startDate.AddMonths(months + 1);
                    }
                    else if (mode == SalesViewMode.FromTo)
                    {
                        startDate = parameter.Begin.GetValueOrDefault();
                        endDate = startDate.AddMonths(months);
                    }
                    salesList = dbContext.Database.SqlQuery<SalesInterfaceModel>(ServiceResource.SelectSalesViews, startDate, endDate, product.resultData.Id).ToList<SalesInterfaceModel>();

                    ICollection<OfficeModel> office = dbContext.OfficeModels.Where(o => o.Deleted == false).OrderBy(o => o.Code).ToList();

                    // 事業所別
                    List<SalesOfficeInterfaceModel> salesOfficeList;
                    salesOfficeList = dbContext.Database.SqlQuery<SalesOfficeInterfaceModel>(ServiceResource.SelectOfficesSalesViews, startDate, endDate, product.resultData.Id).ToList<SalesOfficeInterfaceModel>();

                    // 在庫予測計算用に１ヶ月多めに取得（貿易のみ）
                    // DateTime check = startDate.AddMonths(-1);
                    DateTime check = startDate;

                    var resultData = new SalesViewInterfaceModel();
                    resultData.Product = product.resultData;
                    resultData.SalesList = new List<SalesInterfaceModel>();
                    resultData.OfficeSales = new List<ICollection<SalesOfficeInterfaceModel>>();

                    for (; check <= endDate; check = check.AddMonths(1))
                    {
                        var work = salesList.Where(x => x.product_id == resultData.Product.Id).Where(x => x.detail_date == check).SingleOrDefault();
                        if (work == null)
                        {
                            var tempModel = new SalesInterfaceModel();
                            tempModel.product_id = resultData.Product.Id;
                            tempModel.detail_date = check;
                            resultData.SalesList.Add(tempModel);
                        }
                        else
                        {
                            resultData.SalesList.Add(work);
                        }
                    }

                    foreach (var value in office)
                    {
                        var workOffice = new List<SalesOfficeInterfaceModel>();
                        check = startDate;
                        for (; check <= endDate; check = check.AddMonths(1))
                        {
                            var ofsData = salesOfficeList.Where(x => x.product_id == resultData.Product.Id).Where(x => x.detail_date == check).Where(x => x.office_id == value.Id).SingleOrDefault();
                            if (ofsData == null)
                            {
                                var tempModel = new SalesOfficeInterfaceModel();
                                tempModel.product_id = resultData.Product.Id;
                                tempModel.detail_date = check;
                                tempModel.office_id = value.Id;
                                tempModel.office_name = value.Name;
                                tempModel.sales_plan = 0;
                                tempModel.sales_actual = 0;
                                workOffice.Add(tempModel);
                            }
                            else
                            {
                                ofsData.office_id = value.Id;
                                ofsData.office_name = value.Name;
                                workOffice.Add(ofsData);
                            }
                        }
                        resultData.OfficeSales.Add(workOffice);
                    }

                    return new RepositoryResult<SalesViewInterfaceModel>(HttpStatusCode.OK, resultData);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public RepositoryResult<SalesViewInterfaceModel> SetSalesView(SalesViewInterfaceModel setSalesView)
        {
            try
            {
                if (setSalesView == null)
                {
                    return new RepositoryResult<SalesViewInterfaceModel>(HttpStatusCode.BadRequest);
                }

                if (setSalesView.Product == null || setSalesView.Product.Id <= 0)
                {
                    return new RepositoryResult<SalesViewInterfaceModel>(HttpStatusCode.BadRequest);
                }

                using (DataContext dbContext = DataContext.Create())
                {

                    int product_id = dbContext.ProductModels.Where(pd => pd.Id == setSalesView.Product.Id).Select(pd => pd.Id).SingleOrDefault();
                    if (product_id == 0)
                    {
                        return new RepositoryResult<SalesViewInterfaceModel>(HttpStatusCode.BadRequest);
                    }

                    int office_id = dbContext.OfficeModels.Where(o => o.Code == ServiceResource.CompanyCode).Select(o => o.Id).SingleOrDefault();
                    if (office_id == 0)
                    {
                        return new RepositoryResult<SalesViewInterfaceModel>(HttpStatusCode.InternalServerError);
                    }

                    // 最小日付と最大日付
                    DateTime MinDate = setSalesView.SalesList.Select(sl => sl.detail_date).Min();
                    DateTime MaxDate = setSalesView.SalesList.Select(sl => sl.detail_date).Max();

                    using (DbContextTransaction tx = dbContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                    {
                        foreach (var month in setSalesView.SalesList)
                        {
                            if (month.detail_date != MinDate)
                            {
                                // 販売予算を更新
                                SalesModel sales = dbContext.SalesModels.Where(sls => sls.ProductModelId == product_id)
                                    .Where(sls => sls.TargetDate == month.detail_date)
                                    .Where(sls => sls.OfficeModelId == office_id).SingleOrDefault();
                                if (sales != null)
                                {
                                    // 削除データが表示される際に不整合を起こさないため
                                    if (sales.Deleted)
                                        sales.Sales = 0;

                                    sales.Plan = month.sales_plan;
                                    sales.Deleted = false;
                                }
                                else
                                {
                                    if (month.sales_plan != 0)
                                    {
                                        sales = new SalesModel();
                                        sales.ProductModelId = product_id;
                                        sales.TargetDate = month.detail_date;
                                        sales.Plan = month.sales_plan;
                                        sales.OfficeModelId = office_id;
                                        dbContext.SalesModels.Add(sales);
                                    }
                                }
                                dbContext.SaveChanges();

                                // 貿易
                                TradeModel trades = dbContext.TradeModels.Where(trd => trd.ProductModelId == product_id)
                                    .Where(trd => trd.TargetDate == month.detail_date).SingleOrDefault();
                                if (trades != null)
                                {
                                    // 削除データが表示される際に不整合を起こさないため
                                    if (trades.Deleted)
                                    {
                                        trades.Order = 0;
                                        trades.Invoice = 0;
                                        trades.RemainingInvoice = 0;
                                    }

                                    trades.OrderPlan = month.order_plan;
                                    trades.InvoicePlan = month.invoice_plan;
                                    trades.AdjustmentInvoice = month.invoice_adjust;
                                    trades.Deleted = false;
                                }
                                else
                                {
                                    trades = new TradeModel();
                                    trades.ProductModelId = product_id;
                                    trades.TargetDate = month.detail_date;
                                    trades.OrderPlan = month.order_plan;
                                    trades.InvoicePlan = month.invoice_plan;
                                    trades.AdjustmentInvoice = month.invoice_adjust;
                                    dbContext.TradeModels.Add(trades);
                                }
                                dbContext.SaveChanges();
                            }
                        }
                        tx.Commit();
                    }

                    dbContext.Database.ExecuteSqlCommand("call _recalculation_invoicedata_id(@p0,@p1,@p2)", MinDate, DateTime.Now.Date, product_id);
                    return new RepositoryResult<SalesViewInterfaceModel>(HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object GetCurrentStocks(int productId, CustomParameterModel parameter)
        {
            using (DataContext dbContext = DataContext.Create())
            {
                var product = dbContext.ProductModels.Where(p => p.Id == productId).SingleOrDefault();
                if (product == null)
                {
                    return null;
                }

                var stocks = dbContext.CurrentStockModels
                    .Where(cs => cs.Deleted == false)
                    .Where(cs => cs.ProductModelId == product.Id)
                    .Select(cs => new { cs.WarehouseCode, cs.WarehouseName, cs.StateName, cs.ExpirationDate, cs.LogicalQuantity, cs.ActualQuantity })
                    .OrderBy(no => no.WarehouseCode).ThenBy(no => no.ExpirationDate).ThenBy(no => no.StateName)
                    .ToList();

                var stockMaxDate = dbContext.CurrentStockModels
                    .Where(cs => cs.Deleted == false)
                    .Where(cs => cs.ProductModelId == product.Id)
                    .Select(cs => cs.ModifiedDateTime).Max();

                var orders = dbContext.OrderModels
                    .Where(od => od.Deleted == false)
                    .Where(od => od.ProductModelId == product.Id)
                    .Select(od => new { od.OrderNo, od.OrderDate, od.Order }).ToList();

                var orderMaxDate = dbContext.OrderModels
                    .Where(od => od.Deleted == false)
                    .Where(od => od.ProductModelId == product.Id)
                    .Select(od => od.ModifiedDateTime).Max();

                var invoices = dbContext.InvoiceModels
                    .Where(iv => iv.Deleted == false)
                    .Where(iv => iv.ProductModelId == product.Id)
                    .Select(iv => new { iv.InvoiceNo, iv.WarehouseCode, iv.ETA, iv.CustomsClearanceDate, iv.PurchaseDate, iv.Quantity }).ToList();

                var invoiceMaxDate = dbContext.InvoiceModels
                    .Where(iv => iv.Deleted == false)
                    .Where(iv => iv.ProductModelId == product.Id)
                    .Select(iv => iv.ModifiedDateTime).Max();

                return new { stocks, orders, invoices, stockMaxDate, orderMaxDate, invoiceMaxDate };
            }
        }

        public IEnumerable<SalesTrendInterfaceModel> GetSalesTrandsForInterface(int productId, CustomParameterModel parameter, int months = 12)
        {
            SalesViewMode mode = SalesViewMode.NONE;
            using (DataContext dbContext = DataContext.Create())
            using (var productRepository = new ProductRepository())
            {
                var product = productRepository.GetProductByIdForInterface(productId);
                if (product.Code != HttpStatusCode.OK)
                {
                    return null;
                }
                if (parameter.Year.HasValue)
                {
                    mode = SalesViewMode.YEAR;
                }
                if (parameter.Begin.HasValue && parameter.End.HasValue)
                {
                    mode = SalesViewMode.FromTo;
                }
                if (mode == SalesViewMode.NONE)
                {
                    return null; ;
                }

                DateTime startDate = DateTime.Now;
                DateTime endDate = DateTime.Now;
                if (mode == SalesViewMode.YEAR)
                {
                    // 在庫予測計算用に１ヶ月多めに取得（貿易のみ）
                    startDate = DateTime.Parse((parameter.Year - 1).ToString() + "/9/1");
                    endDate = startDate.AddMonths(months + 1);
                }
                else if (mode == SalesViewMode.FromTo)
                {
                    startDate = parameter.Begin.GetValueOrDefault();
                    endDate = startDate.AddMonths(months);
                }
                // DateTime startDate = DateTime.Parse((parameter.Year.GetValueOrDefault() - 1).ToString() + "/10/1");
                // DateTime endDate = startDate.AddYears(1);

                var trends = dbContext.SalesTrendModels
                    .Where(st => st.ProductModelId == product.resultData.Id).Where(st => st.Deleted == false)
                    .Where(st => st.TargetDate >= startDate).Where(st => st.TargetDate < endDate)
                    // .ProjectTo<SalesTrendInterfaceModel>()
                    .Select(st => new SalesTrendInterfaceModel
                    {
                        Id = st.Id,
                        Product_id = st.ProductModelId,
                        Detail_date = st.TargetDate,
                        Quantity = st.Sales,
                        Comments = st.Comments,
                        User_id = st.UserModelId,
                        User_name = st.UserModel.Name
                    })
                    .ToList();

                return trends;
            }
        }

        public RepositoryResult<SalesTrendInterfaceModel> GetSalesTrandsByIdForInterface(int productId, int trendId)
        {
            try
            {
                using (DataContext dbContext = DataContext.Create())
                {
                    var trends = dbContext.SalesTrendModels.Where(st => st.ProductModelId == productId).Where(st => st.Id == trendId)
                    //.ProjectTo<SalesTrendInterfaceModel>()
                    .Select(st => new SalesTrendInterfaceModel
                    {
                        Id = st.Id,
                        Product_id = st.ProductModelId,
                        Detail_date = st.TargetDate,
                        Quantity = st.Sales,
                        Comments = st.Comments,
                        User_id = st.UserModelId,
                        User_name = st.UserModel.Name
                    })
                    .SingleOrDefault();
                    if (trends == null)
                    {
                        return new RepositoryResult<SalesTrendInterfaceModel>(HttpStatusCode.NotFound);
                    }
                    return new RepositoryResult<SalesTrendInterfaceModel>(HttpStatusCode.OK, trends);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public RepositoryResult<SalesTrendInterfaceModel> CreateSalesTrend(int productId, SalesTrendInterfaceModel createSalesTrend)
        {
            if (createSalesTrend == null || createSalesTrend.Id != 0)
            {
                return new RepositoryResult<SalesTrendInterfaceModel>(HttpStatusCode.BadRequest);
            }

            using (DataContext dbContext = DataContext.Create())
            {
                var addmodel = new SalesTrendModel();
                addmodel.ProductModelId = createSalesTrend.Product_id;
                addmodel.TargetDate = createSalesTrend.Detail_date.Date;
                addmodel.Sales = createSalesTrend.Quantity;
                addmodel.Comments = createSalesTrend.Comments;
                addmodel.UserModelId = createSalesTrend.User_id;
                addmodel.Deleted = false;

                dbContext.Database.ExecuteSqlCommand(ServiceResource.IncrementResetSalesTrend);
                dbContext.SalesTrendModels.Add(addmodel);
                if (dbContext.SaveChanges() == 0)
                {
                    return new RepositoryResult<SalesTrendInterfaceModel>(HttpStatusCode.BadRequest);
                }
                dbContext.Database.ExecuteSqlCommand(ServiceResource.IncrementResetSalesTrend);

                var result = GetSalesTrandsByIdForInterface(productId, addmodel.Id);
                if (result.Code != HttpStatusCode.OK)
                {
                    return new RepositoryResult<SalesTrendInterfaceModel>(HttpStatusCode.OK);
                }
                return new RepositoryResult<SalesTrendInterfaceModel>(HttpStatusCode.OK, result.resultData);
            }
        }

        public RepositoryResult<SalesTrendInterfaceModel> ModifySalesTrend(int productId, int trendId, SalesTrendInterfaceModel modifiedSalesTrend)
        {
            if (productId == 0 || trendId == 0 || modifiedSalesTrend == null || modifiedSalesTrend.Id ==0)
            {
                return new RepositoryResult<SalesTrendInterfaceModel>(HttpStatusCode.BadRequest);
            }

            using (DataContext dbContext = DataContext.Create())
            {
                var work = dbContext.SalesTrendModels.Where(st => st.Id == trendId).Where(st => st.ProductModelId == productId).SingleOrDefault();
                if (work == null)
                {
                    return new RepositoryResult<SalesTrendInterfaceModel>(HttpStatusCode.NotFound);
                }

                work.TargetDate = modifiedSalesTrend.Detail_date;
                work.Sales = modifiedSalesTrend.Quantity;
                work.Comments = modifiedSalesTrend.Comments;
                work.UserModelId = modifiedSalesTrend.User_id;
                if (dbContext.SaveChanges() == 0)
                {
                    return new RepositoryResult<SalesTrendInterfaceModel>(HttpStatusCode.BadRequest);
                }

                var result = GetSalesTrandsByIdForInterface(productId, work.Id);
                if (result.Code != HttpStatusCode.OK)
                {
                    return new RepositoryResult<SalesTrendInterfaceModel>(HttpStatusCode.OK);
                }
                return new RepositoryResult<SalesTrendInterfaceModel>(HttpStatusCode.OK, result.resultData);
            }
        }

        public RepositoryResult<SalesTrendInterfaceModel> DeleteSalesTrend(int productId, int trendId)
        {
            using (DataContext dbContext = DataContext.Create())
            {
                SalesTrendModel work = dbContext.SalesTrendModels.Where(st => st.Id == trendId).Where(st => st.ProductModelId == productId).SingleOrDefault();
                if (work == null)
                {
                    return new RepositoryResult<SalesTrendInterfaceModel>(HttpStatusCode.NotFound);
                }
                work.Deleted = true;
                if (dbContext.SaveChanges() == 0)
                {
                    return new RepositoryResult<SalesTrendInterfaceModel>(HttpStatusCode.BadRequest);
                }
                return new RepositoryResult<SalesTrendInterfaceModel>(HttpStatusCode.OK);
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
