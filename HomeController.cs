using System.Diagnostics;
using KrishiBazaar.Data;
using KrishiBazaar.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace KrishiBazaar.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult BuyerHome()
        {
            return View();
        }

        public IActionResult FarmerHome()
        {
            var products = _context.Products.ToList();
            return View(products);
        }

        public IActionResult GetProducts()
        {
            var products = _context.Products.ToList();
            return View(products);
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
