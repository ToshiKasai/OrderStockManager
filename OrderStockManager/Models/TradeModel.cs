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
    [Table("trades")]
    public class TradeModel
    {
        [Key, Column("id")]
        public int Id { get; set; }

        [DisplayName("商品ＩＤ"), Column("product_id")]
        [Required]
        [Index("ui_product_id_detail_date", IsUnique = true, Order = 0)]
        public int ProductModelId { get; set; }

        [DisplayName("対象年月"), Column("detail_date", TypeName = "Date")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        [Required]
        [Index("ui_product_id_detail_date", IsUnique = true, Order = 1)]
        public DateTime TargetDate { get; set; }

        [DisplayName("発注予定"), Column("orders_plan_qty")]
        [DefaultValue(0)]
        public decimal OrderPlan { get; set; }

        [DisplayName("発注実績"), Column("orders_qty")]
        [DefaultValue(0)]
        public decimal Order { get; set; }

        [DisplayName("入荷予定"), Column("invoice_plan_qty")]
        [DefaultValue(0)]
        public decimal InvoicePlan { get; set; }

        [DisplayName("入荷実績"), Column("invoice_qty")]
        [DefaultValue(0)]
        public decimal Invoice { get; set; }

        [DisplayName("入荷残数"), Column("remaining_invoice_qty")]
        [DefaultValue(0)]
        public decimal RemainingInvoice { get; set; }

        [DisplayName("入荷残数調整"), Column("adjustment_invoice_qty")]
        [DefaultValue(0)]
        public decimal AdjustmentInvoice { get; set; }

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
        [ForeignKey("ProductModelId")]
        public virtual ProductModel ProductModel { get; set; }
        #endregion
    }
}
