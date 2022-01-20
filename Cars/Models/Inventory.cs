using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Models
{
    public class Inventory
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long InventoryID { get; set; }
        public bool Confirmed { get; set; }
        public string Reason { get; set; }
        public int Quantity { get; set; }
        public virtual List<OrderDetails> OrderDetails { get; set; }
        public virtual List<InventoryDocument> InventoryDocument { get; set; }

        [Required]
        public string SystemUserCreate { get; set; }
        public DateTime DTsCreate { get; set; }
        public string SystemUserUpdate { get; set; }
        public DateTime? DTsUpdate { get; set; }
    }
}
