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
    [Table("application_logs")]
    public class ApplicationLogModel : BaseModel
    {
        [Key, Column("id")]
        public int Id { get; set; }

        [DisplayName("処理日"), Column("processing_date")]
        [DataType(DataType.DateTime)]
        public DateTime ProcessingDate { get; set; }

        [DisplayName("処理名"), Column("process_name")]
        [MaxLength(256)]
        public string ProcessName { get; set; }

        [DisplayName("ユーザーＩＤ"), Column("user_id")]
        [Index("idx_user_id")]
        public int UserModelId { get; set; }

        [DisplayName("処理内容"), Column("message")]
        public string Message { get; set; }

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
        #endregion
    }
}
