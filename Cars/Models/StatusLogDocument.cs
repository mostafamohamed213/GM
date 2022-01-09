using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Models
{
    public class StatusLogDocument
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long StatusLogDocumentID { get; set; }
        [Required]
        public string Path { get; set; }
        public string Details { get; set; }
        public long OrderDetailsStatusLogID { get; set; }
        [ForeignKey("OrderDetailsStatusLogID")]
        public OrderDetailsStatusLog OrderDetailsStatusLog { get; set; }
        [Required]
        public string SystemUserID { get; set; }
        public DateTime DTsCreate { get; set; }
    }
}
