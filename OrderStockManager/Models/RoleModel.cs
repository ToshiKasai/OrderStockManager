using Microsoft.AspNet.Identity;
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
    [Table("roles")]
    public class RoleModel : BaseModel, IRole<int>
    {
        #region IRole<int>
        [Key, Column("id")]
        public int Id { get; set; }

        [DisplayName("ロール名"), Column("name")]
        [Required, MaxLength(128)]
        [Index("ui_name", IsUnique = true)]
        public string Name { get; set; }
        #endregion

        #region カスタム項目
        [DisplayName("概要"), Column("comment")]
        [Required, MaxLength(256)]
        public string DisplayName { get; set; }
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
        public virtual ICollection<UserRoleModel> UserRoleModels { get; set; }
        #endregion
    }
}
