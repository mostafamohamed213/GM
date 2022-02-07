using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Models
{
    public class Notification
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long NotificationID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DTsCreate { get; set; }

        public virtual List<NotificationUser> NotificationUser { get; set; }
    }
}
