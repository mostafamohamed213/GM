using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Models
{
    public class FinanceDocument
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long FinanceDocumentID { get; set; }
        public string Path { get; set; }
        public string Comment { get; set; }
        public long FinanceID { get; set; }
        [ForeignKey("FinanceID")]
        public Finance Finance { get; set; }
        [Required]
        public string SystemUserID { get; set; }
        public DateTime DTsCreate { get; set; }
    }
}
