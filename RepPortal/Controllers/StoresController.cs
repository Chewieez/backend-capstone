using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RepPortal.Data;
using RepPortal.Models;
using RepPortal.Models.StoreViewModels;

namespace RepPortal.Controllers
{
    public class StoresController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StoresController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        // This task retrieves the currently authenticated user
        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        // GET: Stores
        public async Task<IActionResult> Index()
        {
            // get current user
            ApplicationUser user = await GetCurrentUserAsync();

            // Get the roles for the user
            var roles = await _userManager.GetRolesAsync(user);

            // create a list of stores
            var stores = new List<Store>();

            // check if the user is an Administrator
            if (roles.Contains("Administrator"))
            {
                // retrieve all stores to display (for site administrator)
                stores = _context.Store.Include("SalesRep").Include("State").Include("Status").ToList();
            }
            else
            {
                // retrieve only matching stores where current user is the Sales Rep attached to the store
                stores = _context.Store.Include("State").Include("Status").Where(s => s.SalesRep == user).ToList();
            }

            
            return View(stores);

            //var applicationDbContext = _context.Store.Include(s => s.State).Include(s => s.Status);
            //return View(await applicationDbContext.ToListAsync());
        }

        // GET: Stores/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var store = await _context.Store
                .Include(s => s.State)
                .Include(s => s.Status)
                .SingleOrDefaultAsync(m => m.StoreId == id);
            if (store == null)
            {
                return NotFound();
            }

