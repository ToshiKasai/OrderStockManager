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
    [Table("sales")]
    public class SalesModel : BaseModel
    {
        [Key, Column("id")]
        public int Id { get; set; }

        [DisplayName("商品ＩＤ"), Column("product_id")]
        [Required]
        [Index("ui_sales", IsUnique = true, Order = 0)]
        public int ProductModelId { get; set; }

        [DisplayName("対象年月"), Column("detail_date", TypeName = "Date")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy/MM}")]
        [Required]
        [Index("ui_sales", IsUnique = true, Order = 1)]
        public DateTime TargetDate { get; set; }

        [DisplayName("事務所ＩＤ"), Column("office_id")]
        [Required]
        [Index("ui_sales", IsUnique = true, Order = 2)]
        public int OfficeModelId { get; set; }

        [DisplayName("販売予算"), Column("plan")]
        [DefaultValue(0)]
        public decimal Plan { get; set; }

        [DisplayName("販売実績"), Column("actual")]
        [DefaultValue(0)]
        public decimal Sales { get; set; }

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

        [JsonIgnore]
        [ForeignKey("OfficeModelId")]
        public virtual OfficeModel OfficeModel { get; set; }
        #endregion
    }
}
