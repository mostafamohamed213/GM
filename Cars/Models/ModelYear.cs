using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Models
{
    public class ModelYear
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long ModelYearID { get; set; }
        public string Year { get; set; }
        public DateTime DTsCreate { get; set; }
        public long ModelID { get; set; }
        [ForeignKey("ModelID")]
        public BrandModel Model { get; set; }
    }
}
