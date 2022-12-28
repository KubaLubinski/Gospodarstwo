using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Gospodarstwo.Data;
using Gospodarstwo.Models;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.CodeAnalysis.Options;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace Gospodarstwo.Controllers
{
    public class NotesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Notes
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
            {
                var applicationDbContext = _context.Notes.Include(o => o.Item).Include(o => o.User);
                return View(await applicationDbContext.ToListAsync());
            }
            else
            {
                var appDbContext = _context.Notes.Include(o => o.Item).Include(o => o.User).Where(o => o.ItemId == id);
                return View(await appDbContext.ToListAsync());
            }
        }

        // GET: Notes/Details/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Notes == null)
            {
                return NotFound();
            }

            var note = await _context.Notes
                .Include(n => n.Item)
                .Include(n => n.User)
                .FirstOrDefaultAsync(m => m.NoteId == id);
            if (note == null)
            {
                return NotFound();
            }

            return View(note);
        }

        // GET: Notes/Create
        [Authorize]
        public IActionResult Create(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Item item = _context.Items.Find(id);
            if (item == null)
            {
                return BadRequest();
            }
            ViewData["IdItem"] = id;
            ViewData["ItemName"] = item.ItemName;
            return View();
        }

        // POST: Notes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("NoteId,Content,ItemId")] Note note)
        {
            if (ModelState.IsValid)
            {
                note.AddedDate = DateTime.Now;
                note.Id = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _context.Add(note);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Items", new { id = note.ItemId }, "comments");
            }
            ViewData["IdItem"] = note.ItemId;
            ViewData["ItemName"] = note.Item.ItemName;
            return View(note);
        }

        // POST: Notes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> CreatePartial([Bind("NoteId,Content,ItemId")] Note note)
        {
            if (ModelState.IsValid)
            {
                note.AddedDate = DateTime.Now;
                note.Id = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _context.Add(note);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Items", new { id = note.ItemId }, "comments");
            }
            ViewData["IdItem"] = note.ItemId;
            ViewData["ItemName"] = note.Item.ItemName;
            return RedirectToAction("Details", "Items", new { id = note.ItemId }, "comments");

        }

        // GET: Notes/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Notes == null)
            {
                return NotFound();
            }

            var note = await _context.Notes.FindAsync(id);
            if (note == null)
            {
                return NotFound();
            }
            ViewData["ItemId"] = new SelectList(_context.Items, "ItemId", "Content", note.ItemId);
            ViewData["Id"] = new SelectList(_context.AppUsers, "Id", "Id", note.Id);
            return View(note);
        }

        // POST: Notes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, [Bind("NoteId,Content")] Note note)
        {
            if (id != note.NoteId)
            {
                return NotFound();
            }

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
                return RedirectToAction(nameof(Index));
            }
            ViewData["ItemId"] = new SelectList(_context.Items, "ItemId", "Content", note.ItemId);
            ViewData["Id"] = new SelectList(_context.AppUsers, "Id", "Id", note.Id);
            return View(note);
        }

        // GET: Notes/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Notes == null)
            {
                return NotFound();
            }

            var note = await _context.Notes
                .Include(n => n.Item)
                .Include(n => n.User)
                .FirstOrDefaultAsync(m => m.NoteId == id);
            if (note == null)
            {
                return NotFound();
            }

            return View(note);
        }

        // POST: Notes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Notes == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Notes'  is null.");
            }
            var note = await _context.Notes.FindAsync(id);
            if (note != null)
            {
                _context.Notes.Remove(note);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NoteExists(int id)
        {
          return _context.Notes.Any(e => e.NoteId == id);
        }
    }
}
