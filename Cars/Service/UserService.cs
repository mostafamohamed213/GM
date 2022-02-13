using Cars.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Service
{
    public class UserService
    {
        public CarsContext _context { get; set; }

        public UserService(CarsContext carsContext)
        {
            _context = carsContext;
        }

        public async Task<IEnumerable<object>> GetAllDeliveryAsync()
        {
            var usersId = await _context.UserRoles.Where(c => c.RoleId == _context.Roles.FirstOrDefault(a => a.Name.ToLower() == "delivery").Id).Select(c => c.UserId).ToListAsync();
            if (usersId != null && usersId.Count() > 0)
            {
                var users = await _context.Users.Where(c => usersId.Contains(c.Id)).Select(c => new { ID = c.Id, Name = c.FirstName + " " + c.SeconedName }).ToListAsync();
                return users;
            }
            return null;
        }
    }
}
