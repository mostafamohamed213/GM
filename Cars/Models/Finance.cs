using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Models
{
    public class Finance
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long FinanceID { get; set; }
        public bool Confirmed { get; set; }
        public virtual List<OrderDetails> OrderDetails { get; set; }
        public virtual List<FinanceDocument> FinanceDocument { get; set; }

        [Required]
        public string SystemUserCreate { get; set; }
        public DateTime DTsCreate { get; set; }
        public string SystemUserUpdate { get; set; }
        public DateTime? DTsUpdate { get; set; }
    }
}
