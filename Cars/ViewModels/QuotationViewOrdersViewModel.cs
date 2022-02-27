using Cars.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.ViewModels
{
    public class QuotationViewOrdersViewModel
    {
        public Order order { get; set; }
        public decimal TotalPrice { get; set; }

    }
}
