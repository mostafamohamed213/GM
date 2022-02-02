using Cars.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Service
{
    public class NotificationService
    {
        public CarsContext _context { get; set; }
        public NotificationService(CarsContext carsContext)
        {
            _context = carsContext;
        }

        public async Task<Notification> AddAsync(Notification model)
        {
            var result = await _context.Notification.AddAsync(model);
            if (await _context.SaveChangesAsync() > 0)
                return result.Entity;
            else
                return null;
        }
    }
}
