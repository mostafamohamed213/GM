using Cars.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.ViewModels
{
    public class QuotationViewModel
    {
        public Quotation quotation { get; set; }
        public string car { get; set; }
        public string Chases { get; set; }
    }
}
