using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Models
{
    public class TeamDuration
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int TeamDurationID { get; set; }
        public string Roleid { get; set; }

        public double Duration { get; set; }
        public Boolean isAssigned { get; set; }

        public string Userid { get; set; }


        [ForeignKey("Roleid")]
        public virtual IdentityRole IdentityRole { get; set; }
    }
}
