using Cars.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.ViewModels
{
    public class CreateQuotationViewModel
    {
        public long QuotationId { get; set; }
        public Quotation Quotation { get; set; }
        public List<OrderDetails> orderDetails { get; set; }
        public int status { get; set; }
    }
}
