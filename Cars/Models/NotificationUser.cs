using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Models
{
    public class NotificationUser
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long NotificationUserID { get; set; }
        public bool IsRead { get; set; }
        public DateTime DTsCreate { get; set; }
        public long NotificationID { get; set; }
        [ForeignKey("NotificationID")]
        public Notification Notification { get; set; }
        public string UserID { get; set; }
        [ForeignKey("UserID")]
        public virtual ApplicationUser User { get; set; }
    }
}
