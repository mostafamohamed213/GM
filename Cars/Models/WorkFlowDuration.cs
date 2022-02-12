using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Models
{
    public class WorkFlowDuration
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int WorkFlowDurationID { get; set; }

       
        public int WorkflowID { get; set; }

        public double Duration { get; set; }

        [ForeignKey("WorkflowID")]
        public virtual Workflow WorkFlow { get; set; }

       

    }
}
