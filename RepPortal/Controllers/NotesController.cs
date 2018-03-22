using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RepPortal.Data;
using RepPortal.Models;
using RepPortal.Models.NotesViewModels;

namespace RepPortal.Controllers
{
    public class NotesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // This task retrieves the currently authenticated user
        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        // GET: Notes
        public async Task<IActionResult> Index()
        {
            var notes = await _context.Note.Include("ToUser").ToListAsync();

            return View(notes);
        }

        // GET: Notes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var note = await _context.Note
                .SingleOrDefaultAsync(m => m.NoteId == id);
            if (note == null)
            {
                return NotFound();
            }

            return View(note);
        }

        // GET: Notes/Create
        public IActionResult Create()
        {
            CreateNoteViewModel CreateNoteViewModel = new CreateNoteViewModel();

            ViewBag.Users = _context.Users.OrderBy(u => u.FirstName)
                .Select(u => new SelectListItem() { Text = $"{ u.FirstName} { u.LastName}", Value = u.Id }).ToList();


            return View(CreateNoteViewModel);
        }

        // POST: Notes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( CreateNoteViewModel noteViewModel)
        {
            ModelState.Remove("note.user");

            if (ModelState.IsValid)
            {
                if (noteViewModel.ToUserId != null)
                {
                    // find matching user for SalesRep in system
                    ApplicationUser ToUser = _context.Users.Single(u => u.Id == noteViewModel.ToUserId);

                    // store the sales rep on the store
                    noteViewModel.Note.ToUser = ToUser;
                }

                // get current user
                ApplicationUser user = await GetCurrentUserAsync();
                // add current user
                noteViewModel.Note.User = user;

                _context.Add(noteViewModel.Note);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Home");
        }

        // GET: Notes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var note = await _context.Note.Include("ToUser").SingleOrDefaultAsync(m => m.NoteId == id);
            // create a new view model and attach the retrieved note
            var cnvm = new CreateNoteViewModel()
            {
                Note = note
            };

            if (note.ToUser != null)
            {
                cnvm.ToUserId = note.ToUser.Id;
            }
            else
            {
                cnvm.ToUserId = null;
            }

            // populate dropdown of users
            ViewBag.Users = _context.Users.OrderBy(u => u.FirstName)
                .Select(u => new SelectListItem() { Text = $"{ u.FirstName} { u.LastName}", Value = u.Id }).ToList();

            if (note == null)
            {
                return NotFound();
            }
            return View(cnvm);
        }

        // POST: Notes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NoteId,ToUserId,Content,DateCreated")] Note note)
        {
            if (id != note.NoteId)
            {
                return NotFound();
            }

            // remove user from model state
            ModelState.Remove("store.User");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(note);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NoteExists(note.NoteId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Home");
            }
            return View(note);
        }

        // GET: Notes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var note = await _context.Note.Include("ToUser")
                .SingleOrDefaultAsync(m => m.NoteId == id);
            if (note == null)
            {
                return NotFound();
            }

            return View(note);
        }

        // POST: Notes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var note = await _context.Note.SingleOrDefaultAsync(m => m.NoteId == id);
            _context.Note.Remove(note);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        private bool NoteExists(int id)
        {
            return _context.Note.Any(e => e.NoteId == id);
        }
    }
}
