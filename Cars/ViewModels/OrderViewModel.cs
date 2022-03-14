using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.ViewModels
{
    public class OrderViewModel
    {
        public int OrderID { get; set; }
        [Required]
        [StringLength(17, MinimumLength = 1, ErrorMessage = "The field must be no more than 17 characters")] 
        public string Chases { get; set; }
        [Required]
        public string VehicleName { get; set; }
        [StringLength(11, MinimumLength = 11, ErrorMessage = "field must be 11 number")]
        public string CustomerPhone { get; set; }
        public bool WithMaintenance { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Year { get; set; }
        //public long? DraftId  { get; set; }
        //public bool saveDraft { get; set; }
    }
}
