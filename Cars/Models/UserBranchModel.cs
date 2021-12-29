using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Models
{
    public class UserBranchModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int UserBranchID { get; set; }
        public string UserID { get; set; }
        [ForeignKey("UserID")]
        public virtual ApplicationUser User { get; set; }
        public int BranchID { get; set; }
        [ForeignKey("BranchID")]
        public virtual BranchModel Branch { get; set; }
        public bool IsActive { get; set; }
        public DateTime DTsCreate { get; set; }
        public DateTime? DTsEnd { get; set; }
    }
}
