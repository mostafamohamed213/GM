using Cars.Models;
using Cars.Service;
using Cars.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Controllers
{
    public class TeamDurationController : Controller
    {
        public TeamBranchService services { get; set; }
        public CarsContext db { get; set; }
        public TeamDurationController(TeamBranchService _service, CarsContext _db)
        {
            services = _service;
            db = _db;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ViewTeamDuration()
        {
            try
            {
                var TeamdurationRole = services.GetTeamBranches();
                List<string> RoleName = new List<string>();

                var roleData = new RoleData();
                roleData.TeamDurationID = new List<int>();
                roleData.Duration = new List<double>();
                roleData.RoleName = new List<string>();
                roleData.RoleID = new List<string>();

                ViewData["Count"] = TeamdurationRole.Count;

                if (TeamdurationRole != null)
                {
                    foreach (var itm in TeamdurationRole)
                    {
                        var Rolename = db.Roles.Where(r => r.Id == itm.Roleid).Select(r => r.Name).FirstOrDefault();
                        roleData.TeamDurationID.Add(itm.TeamDurationID);
                        roleData.RoleName.Add(Rolename);
                        roleData.Duration.Add(itm.Duration);
                        roleData.RoleID.Add(itm.Roleid);
                    }
                }


                return View(roleData);
            }
            catch(Exception)
            {
                return View("_CustomError");
            }
        }
        public IActionResult EditTeamDuration([FromQuery] int TeamDurationID,[FromQuery] string RoleID)
        {
            try
            {
                var TeamDuration = db.TeamDurations.Where(t => t.TeamDurationID == TeamDurationID).FirstOrDefault();
                var TeamName = db.Roles.Where(r => r.Id == RoleID).Select(r => r.Name).FirstOrDefault();
                ViewData["TeamName"] = TeamName;

                return View(TeamDuration);
            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }
    }
}
