using Cars.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Service
{
    public class WorkflowOrderDetailsLogsService
    {
        public CarsContext _context { get; set; }
        public WorkflowOrderDetailsLogsService(CarsContext carsContext)
        {
            _context = carsContext;
        }

        public async Task<int> AddAsync(WorkflowOrderDetailsLog model)
        {
            try
            {
                var result = await _context.WorkflowOrderDetailsLogs.AddAsync(model);
                return await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
