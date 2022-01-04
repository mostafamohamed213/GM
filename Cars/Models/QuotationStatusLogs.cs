using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Models
{
    public class QuotationStatusLogs
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long QuotationStatusLogID { get; set; }
        public string Detatils { get; set; }
        public int StatusID { get; set; }
        [ForeignKey("StatusID")]
        public Status Status { get; set; }
        public long QuotationID { get; set; }
        [ForeignKey("QuotationID")]
        public Quotation Quotation { get; set; }
        [Required]
        public string SystemUserID { get; set; }
        public DateTime DTsCreate { get; set; }
    }
}
