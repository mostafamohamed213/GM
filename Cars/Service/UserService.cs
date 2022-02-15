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

        /// <summary>
        /// Get All Users By Role (ID, Name)
        /// </summary>
        /// <param name="role"></param>
        /// <returns>Return List of users (ID, Name)</returns>
        public async Task<IEnumerable<object>> GetAllByRoleAsync(string role)
        {
            var usersId = await _context.UserRoles.Where(c => c.RoleId == _context.Roles.FirstOrDefault(a => a.Name.ToLower() == role).Id).Select(c => c.UserId).ToListAsync();
            if (usersId != null && usersId.Count() > 0)
            {
                var users = await _context.Users.Where(c => usersId.Contains(c.Id)).Select(c => new { ID = c.Id, Name = c.FirstName + " " + c.SeconedName }).ToListAsync();
                return users;
            }
            return null;
        }

        /// <summary>
        /// Get List of ApplicationUser by Role 
        /// </summary>
        /// <param name="role"></param>
        /// <returns>Return List of ApplicationUser</returns>
        public async Task<IEnumerable<ApplicationUser>> GetByRoleAsync(string role)
        {
            var usersId = await _context.UserRoles.Where(c => c.RoleId == _context.Roles.FirstOrDefault(a => a.Name.ToLower() == role).Id).Select(c => c.UserId).ToListAsync();
            if (usersId != null && usersId.Count() > 0)
            {
                var users = await _context.Users.Where(c => usersId.Contains(c.Id)).Include(x=>x.Id).Include(x => x.Email).ToListAsync();
                return users;
            }
            return null;
        }
    }
}
