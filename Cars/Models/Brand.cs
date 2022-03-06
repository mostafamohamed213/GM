using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Models
{
    public class Brand
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long BrandID { get; set; }
        public string Name { get; set; }
        public DateTime DTsCreate { get; set; }
        public virtual List<BrandModel> BrandModels { get; set; }

    }
}
