using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Controllers
{
    public class TeamDuration : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ViewTeamDuration()
        {
            return View();
        }
        public IActionResult EditTeamDuration()
        {
            return View();
        }
    }
}
