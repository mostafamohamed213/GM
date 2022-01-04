using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.ViewModels
{
    public class PricingAddOrderLineViewModel
    {
        public long ParentOrderDetailsId { get; set; }
        public string Items { get; set; }
        public int? Quantity { get; set; }
        public int TypeID { get; set; }
        [Required]
        public string PartNumber { get; set; }
        [Range(1, double.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "Primary Contact Name is required")]
        public int VendorLocationID { get; set; }
        public string Comments { get; set; }
        public SelectList VendorLocations { get; set; }
        public SelectList OrderDetailsTypes { get; set; }
    }
}
