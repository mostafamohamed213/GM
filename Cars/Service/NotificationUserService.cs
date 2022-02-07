using Cars.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Service
{
    public class NotificationUserService
    {
        public CarsContext _context { get; set; }
        public NotificationUserService(CarsContext carsContext)
        {
            _context = carsContext;
        }

        public async Task<IEnumerable<NotificationUser>> GetAllByUserIDAsync(string userID)
        {
            var result = await _context.NotificationUser.Where(c => c.UserID == userID).Include(x => x.Notification).ToListAsync();
            return result;
        }
    }
}
