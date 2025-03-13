using Microsoft.AspNetCore.Mvc;
using KrishiBazaarProject.Data;
using KrishiBazaarProject.Models;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace KrishiBazaarProject.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var admin = _context.Admins.FirstOrDefault(a => a.UserName == username && a.Password == password);
            if (admin != null)
            {
                HttpContext.Session.SetString("AdminID", admin.AdminID.ToString());
                return RedirectToAction("Index", "Admin");
            }
            ViewBag.Error = "Invalid username or password";
            return View();
        }
        public IActionResult Index()
        {
            var products = _context.Products.ToList();
            return View(products);
        }

        public IActionResult DeleteProduct(int productId)
        {
            var product = _context.Products.Find(productId);
            if (product == null) return NotFound();

            _context.Products.Remove(product);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult PendingOrders()
        {
            var orders = _context.Orders
                .Include(o => o.Product)
                .Include(o => o.User)
                .Where(o => o.IsPaid == true && o.IsDelivered == false)
                .ToList();

            return View(orders);
        }
        public IActionResult ApproveDelivery(int orderId)
        {
            var order = _context.Orders.Find(orderId);
            if (order == null) return NotFound();

            order.IsDelivered = true;
            _context.SaveChanges();

            return RedirectToAction("PendingOrders");
        }
        public IActionResult CreatePrimaryHub()
        {
            ViewBag.Locations = _context.Locations.ToList();
            return View();
        }
        [HttpPost]
        public IActionResult CreatePrimaryHub(PrimaryHub primaryHub)
        {
            if (true)
            {
                _context.PrimaryHubs.Add(primaryHub);
                _context.SaveChanges();
                return RedirectToAction("Index", "Admin");
            }
            ViewBag.Locations = _context.Locations.ToList();
            return View(primaryHub);
        }
        public IActionResult CreateSecondaryHub()
        {
            ViewBag.Locations = _context.Locations.ToList();
            ViewBag.PrimaryHubs = _context.PrimaryHubs.ToList();
            return View();
        }
        [HttpPost]
        public IActionResult CreateSecondaryHub(SecondaryHub secondaryHub)
        {
            if (true)
            {
                _context.SecondaryHubs.Add(secondaryHub);
                _context.SaveChanges();
                return RedirectToAction("Index", "Admin");
            }
            ViewBag.Locations = _context.Locations.ToList();
            ViewBag.PrimaryHubs = _context.PrimaryHubs.ToList();
            return View(secondaryHub);
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("AdminID");
            return RedirectToAction("Login");
        }
    }
}