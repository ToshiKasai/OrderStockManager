using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderStockManager.Services
{
    public class SalesViewExcelService :  SupportExcel, ISalesViewExcelService, IDisposable
    {
        public SalesViewExcelService() : base()
        {
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
