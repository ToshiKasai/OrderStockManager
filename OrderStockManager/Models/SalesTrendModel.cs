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
    [Table("sales_trends")]
    public class SalesTrendModel : BaseModel
    {
        [Key, Column("id")]
        public int Id { get; set; }

        [DisplayName("商品ＩＤ"), Column("product_id")]
        [Required]
        [Index("idx_product_id_detail_date", IsUnique = false, Order = 0)]
        public int ProductModelId { get; set; }

        [DisplayName("対象年月"), Column("detail_date", TypeName = "Date")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy/MM}")]
        [Required]
        [Index("idx_product_id_detail_date", IsUnique = false, Order = 1)]
        public DateTime TargetDate { get; set; }

        [DisplayName("数量"), Column("quantity")]
        [DefaultValue(0)]
        public decimal Sales { get; set; }

        [DisplayName("コメント"), Column("comments")]
        public string Comments { get; set; }

        [DisplayName("ユーザーＩＤ"), Column("user_id")]
        [Required]
        [Index("idx_user_id")]
        public int UserModelId { get; set; }

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
        [ForeignKey("UserModelId")]
        public virtual UserModel UserModel { get; set; }
        #endregion
    }
}