            return View(store);
        }

        // GET: Stores/Create
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create()
        {
            CreateStoreViewModel createStoreViewModel = new CreateStoreViewModel();

            var CurrentUser = await GetCurrentUserAsync();

            ViewBag.SalesReps = _context.Users.OrderBy(u => u.FirstName)
                .Select(u => new SelectListItem() { Text = $"{ u.FirstName} { u.LastName}", Value = u.Id}).ToList();

            ViewData["StateId"] = new SelectList(_context.State.OrderBy( s=> s.Name), "StateId", "Name");
            ViewData["StatusId"] = new SelectList(_context.Status, "StatusId", "Name");
            
            return View(createStoreViewModel);
        }

        // POST: Stores/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create(CreateStoreViewModel storeModel)
        {
            ModelState.Remove("store.user");


            if (ModelState.IsValid)
            {
                // Get the current user
                ApplicationUser user = await GetCurrentUserAsync();

                // find making user for SalesRep in system
                ApplicationUser SalesRep = _context.Users.Single(u => u.Id == storeModel.SalesRepId);

                // store the sales rep on the store
                storeModel.Store.SalesRep = SalesRep;
                // Add current user to store listing
                storeModel.Store.User = user;

                // save store to context
                _context.Add(storeModel.Store);
                // save context file to database
                await _context.SaveChangesAsync();
                // redirect user to list of all stores
                return RedirectToAction(nameof(Index));
            }

            CreateStoreViewModel createStoreViewModel = new CreateStoreViewModel();
            // get current user
            var CurrentUser = await GetCurrentUserAsync();
            // populate SaleReps dropdown list by retrieving all users that are not the current user. 
            // Only administrator will be allowed to create a new store listing, so they will be current User.
            ViewBag.SalesReps = _context.Users.OrderBy(u => u.FirstName)
                .Select(u => new SelectListItem() { Text = u.FirstName, Value = u.Id }).ToList();
            ViewData["StateId"] = new SelectList(_context.State, "StateId", "Name", storeModel.Store.StateId);
            ViewData["StatusId"] = new SelectList(_context.Status, "StatusId", "Color", storeModel.Store.StatusId);

            return View(createStoreViewModel);
        }

        // GET: Stores/Edit/5
        //[Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var store = await _context.Store.SingleOrDefaultAsync(m => m.StoreId == id);
            if (store == null)
            {
                return NotFound();
            }

            CreateStoreViewModel createStoreViewModel = new CreateStoreViewModel();

            createStoreViewModel.Store = store;

            var CurrentUser = await GetCurrentUserAsync();

            ViewBag.SalesReps = _context.Users.Where(user => user != CurrentUser)
                .Select(u => new SelectListItem() { Text = $"{ u.FirstName} { u.LastName}", Value = u.Id }).ToList();

            ViewData["StateId"] = new SelectList(_context.State, "StateId", "Name", store.StateId);
            ViewData["StatusId"] = new SelectList(_context.Status, "StatusId", "Color", store.StatusId);
            return View(createStoreViewModel);
        }

        // POST: Stores/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StoreId,SalesRepId,Name,StreetAddress,City,StateId,Zipcode,PhoneNumber,WebsiteURL,ContactName,DateAdded,DateCreated,StatusId,DateClosed,LasterOrderTotal,LastOrderDate,LastOrderShipDate,LastOrderPaidDate,Lat,Long")] Store store)
        {
            if (id != store.StoreId)
            {
                return NotFound();
            }

            ModelState.Remove("store.User");

            if (ModelState.IsValid)
            {
                var user = await GetCurrentUserAsync();

                store.User = user;

                try
                {
                    _context.Update(store);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StoreExists(store.StoreId))
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
            CreateStoreViewModel createStoreViewModel = new CreateStoreViewModel();

            createStoreViewModel.Store = store;

            ViewBag.SalesReps = _context.Users.OrderBy(u => u.FirstName)
                .Select(u => new SelectListItem() { Text = $"{ u.FirstName} { u.LastName}", Value = u.Id }).ToList();


            ViewData["StateId"] = new SelectList(_context.State, "StateId", "Name", store.StateId);
            ViewData["StatusId"] = new SelectList(_context.Status, "StatusId", "Color", store.StatusId);
            return View(createStoreViewModel);
        }

        // GET: Stores/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var store = await _context.Store
                .Include(s => s.State)
                .Include(s => s.Status)
                .SingleOrDefaultAsync(m => m.StoreId == id);
            if (store == null)
            {
                return NotFound();
            }

            return View(store);
        }

        // POST: Stores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var store = await _context.Store.SingleOrDefaultAsync(m => m.StoreId == id);
            _context.Store.Remove(store);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StoreExists(int id)
        {
            return _context.Store.Any(e => e.StoreId == id);
        }

        // GET: Stores
        // This is used in javascript to retrieve all the stores that are attached to the current user, to be added to the Google Map view
        public async Task<IActionResult> StoresList()
        {
            // get current user
            ApplicationUser user = await GetCurrentUserAsync();

            // Get the roles for the user
            var roles = await _userManager.GetRolesAsync(user);

            // create a list of stores
            var stores = new List<Store>();

            // check if the user is an Administrator
            if (roles.Contains("Administrator"))
            {
                // retrieve all stores to display on map (for site administrator)
                stores = _context.Store.Include("State").Include("Status").ToList();
            } else
            {
                // retrieve only matching stores where current user is the Sales Rep attached to the store
                stores = _context.Store.Include("State").Include("Status").Where(s => s.SalesRep == user).ToList();     
            }

            // update the status of all stores by checking their last order date versus the current date
            DateTime currentDate = DateTime.Now;

            foreach (Store s in stores)
            {
                // calculate the time difference between current date and the store's last order date
                TimeSpan interval = currentDate - s.LastOrderDate;
                // store the store's current status
                int storeStatusId = s.StatusId;
                // check if the difference is greater than 6 months or 12 months
                if ((interval.Days / 29) > 12)
                {
                    storeStatusId = 3;
                }
                else if ((interval.Days / 29) > 6)
                {
                    storeStatusId = 2;
                }
                
                // if status has changed, save new status and write to database
                if (storeStatusId != s.StatusId)
                {
                    s.StatusId = storeStatusId;
                    _context.Update(s);
                    _context.SaveChanges();
                }
            }

            Console.WriteLine(stores);
            // return a json formatted response to be used in javascript ajax call
            return Json(stores);
        }
    }
}
