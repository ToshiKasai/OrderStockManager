using OrderStockManager.Models.Parameters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderStockManager.Services
{
    public interface ISalesViewExcelService
    {
        byte[] CreateXlsxOneSheetBySalesView(CustomParameterModel param);
        bool ReadXlsxToSalesView(FileInfo fileInfo);
    }
}
