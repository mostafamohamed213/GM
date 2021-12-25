using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Models
{
    public class WorkflowOrderDetailsLog
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long WorkflowOrderDetailsLogID { get; set; }      
        public string Details { get; set; }
        public bool Active { get; set; }
        public int WorkflowID { get; set; }
        [ForeignKey("WorkflowID")]
        public Workflow Workflow { get; set; }
        public long OrderDetailsID { get; set; }
        [ForeignKey("OrderDetailsID")]
        public OrderDetails OrderDetails { get; set; }
        [Required]
        public string SystemUserID { get; set; }
        public DateTime DTsCreate { get; set; }
    }
}
