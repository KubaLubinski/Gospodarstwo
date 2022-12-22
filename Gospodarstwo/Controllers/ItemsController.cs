using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Gospodarstwo.Data;
using Gospodarstwo.Models;
using Gospodarstwo.Models.ViewModels;
using static System.Net.Mime.MediaTypeNames;

namespace Gospodarstwo.Controllers
{
    public class ItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Items
        public async Task<IActionResult> Index(string Fraza, string Autor, int? Kategoria, int PageNumber = 1)
        {
            var SelectedItems = _context.Items?
                .Include(t => t.Category)
                .Include(t => t.User)
                .Where(t => t.Active == true)
                .OrderByDescending(t => t.AddedDate);

            if (Kategoria != null)
            {
                SelectedItems = (IOrderedQueryable<Item>)SelectedItems.Where(r => r.Category.CategoryId == Kategoria);
            }
            if (!String.IsNullOrEmpty(Autor))
            {
                SelectedItems = (IOrderedQueryable<Item>)SelectedItems.Where(r => r.User.Id == Autor);
            }
            if (!String.IsNullOrEmpty(Fraza))
            {
                SelectedItems = (IOrderedQueryable<Item>)SelectedItems.Where(r => r.Content.Contains(Fraza));
            }

            ItemsViewModel itemsViewModel = new();
            itemsViewModel.ItemsView = new ItemsView();

            itemsViewModel.ItemsView.ItemCount = SelectedItems.Count();
            itemsViewModel.ItemsView.PageNumber = PageNumber;
            itemsViewModel.ItemsView.Author = Autor;
            itemsViewModel.ItemsView.Phrase = Fraza;
            itemsViewModel.ItemsView.Category = Kategoria;

            itemsViewModel.Items = (IEnumerable<Item>?)await SelectedItems
                .Skip((PageNumber - 1) * itemsViewModel.ItemsView.PageSize)
                .Take(itemsViewModel.ItemsView.PageSize)
                .ToListAsync();

            ViewData["Category"] = new SelectList(_context.Categories
                 .Where(c => c.Active == true),
                 "Category", "CategoryName", Kategoria);

            ViewData["Author"] = new SelectList(_context.Items
                 .Include(u => u.User)
                .Select(u => u.User)
                .Distinct(),
                 "Id", "FullName", Autor);

            return View(itemsViewModel);
        }

        // GET: Items
        public async Task<IActionResult> List()
        {
            var applicationDbContext = _context.Items.Include(i => i.Category).Include(i => i.Unit).Include(i => i.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Items/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Items == null)
            {
                return NotFound();
            }

            var item = await _context.Items
                .Include(i => i.Category)
                .Include(i => i.Unit)
                .Include(i => i.User)
                .FirstOrDefaultAsync(m => m.ItemId == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // GET: Items/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            ViewData["UnitId"] = new SelectList(_context.Units, "UnitId", "UnitName");
            ViewData["Id"] = new SelectList(_context.AppUsers, "Id", "Id");
            return View();
        }

        // POST: Items/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ItemId,ItemName,Content,Graphic,Active,AddedDate,CategoryId,Id,ItemQuantity,MaxStoreCapacity,UnitId")] Item item)
        {
            if (ModelState.IsValid)
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", item.CategoryId);
            ViewData["UnitId"] = new SelectList(_context.Units, "UnitId", "UnitName", item.UnitId);
            ViewData["Id"] = new SelectList(_context.AppUsers, "Id", "Id", item.Id);
            return View(item);
        }

        // GET: Items/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Items == null)
            {
                return NotFound();
            }

            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", item.CategoryId);
            ViewData["UnitId"] = new SelectList(_context.Units, "UnitId", "UnitName", item.UnitId);
            ViewData["Id"] = new SelectList(_context.AppUsers, "Id", "Id", item.Id);
            return View(item);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ItemId,ItemName,Content,Graphic,Active,AddedDate,CategoryId,Id,ItemQuantity,MaxStoreCapacity,UnitId")] Item item)
        {
            if (id != item.ItemId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemExists(item.ItemId))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", item.CategoryId);
            ViewData["UnitId"] = new SelectList(_context.Units, "UnitId", "UnitName", item.UnitId);
            ViewData["Id"] = new SelectList(_context.AppUsers, "Id", "Id", item.Id);
            return View(item);
        }

        // GET: Items/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Items == null)
            {
                return NotFound();
            }

            var item = await _context.Items
                .Include(i => i.Category)
                .Include(i => i.Unit)
                .Include(i => i.User)
                .FirstOrDefaultAsync(m => m.ItemId == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Items == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Items'  is null.");
            }
            var item = await _context.Items.FindAsync(id);
            if (item != null)
            {
                _context.Items.Remove(item);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ItemExists(int id)
        {
          return _context.Items.Any(e => e.ItemId == id);
        }
    }
}
