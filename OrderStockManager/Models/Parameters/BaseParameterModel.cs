using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace OrderStockManager.Models.Parameters
{
    public class BaseParameterModel : BaseModel
    {
        public int? Limit { get; set; }

        public int? Page { get; set; }

        [DefaultValue(false)]
        public bool Deleted { get; set; }

        [DefaultValue(true)]
        public bool Enabled { get; set; }
    }
}
