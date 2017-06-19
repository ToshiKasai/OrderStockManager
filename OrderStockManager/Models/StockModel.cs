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
    [Table("stocks")]
    public class StockModel : BaseModel
    {
        [Key, Column("id")]
        public int ZaikoModelId { get; set; }

        [DisplayName("商品ＩＤ"), Column("product_id")]
        [Required]
        [Index("ui_product_id_detail_date", IsUnique = true, Order = 0)]
        public int ProductModelId { get; set; }

        [DisplayName("対象年月"), Column("detail_date", TypeName = "Date")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        [Required]
        [Index("ui_product_id_detail_date", IsUnique = true, Order = 1)]
        public DateTime TargetDate { get; set; }

        [DisplayName("在庫数"), Column("stocks")]
        [DefaultValue(0)]
        public decimal ShohinZaiko { get; set; }

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
