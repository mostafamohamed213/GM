using Cars.Controllers;
using Cars.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Service
{
    public class UserBranchService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public CarsContext _context { get; set; }
        public UserBranchService(CarsContext carsContext, UserManager<ApplicationUser> userManager)
        {
            _context = carsContext;
            _userManager = userManager;
        }

        public async Task<PaginatedList<UserBranchViewModel>> GetAllAsync(string currentFilter, string searchString, int? pageNumber, int pageSize)
        {
            try
            {
                if (searchString != null)
                    pageNumber = 1;
                else
                    searchString = currentFilter;

                var users = await _userManager.Users.Include(c => c.UserBranches).ThenInclude(c => c.Branch).AsNoTracking().ToListAsync();

                var usersBranches = users.Select(x =>
                {
                    var userbranch = x.UserBranches.Where(y => y.IsActive == true).FirstOrDefault();
                    var user = new UserBranchViewModel()
                    {
                        UserId = x.Id,
                        Email = x.Email,
                        FirstName = x.FirstName,
                        Username = x.UserName
                    };
                    if (userbranch != null)
                    {
                        user.BranchID = userbranch.BranchID;
                        user.UserBranchID = userbranch.UserBranchID;
                        user.BranchName = userbranch.Branch.Name;
                    };
                    return user;
                });

                //Search by Username, BranchName, FirstName, Email
                if (!String.IsNullOrEmpty(searchString))
                    usersBranches = usersBranches.Where(s => s.BranchName.ToLower().Trim().Contains(searchString.ToLower().Trim())
                     || s.Username.ToLower().Trim().Contains(searchString.ToLower().Trim())
                     || s.FirstName.ToLower().Trim().Contains(searchString.ToLower().Trim())
                     || s.Email.ToLower().Trim().Contains(searchString.ToLower().Trim())).OrderBy(x => x.FirstName).ThenBy(x => x.Username).ThenBy(x => x.Email).ThenBy(x => x.BranchName).ToList();
                return PaginatedList<UserBranchViewModel>.CreateAsync(usersBranches, pageNumber ?? 1, pageSize);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private async Task<IEnumerable<UserBranchModel>> GetAllUserBranchesAsync(string userID)
        {
            try
            {
                return await _context.UserBranches.Where(x => x.UserID == userID).ToListAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        public async Task<UserBranchViewModel> GetByIDAsync(string userID, int? userBranchID)
        {
            try
            {
                UserBranchViewModel userBranch = new UserBranchViewModel();
                if (userBranchID == null || userBranchID <= 0)
                {
                    userBranch = await _userManager.Users.Where(x => x.Id == userID).Select(x => new UserBranchViewModel()
                    {
                        UserId = x.Id,
                        Username = x.UserName,
                        Email = x.Email,
                        FirstName = x.FirstName
                    }).FirstOrDefaultAsync();
                }
                else
                {
                    userBranch = await _context.UserBranches.Include(x => x.Branch).Include(x => x.User).Where(x => x.User.Id == userID && x.UserBranchID == userBranchID.Value).Select(x => new UserBranchViewModel()
                    {
                        UserId = x.User.Id,
                        Username = x.User.UserName,
                        Email = x.User.Email,
                        FirstName = x.User.FirstName,
                        UserBranchID = x.UserBranchID,
                        BranchID = x.BranchID,
                        BranchName = x.Branch.Name,
                        DTsCreate = x.DTsCreate,
                        DTsEnd = x.DTsEnd,
                        IsActive = x.IsActive
                    }).FirstOrDefaultAsync();
                }
                return userBranch;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<int> AddAsync(string userID, int branchID)
        {
            try
            {
                UserBranchModel model = new UserBranchModel()
                {
                    IsActive = true,
                    UserID = userID,
                    BranchID = branchID,
                    DTsCreate = DateTime.UtcNow
                };
                var result = await _context.UserBranches.AddAsync(model);
                return await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private async Task<int> UpdateAsync(UserBranchModel model)
        {
            try
            {
                var result = _context.UserBranches.Update(model);
                return await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private async Task<int> UpdateAsync(IEnumerable<UserBranchModel> model)
        {
            try
            {
                _context.UserBranches.UpdateRange(model);
                return await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> ChangeUserBranchAsync(UserBranchViewModel model)
        {
            try
            {
                var userBranches = await GetAllUserBranchesAsync(model.UserId);

                //Get Current User Branch 
                var activeUserBranch = userBranches.Where(x => x.IsActive == true).FirstOrDefault();
                var currentUserBranch = userBranches.Where(x => x.UserBranchID == model.UserBranchID).FirstOrDefault();
                if (currentUserBranch == null)
                    return 0;
                //Update Is Active
                if (currentUserBranch.BranchID == model.BranchID)
                {
                    if (currentUserBranch.IsActive = true && model.IsActive == false)
                        currentUserBranch.DTsEnd = DateTime.UtcNow;
                    else if (currentUserBranch.IsActive = false && model.IsActive == true)
                    {
                        currentUserBranch.DTsEnd = null;
                        currentUserBranch.DTsCreate = DateTime.UtcNow;
                        if (activeUserBranch != null)
                        {
                            activeUserBranch.IsActive = false;
                            activeUserBranch.DTsEnd = DateTime.UtcNow;
                        }
                    }
                    currentUserBranch.IsActive = model.IsActive;
                    if (activeUserBranch != null && activeUserBranch.UserBranchID != currentUserBranch.UserBranchID)
                        return await UpdateAsync(new List<UserBranchModel>()
                    {
                        currentUserBranch,
                    activeUserBranch
                    });
                    else
                        return await UpdateAsync(currentUserBranch);
                }
                //Change Branch, And set Current Active Branch to inActive
                else
                {
                    if (activeUserBranch != null)
                    {
                        activeUserBranch.IsActive = false;
                        activeUserBranch.DTsEnd = DateTime.UtcNow;
                    }
                    var UpdateUserActiceBranch = await UpdateAsync(activeUserBranch);
                    if (UpdateUserActiceBranch <= 0)
                        return 0;
                    return await AddAsync(model.UserId, model.BranchID);
                }
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
