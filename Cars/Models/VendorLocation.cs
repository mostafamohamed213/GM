using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Models
{
    public class VendorLocation
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int VendorLocationID { get; set; }
        [Required]
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string Description { get; set; }

    }
}
