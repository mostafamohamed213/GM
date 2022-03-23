using Cars.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.ViewModels
{
    public class LaborOrdersViewModel : OrderDetails
    {
        public bool IsSelected { get; set; }
    }
}
