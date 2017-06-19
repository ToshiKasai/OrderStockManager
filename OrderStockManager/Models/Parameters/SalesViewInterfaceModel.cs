using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderStockManager.Models.Parameters
{
    public class SalesViewInterfaceModel : BaseModel
    {
        public ProductInterfaceModel Product { get; set; }
        public ICollection<SalesInterfaceModel> SalesList { get; set; }
        public ICollection<ICollection<SalesOfficeInterfaceModel>> OfficeSales { get; set; }
    }
}
