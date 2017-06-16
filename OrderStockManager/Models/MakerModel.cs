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
    [Table("makers")]
    public class MakerModel : BaseModel
    {
        [Key, Column("id")]
        public int Id { get; set; }

        [DisplayName("メーカーコード"), Column("code")]
        [Required, MaxLength(128), RegularExpression(@"[a-zA-Z0-9]+")]
        [Index("ui_code", IsUnique = true)]
        public string Code { get; set; }

        [DisplayName("メーカー名"), Column("name")]
        [Required, MaxLength(256)]
        public string Name { get; set; }

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
        #endregion
    }
}
