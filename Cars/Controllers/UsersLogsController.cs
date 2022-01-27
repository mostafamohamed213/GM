using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cars.Models;

namespace Cars.Controllers
{
    public class UsersLogsController : Controller
    {
        private readonly CarsContext _context;

        public UsersLogsController(CarsContext context)
        {
            _context = context;
        }

        // GET: UsersLogs
        public async Task<IActionResult> Index(string sortOrder,
            string currentFilter,
            string searchString,
             string currentFilter2,
            string searchString2,
            DateTime? searchString3,
            DateTime? searchString4,
               DateTime? currentFilter3,
                  DateTime? currentFilter4,
            int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            //  ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            //ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null || searchString2 != null || searchString3 != null || searchString4 != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
                searchString2 = currentFilter2;
                searchString3 = currentFilter3;
                searchString4 = currentFilter4;
            }

            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentFilter2"] = searchString2;

            ViewData["CurrentFilter3"] = searchString3;
            ViewData["CurrentFilter4"] = searchString4;


            var usersLogs = from s in _context.UsersLogs
                            select s;


            if (!String.IsNullOrEmpty(searchString))
            {
                usersLogs = usersLogs.Where(s =>  s.UserCity.ToLower().Trim().Contains(searchString.ToLower().Trim()));
            }
            if (!String.IsNullOrEmpty(searchString2))
            {
                usersLogs = usersLogs.Where(s => s.User.UserName.ToLower().Trim().Contains(searchString2.ToLower().Trim()));
            }

            if (searchString3 !=null)
            {
                usersLogs = usersLogs.Where(s => s.CreateDts>=searchString3);
            }

            if (searchString4 != null)
            {
                usersLogs = usersLogs.Where(s => s.CreateDts <= searchString4);
            }

            int count = usersLogs.Count();
            int pageSize = 20;
            int TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            ViewBag.pages = TotalPages;
            ViewBag.currentpage = pageNumber ?? 1;
            return View(await PaginatedList<UsersLogs>.CreateAsync(usersLogs.Include(s=>s.User).OrderByDescending(a=>a.CreateDts).AsNoTracking(), pageNumber ?? 1, pageSize));
         
        }

        // GET: UsersLogs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usersLogs = await _context.UsersLogs
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.UsersLogsID == id);
            if (usersLogs == null)
            {
                return NotFound();
            }

            return View(usersLogs);
        }

        // GET: UsersLogs/Create
        public IActionResult Create()
        {
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: UsersLogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UsersLogsID,UserIP,UserCity,UserRegion,CreateDts,UserID")] UsersLogs usersLogs)
        {
            if (ModelState.IsValid)
            {
                _context.Add(usersLogs);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id", usersLogs.UserID);
            return View(usersLogs);
        }

        // GET: UsersLogs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usersLogs = await _context.UsersLogs.FindAsync(id);
            if (usersLogs == null)
            {
                return NotFound();
            }
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id", usersLogs.UserID);
            return View(usersLogs);
        }

        // POST: UsersLogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UsersLogsID,UserIP,UserCity,UserRegion,CreateDts,UserID")] UsersLogs usersLogs)
        {
            if (id != usersLogs.UsersLogsID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usersLogs);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsersLogsExists(usersLogs.UsersLogsID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id", usersLogs.UserID);
            return View(usersLogs);
        }

        // GET: UsersLogs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usersLogs = await _context.UsersLogs
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.UsersLogsID == id);
            if (usersLogs == null)
            {
                return NotFound();
            }

            return View(usersLogs);
        }

        // POST: UsersLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usersLogs = await _context.UsersLogs.FindAsync(id);
            _context.UsersLogs.Remove(usersLogs);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsersLogsExists(int id)
        {
            return _context.UsersLogs.Any(e => e.UsersLogsID == id);
        }
    }
}
