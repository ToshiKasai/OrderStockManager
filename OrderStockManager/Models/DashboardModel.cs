using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OrderStockManager.Models
{
    [Table("dashboard")]
    public class DashboardModel : BaseModel
    {
        [Key, Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column("startDateTime")]
        public DateTime StartDateTime { get; set; }

        [Required]
        [Column("endDateTime")]
        public DateTime EndDateTime { get; set; }

        [Required]
        [Column("priority"), DefaultValue(0)]
        [Range(0, 99)]
        public int Priority { get; set; }

        [Required]
        [Column("message")]
        public string Message { get; set; }

        #region 定型管理項目
        [Column("deleted")]
        [DefaultValue(false)]
        public bool Deleted { get; set; }

        [Column("created_at")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDateTime { get; set; }

        [Column("modified_at")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime ModifiedDateTime { get; set; }
        #endregion
    }
}
