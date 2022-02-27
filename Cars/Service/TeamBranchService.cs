using Cars.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Service
{
    public class TeamBranchService
    {
        public CarsContext db { get; set; }
        public TeamBranchService(CarsContext carsContext)
        {
            db = carsContext;
        }
        public List<TeamDuration> GetTeamBranches()
        {
            var orders = db.TeamDurations.ToList();
            return orders;
          
        }

        public void EditTeamBranches(int TeamDurationID,  string Roleid, double duration,  string[] unasign, string[] asign)
        {


            var EditTeam = db.TeamDurations.Where(t => t.TeamDurationID == TeamDurationID).FirstOrDefault();
            EditTeam.Duration = duration;
            db.SaveChanges();


            var TeamAllowesMembers = db.TeamMemberAlloweds.Where(t => t.TeamDurationID == TeamDurationID).ToList();


            db.TeamMemberAlloweds.RemoveRange(TeamAllowesMembers);
            db.SaveChanges();
            if (unasign.Length > 0)
            {
                foreach (var itm in unasign)
                {
                    var teammember = new TeamMemberAllowed();
                    teammember.isAssigned = false;
                    teammember.Roleid = Roleid;
                    teammember.TeamDurationID = TeamDurationID;
                    teammember.Userid = itm;
                    db.TeamMemberAlloweds.Add(teammember);
                    db.SaveChanges();
                }

            }


            if (asign.Length > 0)
            {

                foreach (var itm in asign)
                {
                    var teammember = new TeamMemberAllowed();
                    teammember.isAssigned = true;
                    teammember.Roleid = Roleid;
                    teammember.TeamDurationID = TeamDurationID;
                    teammember.Userid = itm;
                    db.TeamMemberAlloweds.Add(teammember);
                    db.SaveChanges();


                }
            }


        }
    }
}
