using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using KrishiBazaar.Data;
using KrishiBazaar.Models;
using KrishiBazaar.ViewModels;

namespace KrishiBazaar.Controllers
{
    [Authorize]
    public class FarmerController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FarmerController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // ✅ Logged-in Farmer এর সকল প্রোডাক্ট দেখানো হবে
        public async Task<IActionResult> MyPosts()
        {
            string farmerEmail = User.Identity.Name;
            if (string.IsNullOrEmpty(farmerEmail)) return Unauthorized();

            var myProducts = await _context.Products
                .Where(p => p.Farmer.Email == farmerEmail) // শুধু Logged-in User এর পোস্ট
                .ToListAsync();

            return View(myProducts);
        }

        // ✅ Dashboard - Future Use (Currently Not Showing Products)
        public IActionResult Dashboard()
        {
            return View();
        }

        // ✅ GET: Post Product Form
        public IActionResult PostProduct()
        {
            return View();
        }

        // ✅ POST: New Product Submission
        [HttpPost]
        public async Task<IActionResult> PostProduct(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string farmerEmail = User.Identity.Name;
            if (string.IsNullOrEmpty(farmerEmail)) return Unauthorized();

            var farmer = await _context.Users.SingleOrDefaultAsync(u => u.Email == farmerEmail);
            if (farmer == null) return NotFound("Farmer not found");

            string imagePath = await UploadImage(model.Image);

            var product = new Product
            {
                ProductType = model.ProductType,
                ProductName = model.ProductName,
                Quantity = model.Quantity,
                QuantityUnit = model.QuantityUnit,
                Price = model.Price,
                ImagePath = imagePath,
                FarmerId = farmer.Id
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return RedirectToAction("MyPosts");
        }

        // ✅ Edit Product Form
        public async Task<IActionResult> EditProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            string farmerEmail = User.Identity.Name;
            var farmer = await _context.Users.SingleOrDefaultAsync(u => u.Email == farmerEmail);
            if (farmer == null || product.FarmerId != farmer.Id) return Unauthorized();

            return View(product);
        }

        // ✅ Update Product Data
        [HttpPost]
        public async Task<IActionResult> EditProduct(Product model, IFormFile? Image)
        {
            if (!ModelState.IsValid) return View(model);

            var product = await _context.Products.FindAsync(model.Id);
            if (product == null) return NotFound();

            string farmerEmail = User.Identity.Name;
            var farmer = await _context.Users.SingleOrDefaultAsync(u => u.Email == farmerEmail);
            if (farmer == null || product.FarmerId != farmer.Id) return Unauthorized();

            product.ProductName = model.ProductName;
            product.Quantity = model.Quantity;
            product.QuantityUnit = model.QuantityUnit;
            product.Price = model.Price;

            if (Image != null)
            {
                string newImagePath = await UploadImage(Image);
                product.ImagePath = newImagePath;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("MyPosts");
        }

        // ✅ Delete Product
        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            string farmerEmail = User.Identity.Name;
            var farmer = await _context.Users.SingleOrDefaultAsync(u => u.Email == farmerEmail);
            if (farmer == null || product.FarmerId != farmer.Id) return Unauthorized();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction("MyPosts");
        }

        // ✅ Utility Function for Image Upload
        private async Task<string> UploadImage(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0) return null;

            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return "uploads/" + uniqueFileName;
        }
    }
}
