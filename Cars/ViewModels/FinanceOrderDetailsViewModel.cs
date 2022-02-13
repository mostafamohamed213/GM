using Cars.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.ViewModels
{
    public class FinanceOrderDetailsViewModel
    {
        public OrderDetails model { get; set; }
        public int Status { get; set; }
    }
    public class SalesOrderDetailsViewModel
    {
        public List<OrderDetails> orderDetails { get; set; }
        public int status { get; set; }
    }
}
