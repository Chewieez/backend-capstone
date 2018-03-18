﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RepPortal.Data;
using RepPortal.Models;
using RepPortal.Models.StoreViewModels;
using CsvHelper.Configuration;
using System.Text.RegularExpressions;

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
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            

            // get current user
            ApplicationUser user = await GetCurrentUserAsync();

            // Get the roles for the user
            var roles = await _userManager.GetRolesAsync(user);

            // create a list of stores
            // by default, only retrieve matching stores where current user is the Sales Rep attached to the store
            var stores = _context.Store.Include("SalesRep").Include("State").Include("Status").Where(s => s.SalesRep == user);

            // check if the user is an Administrator
            if (roles.Contains("Administrator"))
            {
                // retrieve all stores to display (for site administrator)
                stores = _context.Store.Include("SalesRep").Include("State").Include("Status");
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                stores = stores.Where(s => s.Name.Contains(searchString) || s.Status.Name.Contains(searchString));
            }

            

            ViewData["OrderDateSortParm"] = String.IsNullOrEmpty(sortOrder) ? "Date" : "";
            ViewData["NameSortParm"] = sortOrder == "name" ? "name_desc" : "Name";
            ViewData["StatusSortParm"] = sortOrder == "Status" ? "status_desc" : "Status";


            switch (sortOrder)
            {
                case "Name":
                    stores = stores.OrderBy(s => s.Name);
                    break;
                case "name_desc":
                    stores = stores.OrderByDescending(s => s.Name);
                    break;
                case "Date":
                    stores = stores.OrderBy(s => s.LastOrderDate);
                    break;
                case "date_desc":
                    stores = stores.OrderByDescending(s => s.LastOrderDate);
                    break;
                case "Status":
                    stores = stores.OrderBy(s => s.StatusId);
                    break;
                case "status_desc":
                    stores = stores.OrderByDescending(s => s.StatusId);
                    break;
                default:
                    stores = stores.OrderByDescending(s => s.LastOrderDate);
                    break;
            }

            // create a iQueryable collection of store view models
            List<StoreListViewModel> StoresViewModels = new List<StoreListViewModel>();

            var RetrievedStores = await stores.ToListAsync();

            foreach (Store s in RetrievedStores)
            {
                // create a new view model instance
                StoreListViewModel slvm = new StoreListViewModel();

                // find any flags for the store
                var flag = await _context.StoreFlag.Include("Flag").Where(f => f.StoreId == s.StoreId).SingleOrDefaultAsync();
                // attach flag info to the view model
                slvm.Flag1 = flag;
                // attach the store to the view model
                slvm.Store = s;
                // add the view model to the StoresViewModels list
                StoresViewModels.Add(slvm);

            }

            return View(StoresViewModels);

        }

        // GET: Stores/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            // create a new view model instance
            StoreDetailViewModel sdvm = new StoreDetailViewModel();

            // find any flags for the store
            var flag = await _context.StoreFlag.Include("Flag").Where(f => f.StoreId == id ).SingleOrDefaultAsync();
            // attach flag info to the view model
            sdvm.Flag1 = flag;

            if (id == null)
            {
                return NotFound();
            }

            var store = await _context.Store
                .Include(s => s.SalesRep)
                .Include(s => s.State)
                .Include(s => s.Status)
                .SingleOrDefaultAsync(m => m.StoreId == id);
            if (store == null)
            {
                return NotFound();
            }

            // attach the store to the view model
            sdvm.Store = store;

            // return the view model
            return View(sdvm);
        }

        // GET: Stores/Create
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create()
        {
            CreateStoreViewModel createStoreViewModel = new CreateStoreViewModel();

            

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

                // find matching user for SalesRep in system
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

            var store = await _context.Store.Include("User").Include("SalesRep").Include("State").Include("Status").SingleOrDefaultAsync(m => m.StoreId == id);
            if (store == null)
            {
                return NotFound();
            }

            CreateStoreViewModel createStoreViewModel = new CreateStoreViewModel();
            if (store.SalesRep != null)
            {
                createStoreViewModel.SalesRepId = store.SalesRep.Id;
            } else
            {
                createStoreViewModel.SalesRepId = null;
            }
            createStoreViewModel.Store = store;

            ViewBag.SalesReps = _context.Users.Select(u => new SelectListItem() { Text = $"{ u.FirstName} { u.LastName}", Value = u.Id }).ToList();

            ViewData["StateId"] = new SelectList(_context.State.OrderBy(s => s.Name), "StateId", "Name", store.StateId);
            ViewData["StatusId"] = new SelectList(_context.Status, "StatusId", "Color", store.StatusId);
            return View(createStoreViewModel);
        }

        // POST: Stores/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreateStoreViewModel storeModel)
        {
            if (id != storeModel.Store.StoreId)
            {
                return NotFound();
            }

            ModelState.Remove("store.User");

            if (ModelState.IsValid)
            {
                var user = await GetCurrentUserAsync();

                storeModel.Store.User = user;

                try
                {
                    _context.Update(storeModel.Store);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StoreExists(storeModel.Store.StoreId))
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

            createStoreViewModel.Store = storeModel.Store;

            ViewBag.SalesReps = _context.Users.OrderBy(u => u.FirstName)
                .Select(u => new SelectListItem() { Text = $"{ u.FirstName} { u.LastName}", Value = u.Id }).ToList();


            ViewData["StateId"] = new SelectList(_context.State.OrderBy(st => st.Name), "StateId", "Name", storeModel.Store.StateId);
            ViewData["StatusId"] = new SelectList(_context.Status, "StatusId", "Color", storeModel.Store.StatusId);
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
                // use null-coalescing operator to cast DateTime? to DateTime, in case the current store doesn't have a last order date saved
                DateTime LastOrderDateForStore = s.LastOrderDate ?? DateTime.MinValue;

                // calculate the time difference between current date and the store's last order date
                TimeSpan interval = currentDate - LastOrderDateForStore;
                // store the store's current status
                int storeStatusId = s.StatusId;

                // check if the store is already marked as closed, and if the time difference between last order date and current date is greater than 6 months or 12 months
                if (s.StatusId == 4 || s.DateClosed != null)
                {
                    storeStatusId = 4;
                }
                else if ((interval.Days / 29) > 12)
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

        
        public async Task<IActionResult> AddFlag(int? id)
        {
            var store = await _context.Store.SingleOrDefaultAsync(m => m.StoreId == id);

            var FollowUpFlag = _context.Flag.Where(f => f.Name == "Follow Up").Single();

            var StoreFollowUp = new StoreFlag();
            StoreFollowUp.StoreId = store.StoreId;
            StoreFollowUp.FlagId = FollowUpFlag.FlagId;

            _context.Add(StoreFollowUp);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = id });
        }


        [HttpPost]
        public async Task<ActionResult> UploadCsv(IFormFile attachmentcsv)
        {
            // get current User
            var user = await GetCurrentUserAsync();
            // create list to hold csv records
            List<string> records = new List<string>();
            // create list to hold new stores to add
            List<Store> StoresToAdd = new List<Store>();
            // get the file path of the temp file created when csv is uploaded
            var filePath = Path.GetTempFileName();
            // create a stream file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                // copy contents of the temp csv file to the stream
                await attachmentcsv.CopyToAsync(stream);
                //var csv = new CsvReader(stream);
                // create a stream reader to read the file
                var reader = new StreamReader(stream);
                    
                //var records = csv.GetRecords().ToList();
                    
                /* reset the reader to the beginning to allow reading. Not sure exactly why 
                the reader is being created and immediately read, but this is a good workaround.
                */
                stream.Position = 0;
                reader.DiscardBufferedData();
                // get contents of the csv file
                var CsvContent = reader.ReadToEnd();
                // split the csv file contents on each new line        
                records = new List<string>(CsvContent.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));
                // iterate through the records and act upon each record
                foreach (string s in records)
                {
                    /* if statement makes sure we are not on the first line of csv that had header data
                    and the line has content */
                    if (!s.StartsWith("Customer") && s.Length > 5)
                    {
                        // create a new store instance
                        var ns = new Store();
                        // separate each column at the comma
                        //  this regular expression splits string on the separator character NOT inside double quotes. 
                        //and allows single quotes inside the string value: e.g. "Mike's Kitchen"
                        Regex regx = new Regex("," + "(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                        string[] csvColumn = regx.Split(s);
                        // assign each column to a store field

                        ns.Name = csvColumn[0].Replace("\\", "").Replace("\"","");
                        ns.ContactName = csvColumn[1];
                        ns.PhoneNumber = csvColumn[2];
                        ns.Email = csvColumn[3];
                        ns.StreetAddress = csvColumn[4] + " " + csvColumn[5];
                        ns.City = csvColumn[6];
                        // retrieve id for state from database then use it on store model
                        var StoreState = _context.State.Where(state => state.Name == csvColumn[7]).SingleOrDefault();
                        ns.StateId = StoreState.StateId;                     
                        ns.Zipcode = csvColumn[8];
                        // add current user
                        ns.User = user;
                        ns.StatusId = 1;

                        // move these fields to a different import csv file function
                        //ns.LastOrderDate = Convert.ToDateTime(textpart[2]);
                        //ns.DateAdded = DateTime.Now;
                        //ns.LastOrderShipDate = DateTime.Now;
                        //ns.LastOrderTotal = 22;

                        // add store to list of stores to add
                        StoresToAdd.Add(ns);
                    }
                }  
            }
            // add the new stores to the database
            _context.Store.AddRange(StoresToAdd);
            _context.SaveChanges();
            return Redirect("Index");
        }


    }
}
