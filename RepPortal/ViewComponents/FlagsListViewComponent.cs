using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepPortal.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using RepPortal.Models;
using RepPortal.Models.StoreViewModels;

namespace RepPortal.ViewComponents
{
    public class FlagsListViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FlagsListViewComponent(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // This task retrieves the currently authenticated user
        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        public async Task<IViewComponentResult> InvokeAsync()
        {

            // get flags
            var Flags = await GetFlagsAsync();

            // return flags
            return View(Flags); 
        }

        private async Task<List<StoreFlagViewModel>> GetFlagsAsync()
        {
            // get user
            var user = await GetCurrentUserAsync();

            // create a new list to hold stores
            var UserStores = new List<Store>();

            // check if user is an Administrator, if they are, retrieve all the stores in database, if they are not, 
            // just retrieve stores from db that are in user's (sales rep) territory
            if (User.IsInRole("Administrator"))
            {
                UserStores = await _context.Store.Include("SalesRep").ToListAsync();
            } else
            {
                UserStores = await _context.Store.Where(s => s.SalesRep == user).ToListAsync();
            }

            // create list of view models
            List<StoreFlagViewModel> sfvmList = new List<StoreFlagViewModel>();

            // iterate through stores and get any flags attached
            foreach (Store s in UserStores)
            {
                // create a new view model instance
                StoreFlagViewModel sfvm = new StoreFlagViewModel();

                var Flag = (from f in _context.Flag
                            join sf in _context.StoreFlag
                            on f.FlagId equals sf.FlagId
                            join st in _context.Store
                            on sf.StoreId equals st.StoreId
                            where st.StoreId == s.StoreId
                            select f).SingleOrDefaultAsync();

                // attach store and flag to view model
                sfvm.Store = s;
                sfvm.Flag = await Flag;

                // add view model to list of storeflagviewmodels
                sfvmList.Add(sfvm);
            }

            return sfvmList;
        }
    }
}