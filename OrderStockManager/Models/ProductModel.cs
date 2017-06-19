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
    [Table("products")]
    public class ProductModel : BaseModel
    {
        [Key, Column("id")]
        public int Id { get; set; }

        [DisplayName("商品コード"), Column("code")]
        [Required, MaxLength(128)]
        [Index("ui_code", IsUnique = true)]
        public string Code { get; set; }

        [DisplayName("商品名"), Column("name")]
        [Required, MaxLength(256)]
        public string Name { get; set; }

        [DisplayName("入数"), Column("quantity")]
        [Required, Range(0.0, 100.0)]
        [DefaultValue(0)]
        public decimal Quantity { get; set; }

        [DisplayName("メーカー"), Column("maker_id")]
        [Required]
        [Index("idx_maker_id")]
        public int MakerModelId { get; set; }

        [DisplayName("計量品"), Column("is_sold_weight")]
        [Required]
        [DefaultValue(false)]
        public bool IsSoldWeight { get; set; }

        [DisplayName("パレット入数"), Column("palette_quantity")]
        public decimal? PaletteQuantity { get; set; }

        [DisplayName("カートン入数"), Column("carton_quantity")]
        public decimal? CartonQuantity { get; set; }

        [DisplayName("ケース高さ"), Column("case_height")]
        public decimal? CaseHeight { get; set; }

        [DisplayName("ケース幅"), Column("case_width")]
        public decimal? CaseWidth { get; set; }

        [DisplayName("ケース奥行き"), Column("case_depth")]
        public decimal? CaseDepth { get; set; }

        [DisplayName("ケース容量"), Column("case_capacity")]
        public decimal? CaseCapacity { get; set; }

        [DisplayName("リードタイム"), Column("lead_time")]
        public int? LeadTime { get; set; }

        [DisplayName("発注間隔"), Column("order_interval")]
        public int? OrderInterval { get; set; }

        [DisplayName("関連商品"), Column("old_product_id")]
        [Index("idx_old_product_id")]
        public int? OldProductModelId { get; set; }

        [DisplayName("倍率"), Column("magnification")]
        public decimal? Magnification { get; set; }

        [DisplayName("最低発注数量"), Column("minimum_order_quantity")]
        public decimal? MinimumOrderQuantity { get; set; }

        [DisplayName("使用許可"), Column("enabled")]
        [DefaultValue(true)]
        public bool Enabled { get; set; }

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
        [ForeignKey("MakerModelId")]
        public virtual MakerModel MakerModel { get; set; }
        [JsonIgnore]
        public virtual ICollection<GroupProductModel> GroupProductModels { get; set; }
        [JsonIgnore]
        public virtual ICollection<SalesModel> SalesModels { get; set; }
        [JsonIgnore]
        public virtual ICollection<StockModel> StockModels { get; set; }
        [JsonIgnore]
        public virtual ICollection<TradeModel> TradeModels { get; set; }
        [JsonIgnore]
        public virtual ICollection<SalesTrendModel> SalesTrendModels { get; set; }
        [JsonIgnore]
        [ForeignKey("OldProductModelId")]
        public virtual ProductModel OldProductModel { get; set; }

        [JsonIgnore]
        public virtual ICollection<CurrentStockModel> CurrentStockModels { get; set; }
        [JsonIgnore]
        public virtual ICollection<OrderModel> OrderModels { get; set; }
        [JsonIgnore]
        public virtual ICollection<InvoiceModel> InvoiceModels { get; set; }
        #endregion
    }
}
