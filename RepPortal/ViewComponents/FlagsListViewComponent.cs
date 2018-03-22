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
            //var UserStores = new List<Store>();
            var RetStoreFlags = new List<StoreFlag>();
            RetStoreFlags = await _context.StoreFlag.Include(sf => sf.Flag).Include(sf => sf.Store).ThenInclude(s => s.SalesRep).ToListAsync();

            // create list of view models
            List<StoreFlagViewModel> sfvmList = new List<StoreFlagViewModel>();

            foreach (StoreFlag sf in RetStoreFlags)
            {

                //create a new view model instance
                StoreFlagViewModel sfvm = new StoreFlagViewModel();
                sfvm.Store = sf.Store;
                sfvm.Flag = sf.Flag;

                // add the StoreFlagViewModel to the list
                sfvmList.Add(sfvm);
            }

            return sfvmList;
        }
    }
}