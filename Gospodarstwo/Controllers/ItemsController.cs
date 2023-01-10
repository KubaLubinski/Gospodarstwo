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
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Gospodarstwo.Infrastrukture;

namespace Gospodarstwo.Controllers
{
    public class ItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private IWebHostEnvironment _hostEnvironment;

        public ItemsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _hostEnvironment = environment;
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

            ViewData["Category"] = new SelectList(_context.Categories?
                 .Where(c => c.Active == true),
                 "CategoryId", "CategoryName", Kategoria);

            ViewData["Author"] = new SelectList(_context.Items?
                 .Include(u => u.User)
                .Select(u => u.User)
                .Distinct(),
                 "Id", "FullName", Autor);

            return View(itemsViewModel);
        }

        // GET: Items
        [Authorize(Roles = "admin")]
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

            ItemWithNotes itemWithNotes = new ItemWithNotes();

            itemWithNotes.SelectedItem = await _context.Items
                .Include(i => i.Category)
                .Include(i => i.User)
                .Include(i => i.Notes)
                .ThenInclude(c => c.User)
                .Where(t => t.Active == true)
                .FirstOrDefaultAsync(m => m.ItemId == id);

            if (itemWithNotes.SelectedItem == null)
            {
                return NotFound();
            }

            itemWithNotes.NewNote = new Note { ItemId = (int)id, Id = itemWithNotes.SelectedItem.Id };
            itemWithNotes.ReadingTime = (int)Math.Ceiling((double)itemWithNotes.SelectedItem.Content.Length / 1400);
            itemWithNotes.CommentsNumber = _context.Notes
                .Where(x => x.ItemId == id)
                .Count();

            itemWithNotes.Description = Variaty.Phrase("notatka", "notatki", "notatek", itemWithNotes.CommentsNumber);

            return View(itemWithNotes);
        }

        // GET: Items/Create
        [Authorize(Roles = "admin, author")]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            ViewData["UnitId"] = new SelectList(_context.Units, "UnitId", "UnitName");
            return View();
        }

        // POST: Items/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, author")]
        public async Task<IActionResult> Create([Bind("ItemId,ItemName,Content,Active,CategoryId,ItemQuantity,MaxStoreCapacity,UnitId")] Item item, IFormFile? obrazek)
        {
            if (ModelState.IsValid)
            {
                item.Id = User.FindFirstValue(ClaimTypes.NameIdentifier);
                item.AddedDate = DateTime.Now;

                //zapisanie obrazka i skalowanie
                if (obrazek != null && obrazek.Length > 0)
                {
                    ImageFileUpload imageFileResult = new(_hostEnvironment);
                    FileSendResult fileSendResult = imageFileResult.SendFile(obrazek, "img", 600);

                    if (fileSendResult.Success)
                    {
                        item.Graphic = fileSendResult.Name;
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Wybrany plik nie jest obrazkiem!";
                        ViewData["CategoryId"] = new SelectList(_context.Categories,"CategoryId", "CategoryName", item.CategoryId);
                        ViewData["UnitId"] = new SelectList(_context.Units, "UnitId", "UnitName", item.UnitId);
                        return View(item);
                    }

                }

                if (!ItemNameExist(item.ItemName))
                {
                    _context.Add(item);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.ErrorMessage = "Przedmiot o takiej nazwie już istnieje!";
                    ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", item.CategoryId);
                    ViewData["UnitId"] = new SelectList(_context.Units, "UnitId", "UnitName", item.UnitId);
                    return View(item);
                }
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", item.CategoryId);
            ViewData["UnitId"] = new SelectList(_context.Units, "UnitId", "UnitName", item.UnitId);
            return View(item);
        }

        // GET: Items/Edit/5
        [Authorize(Roles = "admin, author")]
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

            if (string.Compare(User.FindFirstValue(ClaimTypes.NameIdentifier), item.Id) == 0 || User.IsInRole("admin"))
            {
                ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", item.CategoryId);
                ViewData["UnitId"] = new SelectList(_context.Units, "UnitId", "UnitName", item.UnitId);
                ViewData["Author"] = item.Id;
                return View(item);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }

        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, author")]
        public async Task<IActionResult> Edit(int itemid, [Bind("ItemId,ItemName,Content,Graphic,Active,AddedDate,CategoryId,Id,ItemQuantity,MaxStoreCapacity,UnitId")] Item item, IFormFile? picture)
        {
            if (itemid != item.ItemId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (picture != null && picture.Length > 0)
                {
                    ImageFileUpload imageFileResult = new(_hostEnvironment);
                    FileSendResult fileSendResult = imageFileResult.SendFile(picture, "img", 600);
                    if (fileSendResult.Success)
                    {
                        item.Graphic = fileSendResult.Name;
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Wybrany plik nie jest obrazkiem!";
                        ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", item.CategoryId);
                        ViewData["UnitId"] = new SelectList(_context.Units, "UnitId", "UnitName", item.UnitId);
                        ViewData["Author"] = item.Id;
                        return View(item);
                    }
                }
                if (!ItemNameExist(item.ItemName))
                {

                }
                else
                {
                    ViewBag.ErrorMessage = "Przedmiot o takiej nazwie już istnieje!";
                    ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", item.CategoryId);
                    ViewData["UnitId"] = new SelectList(_context.Units, "UnitId", "UnitName", item.UnitId);
                    ViewData["Author"] = item.Id; ;
                    return View(item);
                }
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
            ViewData["Author"] = item.Id;
            return View(item);
        }

        // GET: Items/Delete/5
        [Authorize(Roles = "admin")]
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
        [Authorize(Roles = "admin")]
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

        private bool ItemNameExist(string? name)
        {
            return (_context.Items?.Any(e => e.ItemName == name)).GetValueOrDefault();
        }
    }
}
