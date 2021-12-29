using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Models
{
    public class OrderDetailsStatusLog
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long OrderDetailsStatusLogID { get; set; }
        public string Detatils { get; set; }      
        public int StatusID { get; set; }
        [ForeignKey("StatusID")]
        public Status Status { get; set; }
        public long OrderDetailsID { get; set; }
        [ForeignKey("OrderDetailsID")]
        public OrderDetails OrderDetails { get; set; }
        [Required]
        public string SystemUserID { get; set; }
        public DateTime DTsCreate { get; set; }
    }
}
