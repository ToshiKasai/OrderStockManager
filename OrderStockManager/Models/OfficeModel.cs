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
    [Table("offices")]
    public class OfficeModel : BaseModel
    {
        [Key, Column("id")]
        public int Id { get; set; }

        [DisplayName("事務所コード"), Column("code")]
        [Required, MaxLength(128)]
        [Index("ui_code", IsUnique = true)]
        public string Code { get; set; }

        [DisplayName("事務所名"), Column("name")]
        [Required, MaxLength(256)]
        public string Name { get; set; }

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
        public virtual ICollection<SalesModel> SalesModels { get; set; }
        #endregion
    }
}
