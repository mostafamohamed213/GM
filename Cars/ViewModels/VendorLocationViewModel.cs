using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.ViewModels
{
    public class VendorLocationViewModel
    {
        [Required]
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string Description { get; set; }
        [Required]
        public string SystemUserCreate { get; set; }
        public DateTime DTsCreate { get; set; }

    }
}
