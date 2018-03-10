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

            //only lists stores that the current user is attached to


            var stores = _context.Store.Include(s => s.State).Include(s => s.Status).Where(s => s.User == user);
            return View(await stores.ToListAsync());

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

            ViewBag.SalesReps = _context.Users.Where(user => user != CurrentUser)
                .Select(u => new SelectListItem() { Text = u.FirstName, Value = u.Id}).ToList();

            ViewData["StateId"] = new SelectList(_context.State, "StateId", "Name");
            ViewData["StatusId"] = new SelectList(_context.Status, "StatusId", "Name");
            
            return View(createStoreViewModel);
        }

        // POST: Stores/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create([Bind("StoreId,SalesRepId,Name,StreetAddress,City,StateId,Zipcode,PhoneNumber,WebsiteURL,DateAdded,DateCreated,StatusId,DateClosed,LasterOrderTotal,LastOrderDate,LastOrderShipDate,LastOrderPaidDate,Lat,Long")] Store store)
        {
            ModelState.Remove("store.user");


            if (ModelState.IsValid)
            {
                // Get the current user
                ApplicationUser user = await GetCurrentUserAsync();

                //ApplicationUser user = _context.Users.Where(u => u.Id == CSViewModel.RepId).SingleOrDefault();

                // Add current user to store listing
                store.User = user;

                // save store to context
                _context.Add(store);
                // save context file to database
                await _context.SaveChangesAsync();
                // redirect user to list of all stores
                return RedirectToAction(nameof(Index));
            }

            CreateStoreViewModel createStoreViewModel = new CreateStoreViewModel();

            var CurrentUser = await GetCurrentUserAsync();
            
            ViewBag.SalesReps = _context.Users.Where(user => user != CurrentUser)
                .Select(u => new SelectListItem() { Text = u.FirstName, Value = u.Id }).ToList();
            ViewData["StateId"] = new SelectList(_context.State, "StateId", "Name", store.StateId);
            ViewData["StatusId"] = new SelectList(_context.Status, "StatusId", "Color", store.StatusId);

            return View(createStoreViewModel);
        }

        // GET: Stores/Edit/5
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
            ViewData["StateId"] = new SelectList(_context.State, "StateId", "Name", store.StateId);
            ViewData["StatusId"] = new SelectList(_context.Status, "StatusId", "Color", store.StatusId);
            return View(store);
        }

        // POST: Stores/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StoreId,Name,StreetAddress,City,StateId,Zipcode,PhoneNumber,WebsiteURL,DateAdded,DateCreated,StatusId,DateClosed,LasterOrderTotal,LastOrderDate,LastOrderShipDate,LastOrderPaidDate,Lat,Long")] Store store)
        {
            if (id != store.StoreId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
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
            ViewData["StateId"] = new SelectList(_context.State, "StateId", "Name", store.StateId);
            ViewData["StatusId"] = new SelectList(_context.Status, "StatusId", "Color", store.StatusId);
            return View(store);
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
            // string user = _userManager.GetUserName(HttpContext.User);
            ApplicationUser user = await GetCurrentUserAsync();

            //only lists stores that the current user is attached to
            var stores = _context.Store.Include("State").Include("Status").Where(s => s.SalesRepId == user.Id).ToList();
            Console.WriteLine(stores);
            // return a json formatted response to be used in javascript
            return Json(stores);
        }
    }
}
