using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.ViewModels
{
    public class OrderDetailsViewModel
    {
        public long OrderDetailsID { get; set; }
        public long OrderID { get; set; }
        public string Items { get; set; }
        public int Quantity { get; set; }
        public bool? IsApproved { get; set; }
        public string type { get; set; }
        public string BranchIP { get; set; }
        public string PartNumber { get; set; }
        public decimal? Price { get; set; }
        public string Comments { get; set; }
        public int workflowID { get; set; }
        public int statusID { get; set; }
        public string prefix { get; set; }

    }
}
