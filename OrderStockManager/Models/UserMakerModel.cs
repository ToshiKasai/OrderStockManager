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
    [Table("users_makers")]
    public class UserMakerModel
    {
        [Key, Column("id")]
        public int Id { get; set; }

        [DisplayName("ユーザーＩＤ"), Column("user_id")]
        [Required]
        [Index("idx_user_id")]
        public int UserModelId { get; set; }

        [DisplayName("メーカーＩＤ"), Column("maker_id")]
        [Required]
        [Index("idx_maker_id")]
        public int MakerModelId { get; set; }

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
        [ForeignKey("UserModelId")]
        public virtual UserModel UserModel { get; set; }
        [JsonIgnore]
        [ForeignKey("MakerModelId")]
        public virtual MakerModel MakerModel { get; set; }
        #endregion
    }
}
