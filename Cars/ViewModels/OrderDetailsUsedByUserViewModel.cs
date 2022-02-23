using Cars.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.ViewModels
{
    public class OrderDetailsUsedByUserViewModel
    {
        public OrderDetails orderDetails { get; set; }
        public ApplicationUser user { get; set; }
    }
}
