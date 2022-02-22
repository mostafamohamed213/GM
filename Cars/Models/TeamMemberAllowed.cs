using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Models
{
    public class TeamMemberAllowed
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int ID { get; set; }
        public int TeamDurationID { get; set; }
        public Boolean isAssigned { get; set; }
        public string Roleid { get; set; }
        [ForeignKey("Userid")]
        public string Userid { get; set; }
        public virtual List<ApplicationUser> Users { get; set; }


        [ForeignKey("TeamDurationID")]
        public virtual TeamDuration TeamDuration { get; set; }
    }
}
