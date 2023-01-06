using Gospodarstwo.Data;
using Gospodarstwo.Models;
using Gospodarstwo.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Gospodarstwo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            HomeDataVM homeData = new();
            homeData.DisplayCategories = _context.Categories?
                .Where(c => c.Active == true && c.Display == true);
            homeData.Authors = _context.Items.Include(a => a.User).Select(a => a.User).Distinct();
            homeData.Items = _context.Items.Where(i => i.Active);
            homeData.Units = _context.Units;
            return View(homeData);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}