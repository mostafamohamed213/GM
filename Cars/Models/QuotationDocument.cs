using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Models
{
    public class QuotationDocument
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long QuotationDocumentID { get; set; }
        public string Path { get; set; }
        public string Comment { get; set; }
        public long QuotationID { get; set; }
        [ForeignKey("QuotationID")]
        public Quotation Quotation { get; set; }
        [Required]
        public string SystemUserID { get; set; }
        public DateTime DTsCreate { get; set; }
    }
}
