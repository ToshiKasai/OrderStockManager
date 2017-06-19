using OrderStockManager.Models.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderStockManager.Repositories
{
    public interface IProductRepository
    {
        IEnumerable<ProductInterfaceModel> GetProductsForInterface(CustomParameterModel parameter);
        RepositoryResult<ProductInterfaceModel> GetProductByIdForInterface(int productId);
        RepositoryResult<ProductInterfaceModel> ModifyProduct(int productId, ProductInterfaceModel modifiedProduct);
        object GetProductsPages(CustomParameterModel parameter);
    }
}
