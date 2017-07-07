using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderStockManager.Models.Parameters
{
    public class CustomParameterModel : BaseParameterModel
    {
        public int? GroupId { get; set; }
        public int? MakerId { get; set; }
        public int? Year { get; set; }
        public DateTime? Begin { get; set; }
        public DateTime? End { get; set; }
        // public int? UserId { get; set; }
    }
}
