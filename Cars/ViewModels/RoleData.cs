using Cars.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.ViewModels
{
    public class RoleData
    {
        public List<int> TeamDurationID { set; get; }
        public List<string> RoleID { set; get; }
        public List<Double> Duration { set; get; }

        public List<String> RoleName { set; get; }
    }
}
