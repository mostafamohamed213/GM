using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Models
{
    public class OrderDetails
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long OrderDetailsID { get; set; }
        [Required]
        public string Items { get; set; }
        public int Quantity { get; set; }
        public bool? IsApproved { get; set; }
        public string Prefix { get; set; }
        public int BranchID { get; set; }
        public string PartNumber { get; set; }
        public decimal? Price { get; set; }
        public string Comments { get; set; }
        public DateTime? UsedDateTime { get; set; }
        public string UsedByUser { get; set; }
        public long? ParentOrderDetailsID { get; set; }
        public long OrderID { get; set; }
        [ForeignKey("OrderID")]
        public virtual Order Order { get; set; }

        public int OrderDetailsTypeID { get; set; }
        [ForeignKey("OrderDetailsTypeID")]
        public OrderDetailsType OrderDetailsType { get; set; }

        public int? VendorLocationID { get; set; }
        [ForeignKey("VendorLocationID")]
        public VendorLocation VendorLocation { get; set; }

        public int WorkflowID { get; set; }
        [ForeignKey("WorkflowID")]
        public Workflow Workflow { get; set; }  
      
        public string SystemUserCreate { get; set; }
        public DateTime DTsCreate { get; set; }     

        public decimal? Labor_Hours { get; set; } 

        public double? Labor_Value { get; set; }

        public int StatusID { get; set; }
        [ForeignKey("StatusID")]
        public Status Status { get; set; }
    }

}
