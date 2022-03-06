using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Models
{
    public class BrandModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long ModelID { get; set; }
        public string Name { get; set; }
        public DateTime DTsCreate { get; set; }
        public long BrandID { get; set; }
        [ForeignKey("BrandID")]
        public Brand Brand { get; set; }
    
        public virtual List<ModelYear> ModelYears { get; set; }
    }
}
