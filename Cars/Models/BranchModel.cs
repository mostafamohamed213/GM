using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Models
{
    public class BranchModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int BranchID { get; set; }
        public string Name { get; set; }
        public string BranchIP { get; set; }
        public bool IsActive { get; set; }
        public DateTime DTsCreate { get; set; }
        public DateTime? DTsUpdate { get; set; }
    }
}
