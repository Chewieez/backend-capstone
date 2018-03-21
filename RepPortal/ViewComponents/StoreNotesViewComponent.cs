using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepPortal.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using RepPortal.Models;
using RepPortal.Models.StoreNotesViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RepPortal.ViewComponents
{
    public class StoreNotesViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StoreNotesViewComponent(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // This task retrieves the currently authenticated user
        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        public async Task<IViewComponentResult> InvokeAsync(int storeId)
        {
            var user = await GetCurrentUserAsync();

            // create a new StoreNoteViewModel
            StoreNoteViewModel snvm = new StoreNoteViewModel()
            {
                // attach the current store id to the view model, to use to attach to a new store note when one is created
                CurrentStoreId = storeId,
                CurrentUser = user
            };

            // get StoreNotes from the database for the current store being viewed
            snvm.AllNotesForStore = await _context.StoreNote.Include("User").Include(sn => sn.Store).Where(sn => sn.Store.StoreId == storeId).OrderBy(sn => sn.DateCreated).ToListAsync();
                       
            // return notes
            return View(snvm);
        }

    }
}