using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Models
{
    public class UserBranchViewModel
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        [Required]
        public int BranchID { get; set; } 
        public string BranchName { get; set; }
        public int UserBranchID { get; set; }
        public bool IsActive { get; set; }
        public DateTime DTsCreate { get; set; }
        public DateTime? DTsEnd { get; set; }
    }
}
