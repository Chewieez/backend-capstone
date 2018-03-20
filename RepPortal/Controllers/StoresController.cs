using System;
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
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Xml.Linq;

namespace RepPortal.Controllers
{
    public class StoresController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _iConfiguration;

        public StoresController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IConfiguration iConfiguration)
        {
            _userManager = userManager;
            _context = context;
            _iConfiguration = iConfiguration;
        }

        // This task retrieves the currently authenticated user
        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        // GET: Stores
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
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

            ViewData["CurrentSort"] = sortOrder;
            ViewData["OrderDateSortParm"] = String.IsNullOrEmpty(sortOrder) ? "Date" : "";
            ViewData["NameSortParm"] = sortOrder == "name" ? "name_desc" : "Name";
            ViewData["StatusSortParm"] = sortOrder == "Status" ? "status_desc" : "Status";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

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
            //List<StoreListViewModel> StoresViewModels = new List<StoreListViewModel>();


            //foreach (Store s in stores)
            //{
            //    // create a new view model instance
            //    StoreListViewModel slvm = new StoreListViewModel();

            //    // find any flags for the store
            //    var flag = await _context.StoreFlag.Include("Flag").Where(f => f.StoreId == s.StoreId).SingleOrDefaultAsync();
            //    // attach flag info to the view model
            //    slvm.Flag1 = flag;
            //    // attach the store to the view model
            //    slvm.Store = s;
            //    // add the view model to the StoresViewModels list
            //    StoresViewModels.Add(slvm);
            //}


            int pageSize = 25;
            return View(await PaginatedList<Store>.CreateAsync(stores, page ?? 1, pageSize));

        }

        // GET: Stores/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            // create a new view model instance
            StoreDetailViewModel sdvm = new StoreDetailViewModel();

            // find any flags for the store
            var flag = await _context.StoreFlag.Include("Flag").Where(f => f.StoreId == id).SingleOrDefaultAsync();
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
                .Select(u => new SelectListItem() { Text = $"{ u.FirstName} { u.LastName}", Value = u.Id }).ToList();

            ViewData["StateId"] = new SelectList(_context.State.OrderBy(s => s.Name), "StateId", "Name");
            ViewData["StatusId"] = new SelectList(_context.Status, "StatusId", "Name");

            return await View(createStoreViewModel);
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
            }
            else
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
                // add salesRep if info changed
                if (storeModel.Store.SalesRep == null)
                {
                    var AddedSalesRep = await _context.Users.Where(u => u.Id == storeModel.SalesRepId).SingleOrDefaultAsync();
                    storeModel.Store.SalesRep = AddedSalesRep;
                }

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
            // create a list of smaller version of each store to send in JSON response.
            var SmallStores = new List<StoreJsonResponse>();

            // check if the user is an Administrator
            if (roles.Contains("Administrator"))
            {
                // retrieve all stores to display on map (for site administrator)
                stores = await _context.Store.Include(s => s.State).Include(s => s.Status).ToListAsync();
            }
            else
            {
                // retrieve only matching stores where current user is the Sales Rep attached to the store
                stores = await _context.Store.Include(s => s.State).Include(s => s.Status).Where(s => s.SalesRep == user).ToListAsync();
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
                else if ((interval.Days / 29) >= 12)
                {
                    storeStatusId = 3;
                }
                else if ((interval.Days / 29) >= 6)
                {
                    storeStatusId = 2;
                }
                else
                {
                    storeStatusId = 1;
                }

                // if status has changed, save new status and write to database
                if (storeStatusId != s.StatusId)
                {
                    s.StatusId = storeStatusId;
                    _context.Update(s);
                    await _context.SaveChangesAsync();
                }

                // create a new smaller version of the store info to return in the JSON result
                var SmallStore = new StoreJsonResponse()
                {
                    Name = s.Name,
                    StoreId = s.StoreId,
                    Lat = s.Lat,
                    Long = s.Long,
                    StreetAddress = s.StreetAddress,
                    CityStateZip = $"{ s.City} {s.State.Name} { s.Zipcode}",
                    StatusId = s.StatusId
                };

                SmallStores.Add(SmallStore);
            }


            // return a json formatted response to be used in javascript ajax call
            return Ok(SmallStores);
        }


        public async Task<IActionResult> AddFlag(int? id)
        {
            var store = await _context.Store.SingleOrDefaultAsync(m => m.StoreId == id);

            var FollowUpFlag = await _context.Flag.Where(f => f.Name == "Follow Up").SingleOrDefaultAsync();

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

                // create a stream reader to read the file
                var reader = new StreamReader(stream);

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
                        // this regular expression splits string on the separator character NOT inside double quotes. 
                        // and allows single quotes inside the string value: e.g. "Mike's Kitchen"
                        Regex regx = new Regex("," + "(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                        string[] csvColumn = regx.Split(s);

                        // assign each column to a store field

                        ns.Name = csvColumn[0].Replace("\\", "").Replace("\"", "");
                        ns.ContactName = csvColumn[1];
                        ns.PhoneNumber = csvColumn[2];
                        ns.Email = csvColumn[3];
                        ns.StreetAddress = csvColumn[4] + " " + csvColumn[5];
                        ns.City = csvColumn[6];
                        // retrieve id for state from database then use it on store model
                        var StoreState = await _context.State.Where(state => state.Name == csvColumn[7]).SingleOrDefaultAsync();
                        ns.StateId = StoreState.StateId;
                        ns.Zipcode = csvColumn[8];
                        // add current user
                        ns.User = user;
                        ns.StatusId = 1;
                        // assign the sales rep if one is assigned, if not, assign the admin house user
                        ns.SalesRep = await _context.Users.Where(sr => sr.Company == csvColumn[9]).SingleOrDefaultAsync();

                        // add store to list of stores to add
                        StoresToAdd.Add(ns);
                    }
                }
            }
            // add the new stores to the database
            _context.Store.AddRange(StoresToAdd);
            await _context.SaveChangesAsync();
            return Redirect("Index");
        }

        [HttpPost]
        public async Task<ActionResult> UpdateStoreViaCsv(IFormFile attachmentUpdateCsv)
        {
            // get current User
            var user = await GetCurrentUserAsync();
            // create list to hold csv records
            List<string> UpdatedRecords = new List<string>();

            // get the file path of the temp file created when csv is uploaded
            var filePath = Path.GetTempFileName();
            // create a stream file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                // copy contents of the temp csv file to the stream
                await attachmentUpdateCsv.CopyToAsync(stream);

                // create a stream reader to read the file
                var reader = new StreamReader(stream);

                /* reset the reader to the beginning to allow reading. Not sure exactly why 
                the reader is being created and immediately read, but this is a good workaround.
                */
                stream.Position = 0;
                reader.DiscardBufferedData();
                // read first line of csv file which is just headers, and toss
                reader.ReadLine();
                // get contents of the csv file
                var CsvContent = reader.ReadToEnd();
                // split the csv file contents on each new line        
                UpdatedRecords = new List<string>(CsvContent.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));
                // iterate through the records and act upon each record
                foreach (string line in UpdatedRecords)
                {
                    /* make sure the line has content */
                    if (line.Length > 5)
                    {
                        // separate each column at the comma
                        // this regular expression splits string on the separator character NOT inside double quotes. 
                        // and allows single quotes inside the string value: e.g. "Mike's Kitchen"
                        Regex regx = new Regex("," + "(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                        string[] csvColumn = regx.Split(line);

                        // Get the name of the current store in the csv file
                        var StoreToUpdateName = csvColumn[0].Replace("\\", "").Replace("\"", "");
                        // find the matching store in the database
                        var UpdatedStore = _context.Store.Where(s => s.Name == StoreToUpdateName).SingleOrDefault();

                        if (UpdatedStore != null)
                        {
                            // assign each column to a store field
                            try
                            {
                                UpdatedStore.LastOrderDate = Convert.ToDateTime(csvColumn[1]);
                            }
                            catch (FormatException e)
                            {
                                Console.WriteLine(e.Message);
                            }
                            try
                            {
                                UpdatedStore.LastOrderShipDate = Convert.ToDateTime(csvColumn[2]);

                            }
                            catch (FormatException e)
                            {
                                Console.WriteLine(e.Message);
                            }
                            UpdatedStore.LastOrderTotal = Convert.ToDouble(csvColumn[3].Replace("\\", "").Replace("\"", "").Replace(",", ""));

                            // update store in database
                            try
                            {
                                _context.Update(UpdatedStore);
                            }
                            catch (DbUpdateConcurrencyException)
                            {
                                if (!StoreExists(UpdatedStore.StoreId))
                                {
                                    return NotFound();
                                }
                                else
                                {
                                    throw;
                                }
                            }

                        }
                    }
                }
            }
            // save changes to the database
            await _context.SaveChangesAsync();
            return Redirect("Index");
        }

        // GET - Function retrieves all the stores in database, checks if they contain geolocation, if they don't, an api call to 
        // Google is performed and the results are attached to the store and saved to database.
        public async void GeolocateAllStores()
        {
            // get google api key
            var GoogleApi = _iConfiguration.GetValue<string>("ApplicationConfiguration:GoogleAPIKey");
            //get list of all stores
            List<Store> AllStores = await _context.Store.Include(s => s.State).ToListAsync();
            // iterate through list and pull address info to use in call to google api
            foreach (Store s in AllStores)
            {
                if (s.Lat == null) {
                    // create a URI string for the api call to Google
                    string requestUri = string.Format("https://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=false&key={1}", Uri.EscapeDataString(s.StreetAddress + s.City + s.State.Name + s.Zipcode), GoogleApi);
                    // create a web request using the built uri
                    WebRequest request = WebRequest.Create(requestUri);
                    WebResponse response = request.GetResponse();
                    // if there is a valid response from Google, read response, if not, catch errors
                    try
                    {
                        // get response stream and read it
                        XDocument xdoc = XDocument.Load(response.GetResponseStream());
                        XElement result = xdoc.Element("GeocodeResponse").Element("result");
                        XElement locationElement = result.Element("geometry").Element("location");
                        XElement lat = locationElement.Element("lat");
                        XElement lng = locationElement.Element("lng");
                        // parse the lat and long data returned
                        string ParsedLat = lat.ToString().Replace("<lat>", "").Replace("</lat>", "");
                        string ParsedLng = lng.ToString().Replace("<lng>", "").Replace("</lng>", "");

                        // attach geolocation data to store
                        s.Lat = ParsedLat;
                        s.Long = ParsedLng;

                        // update store in database
                        _context.Update(s);
                        await _context.SaveChangesAsync(); 
                    } catch (Exception)
                    {
                        Console.WriteLine("Error getting geolocation");
                    }
                }
            }
        }
    }
}
