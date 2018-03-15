using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepPortal.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using RepPortal.Models;
using RepPortal.Models.NotesViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RepPortal.ViewComponents
{
    public class NotesViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotesViewComponent(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // This task retrieves the currently authenticated user
        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // populate dropdown of users
            ViewBag.Users = _context.Users.OrderBy(u => u.FirstName)
                .Select(u => new SelectListItem() { Text = $"{ u.FirstName} { u.LastName}", Value = u.Id }).ToList();

            // create a new Notes view model instance
            var NotesViewModel = new CreateNoteViewModel();

            // get notes from the database
            NotesViewModel.UserNotes = await GetNotesAsync();

            // return flags
            return View(NotesViewModel);
        }

        private async Task<List<Note>> GetNotesAsync()
        {
            // get user
            var user = await GetCurrentUserAsync();

            List<Note> UserNotes = new List<Note>();
            // check if the user is an administator
            // if user is admin, get all notes, if user is not, get notes that the user created
            if (User.IsInRole("Administrator"))
            {
                UserNotes = await _context.Note.Include("ToUser").ToListAsync();
            } else
            {
                UserNotes = await _context.Note.Include("ToUser").Where(n => n.User == user || n.ToUser == user).ToListAsync();
            }

            return UserNotes.OrderByDescending(n => n.DateCreated).ToList();
        }

        private void SaveNote(Note note)
        {
        //save note to the database
                _context.SaveChanges();
        }
    }
}