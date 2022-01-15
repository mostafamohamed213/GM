using Cars.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.ViewModels
{
    public class CancelOrderDetailsViewModel
    {
        public OrderDetails OrderDetails { get; set; }
        [Required]
        public string Reason { get; set; }
        public string Detatils { get; set; }
        public long OrderDetailsID { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public IFormFile[] FormFiles  { get; set; }
    }
}
