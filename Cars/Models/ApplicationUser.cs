using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string SeconedName { get; set; }

        public string LocationIP { get; set; }

        public long Mobile { get; set; }

        public long Whatsapp { get; set; }
        public virtual ICollection<UsersLogs> UsersLogs { get; set; }
        public virtual ICollection<UserBranchModel> UserBranches { get; set; }
        public virtual ICollection<NotificationUser> UserNotifications { get; set; }
        public virtual ICollection<OrderDetails> DeliveryOrders { get; set; }
    }
}
