using Cars.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.ViewModels
{
    public class DisplayQuotationViewModel
    {
        public Quotation quotation { get; set; }
        public Vehicle vehicle { get; set; }
       
    }
}
