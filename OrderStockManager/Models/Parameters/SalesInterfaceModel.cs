using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderStockManager.Models.Parameters
{
    public class SalesInterfaceModel : BaseModel
    {
        public int product_id { get; set; }
        public DateTime detail_date { get; set; }
        public decimal zaiko_actual { get; set; }
        public decimal order_plan { get; set; }
        public decimal order_actual { get; set; }
        public decimal invoice_plan { get; set; }
        public decimal invoice_actual { get; set; }
        public decimal invoice_zan { get; set; }
        public decimal invoice_adjust { get; set; }
        public decimal pre_sales_actual { get; set; }
        public decimal sales_plan { get; set; }
        public decimal sales_actual { get; set; }
        public decimal sales_trend { get; set; }
    }
}
