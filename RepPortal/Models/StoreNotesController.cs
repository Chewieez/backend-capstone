using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RepPortal.Data;

namespace RepPortal.Models
{
    public class StoreNotesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StoreNotesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: StoreNotes
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.StoreNote.Include(s => s.Store);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: StoreNotes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var storeNote = await _context.StoreNote
                .Include(s => s.Store)
                .SingleOrDefaultAsync(m => m.StoreNoteId == id);
            if (storeNote == null)
            {
                return NotFound();
            }

            return View(storeNote);
        }

        // GET: StoreNotes/Create
        public IActionResult Create()
        {
            ViewData["StoreId"] = new SelectList(_context.Store, "StoreId", "City");
            return View();
        }

        // POST: StoreNotes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StoreNoteId,StoreId,Content,DateCreated")] StoreNote storeNote)
        {
            if (ModelState.IsValid)
            {
                _context.Add(storeNote);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StoreId"] = new SelectList(_context.Store, "StoreId", "City", storeNote.StoreId);
            return View(storeNote);
        }

        // GET: StoreNotes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var storeNote = await _context.StoreNote.SingleOrDefaultAsync(m => m.StoreNoteId == id);
            if (storeNote == null)
            {
                return NotFound();
            }
            ViewData["StoreId"] = new SelectList(_context.Store, "StoreId", "City", storeNote.StoreId);
            return View(storeNote);
        }

        // POST: StoreNotes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StoreNoteId,StoreId,Content,DateCreated")] StoreNote storeNote)
        {
            if (id != storeNote.StoreNoteId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(storeNote);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StoreNoteExists(storeNote.StoreNoteId))
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
            ViewData["StoreId"] = new SelectList(_context.Store, "StoreId", "City", storeNote.StoreId);
            return View(storeNote);
        }

        // GET: StoreNotes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var storeNote = await _context.StoreNote
                .Include(s => s.Store)
                .SingleOrDefaultAsync(m => m.StoreNoteId == id);
            if (storeNote == null)
            {
                return NotFound();
            }

            return View(storeNote);
        }

        // POST: StoreNotes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var storeNote = await _context.StoreNote.SingleOrDefaultAsync(m => m.StoreNoteId == id);
            _context.StoreNote.Remove(storeNote);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StoreNoteExists(int id)
        {
            return _context.StoreNote.Any(e => e.StoreNoteId == id);
        }
    }
}
