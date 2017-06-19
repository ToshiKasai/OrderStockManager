using OrderStockManager.Models.Parameters;
using OrderStockManager.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderStockManager.Services
{
    public interface ISalesViewService
    {
        RepositoryResult<IEnumerable<SalesViewInterfaceModel>> GetSalesViewsForInterface(CustomParameterModel parameter, int month = 12);
        RepositoryResult<SalesViewInterfaceModel> GetSalesViewByIdForInterface(int productId, CustomParameterModel parameter, int months = 12);
        RepositoryResult<SalesViewInterfaceModel> SetSalesView(SalesViewInterfaceModel setSalesView);
        object GetCurrentStocks(int productId, CustomParameterModel parameter);
        IEnumerable<SalesTrendInterfaceModel> GetSalesTrandsForInterface(int productId, CustomParameterModel parameter);
        RepositoryResult<SalesTrendInterfaceModel> GetSalesTrandsByIdForInterface(int productId, int trendId);
        RepositoryResult<SalesTrendInterfaceModel> CreateSalesTrend(int productId, SalesTrendInterfaceModel createSalesTrend);
        RepositoryResult<SalesTrendInterfaceModel> ModifySalesTrend(int productId, int trendId, SalesTrendInterfaceModel modifiedSalesTrend);
        RepositoryResult<SalesTrendInterfaceModel> DeleteSalesTrend(int productId, int trendId);
    }
}
