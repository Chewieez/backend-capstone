using Microsoft.AspNetCore.Identity;
using RepPortal.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepPortal.Models.StoreViewModels
{
    public class CreateStoreViewModel
    {
        public List<ApplicationUser> Users { get; set; }

        public Store Store { get; set; }

        public string RepId { get; set; } 

        //private readonly UserManager<ApplicationUser> _userManager;

        // This task retrieves the currently authenticated user
        //private Task<ApplicationUser> GetCurrentUser() => _userManager.GetUserId();


        public CreateStoreViewModel(ApplicationDbContext ctx)
        {
            //_userManager = userManager;

        // get current user
        ////ApplicationUser user = await GetCurrentUserAsync();

            //this.Users = ctx.Users.Where(user => user.ro != ).ToList();
        }
    }
}
