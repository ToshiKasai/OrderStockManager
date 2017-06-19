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
    [Table("groups")]
    public class GroupModel : BaseModel
    {
        [Key, Column("id")]
        public int Id { get; set; }

        [DisplayName("グループコード"), Column("code")]
        [Required, MaxLength(128), RegularExpression(@"[a-zA-Z0-9]+")]
        [Index("ui_code", IsUnique = true)]
        public string Code { get; set; }

        [DisplayName("グループ名"), Column("name")]
        [Required, MaxLength(256)]
        public string Name { get; set; }

        [DisplayName("メーカーＩＤ"), Column("maker_id")]
        [Required]
        [Index("idx_maker_id")]
        public int MakerModelId { get; set; }

        [DisplayName("コンテナＩＤ"), Column("container_id")]
        [Required]
        [Index("idx_container_id")]
        public int ContainerModelId { get; set; }

        [DisplayName("容量管理"), Column("is_capacity")]
        [DefaultValue(true)]
        public bool IsCapacity { get; set; }

        [DisplayName("コンテナ入数：２０ドライ"), Column("capacity_20dry")]
        public decimal ContainerCapacityBt20Dry { get; set; }

        [DisplayName("コンテナ入数：４０ドライ"), Column("capacity_40dry")]
        public decimal ContainerCapacityBt40Dry { get; set; }

        [DisplayName("コンテナ入数：２０リーファ"), Column("capacity_20reefer")]
        public decimal ContainerCapacityBt20Reefer { get; set; }

        [DisplayName("コンテナ入数：４０リーファ"), Column("capacity_40reefer")]
        public decimal ContainerCapacityBt40Reefer { get; set; }

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
        [ForeignKey("ContainerModelId")]
        public virtual ContainerModel ContainerModel { get; set; }
        #endregion
    }
}
