using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OrderStockManager.Models
{
    [Table("invoices")]
    public class InvoiceModel
    {
        [Key, Column("id")]
        public int Id { get; set; }

        [DisplayName("インボイス番号"), Column("invoice_no")]
        [Required, MaxLength(128)]
        public string InvoiceNo { get; set; }

        [DisplayName("倉庫コード"), Column("warehouse_code")]
        [Required, MaxLength(128)]
        public string WarehouseCode { get; set; }

        [DisplayName("入港予定日"), Column("eta", TypeName = "Date")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? ETA { get; set; }

        [DisplayName("通関日"), Column("customs_clearance", TypeName = "Date")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? CustomsClearanceDate { get; set; }

        [DisplayName("仕入日"), Column("purchase_date", TypeName = "Date")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? PurchaseDate { get; set; }

        [DisplayName("商品ＩＤ"), Column("product_id")]
        [Required]
        [Index("idx_product_id")]
        public int ProductModelId { get; set; }

        [DisplayName("仕入数量"), Column("quantity")]
        [DefaultValue(0)]
        public decimal Quantity { get; set; }

        #region 定型管理項目
        [DisplayName("削除済"), Column("deleted")]
        [DefaultValue(false)]
        public bool Deleted { get; set; }

        [DisplayName("作成日時"), Column("created_at")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDateTime { get; set; }

        [DisplayName("更新日時"), Column("modified_at")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? ModifiedDateTime { get; set; }
        #endregion

        #region データ連携
        [JsonIgnore]
        public virtual ProductModel ProductModel { get; set; }
        #endregion
    }
}
