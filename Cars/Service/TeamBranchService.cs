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
    }
}
