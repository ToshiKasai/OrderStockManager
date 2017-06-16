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
    [Table("users_roles")]
    public class UserRoleModel : BaseModel
    {
        #region 項目
        [Key, Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("ユーザーＩＤ"), Column("user_id")]
        [Index("idx_user_id")]
        [Required]
        public int UserModelId { get; set; }

        [DisplayName("ロールＩＤ"), Column("role_id")]
        [Index("idx_role_id")]
        [Required]
        public int RoleModelId { get; set; }
        #endregion

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
        [ForeignKey("RoleModelId")]
        public virtual RoleModel RoleModel { get; set; }
        #endregion
    }
}
