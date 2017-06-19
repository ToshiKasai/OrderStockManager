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
    [Table("current_ctocks")]
    public class CurrentStockModel
    {
        [Key, Column("id")]
        public int Id { get; set; }

        [DisplayName("商品ＩＤ"), Column("product_id")]
        [Required]
        [Index("idx_product_id")]
        public int ProductModelId { get; set; }

        [DisplayName("倉庫コード"), Column("warehouse_code")]
        [Required, MaxLength(128)]
        public string WarehouseCode { get; set; }

        [DisplayName("倉庫名"), Column("warehouse_name")]
        [Required, MaxLength(256)]
        public string WarehouseName { get; set; }

        [DisplayName("打検名"), Column("state_name")]
        [Required, MaxLength(128)]
        public string StateName { get; set; }

        [DisplayName("論理在庫数"), Column("logical_qty")]
        [DefaultValue(0)]
        public decimal LogicalQuantity { get; set; }

        [DisplayName("実在庫数量"), Column("actual_qty")]
        [DefaultValue(0)]
        public decimal ActualQuantity { get; set; }

        [DisplayName("賞味期限"), Column("expiration_date", TypeName = "Date")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? ExpirationDate { get; set; }

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
