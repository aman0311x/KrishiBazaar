using KrishiBazaar.Data;
//using KrishiBazaar.Migrations;
using KrishiBazaar.Models;
using KrishiBazaar.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace KrishiBazaar.Controllers
{
    [Authorize]
    public class BuyerController : Controller
    {
        private readonly AppDbContext _context;
        private readonly AppDbContext _orderService;
        public BuyerController(AppDbContext context, AppDbContext orderService)
        {
            _context = context;
            _orderService = orderService;
        }

        // ✅ Fetch all products for buyers
        public IActionResult GetProducts()
        {
            var products = _context.Products.ToList();
            return View(products);
        }

        // ✅ Add product to cart
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var buyer = await _context.Users.FindAsync(userId);
            var product = await _context.Products.FindAsync(productId);

            if (product == null || buyer == null)
            {
                return NotFound();
            }

            // কার্টে একই প্রোডাক্ট আছে কিনা চেক করা
            var existingCartItem = await _context.Carts
                .FirstOrDefaultAsync(c => c.BuyerId == userId && c.ProductId == productId);

            if (existingCartItem != null)
            {
                TempData["Error"] = "Product already exist!";
                return RedirectToAction("Cart");
            }
            else
            {
                // নতুন কার্ট আইটেম যোগ করা হবে
                var cartItem = new Cart
                {
                    ProductId = productId,
                    BuyerId = userId,
                    MobileNumber = buyer.MobileNumber,
                    Division = buyer.Division,

                };
                _context.Carts.Add(cartItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Cart");
        }


        // ✅ Fetch Cart Items
        public IActionResult Cart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItems = _context.Carts
                .Include(c => c.Product)
                .ThenInclude(p => p.Farmer)
                .Where(c => c.BuyerId == userId)
                .ToList();

            // Fetch hubs for each product's division
            var hubs = _context.Hubs.ToList();
            ViewBag.Hubs = hubs;

            return View(cartItems);
        }
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int cartId)
        {
            var cartItem = await _context.Carts.FindAsync(cartId);

            if (cartItem == null)
            {
                return NotFound();
            }

            _context.Carts.Remove(cartItem);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Item removed from cart successfully!";
            return RedirectToAction("Cart");
        }
        [HttpGet]
        public IActionResult Checkout(int cartId)
        {
            var cartItem = _context.Carts
                .Include(c => c.Product)
                .FirstOrDefault(c => c.Id == cartId);

            if (cartItem == null)
            {
                return NotFound();
            }

            var hubs = _context.Hubs.ToList();

            var model = new CheckoutViewModel
            {
                CartId = cartItem.Id,
                ProductId = cartItem.ProductId,
                ProductName = cartItem.Product.ProductName,
                SellerName = cartItem.Product.Farmer?.FullName ?? "Unknown",
                Price = cartItem.Product.Price,
                AvailableQuantity = cartItem.Product.Quantity,
                Hubs = hubs
            };

            return View(model);
        }
        [HttpPost]
        [HttpPost]
        public IActionResult ConfirmCheckout(int cartId, int productId, int selectedQuantity, int selectedHubId, string paymentMethod)
        {
            if (selectedQuantity <= 0)
            {
                ModelState.AddModelError("", "Invalid quantity selected.");
                return RedirectToAction("Checkout", new { cartId });
            }

            var product = _context.Products.FirstOrDefault(p => p.Id == productId);
            if (product == null)
            {
                return NotFound();
            }

            // ✅ Check if selected quantity exceeds available stock
            if (selectedQuantity > product.Quantity)
            {
                TempData["Error"] = "Selected quantity exceeds available stock!";
                return RedirectToAction("Checkout", new { cartId });
            }

            var buyerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var totalAmount = selectedQuantity * product.Price;

            // Convert payment method to B, C, or M
            string paymentCode = paymentMethod switch
            {
                "Bank" => "B",
                "Card" => "C",
                "Cash" => "M",
                _ => "M" // Default to Cash if something is wrong
            };

            var order = new Order
            {
                ProductId = productId,
                BuyerId = buyerId,
                Quantity = selectedQuantity,
                TotalAmount = totalAmount,
                Status = "Pending",
                PaymentMethod = paymentCode,  // Stored as B, C, or M
                OrderDate = DateTime.Now,
                SelectedHubId = selectedHubId,
                ImagePath = product.ImagePath // Store product image path
            };

            _context.Orders.Add(order);

            // ✅ Reduce product quantity after successful order
            product.Quantity -= selectedQuantity;
            _context.SaveChanges();

            var cartItem = _context.Carts.FirstOrDefault(c => c.Id == cartId);
            if (cartItem != null)
            {
                _context.Carts.Remove(cartItem);
                _context.SaveChanges();
            }

            return RedirectToAction("MyOrders", new { orderId = order.Id });
        }



        public async Task<IActionResult> MyOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orders = await _context.Orders
                .Include(o => o.Product)
                .Include(o => o.SelectedHub)
                .Where(o => o.BuyerId == userId)
                .ToListAsync();

            return View(orders);
        }
        public IActionResult OrderDetails(int orderId)
        {
            var order = _context.Orders
                .Include(o => o.Product)
                .ThenInclude(p => p.Farmer)
                .FirstOrDefault(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound();
            }

            var viewModel = new OrderDetailsViewModel
            {
                ProductName = order.Product.ProductName,
                SellerName = order.Product.Farmer.FullName,
                SellerAddress = order.Product.Farmer.Address,
                Quantity = order.Quantity,
                TotalAmount = order.TotalAmount,
                PaymentMethod = order.PaymentMethod,
                OrderDate = order.OrderDate
            };

            return View(viewModel);
        }



    }
}