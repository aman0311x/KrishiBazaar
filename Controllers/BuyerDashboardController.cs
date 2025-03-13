using Microsoft.AspNetCore.Mvc;
using KrishiBazaarProject.Data;
using KrishiBazaarProject.Models;
using Microsoft.EntityFrameworkCore;
using KrishiBazaarProject.ViewModels;

namespace KrishiBazaarProject.Controllers
{
    public class BuyerDashboardController : Controller
    {
        private readonly AppDbContext _context;

        public BuyerDashboardController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var products = _context.Products.ToList();
            return View(products);
        }

        public IActionResult ProductDetails(int id)
        {
            var product = _context.Products
                .Where(p => p.ProductID == id)
                .Select(p => new
                {
                    p.ProductID,
                    p.Name,
                    p.Description,
                    p.PricePerUnit,
                    p.AvailableQuantity,
                    p.Photos,
                    Farmer = _context.Users
                        .Where(u => u.UserID == p.FarmerID)
                        .Select(u => new { u.FullName, u.Email, u.PhoneNumber })
                        .FirstOrDefault(),
                    Reviews = _context.Reviews
                        .Where(r => r.ProductID == p.ProductID)
                        .Select(r => new
                        {
                            r.Rating,
                            r.Comment,
                            BuyerName = _context.Users
                                .Where(u => u.UserID == r.BuyerID)
                                .Select(u => u.FullName)
                                .FirstOrDefault()
                        }).ToList()
                })
                .FirstOrDefault();

            if (product == null)
            {
                return NotFound();
            }

            double avgRating = product.Reviews.Any() ? product.Reviews.Average(r => r.Rating) : 0;

            var viewModel = new ProductDetailsViewModel
            {
                ProductID = product.ProductID,
                Name = product.Name,
                Description = product.Description,
                PricePerUnit = product.PricePerUnit,
                AvailableQuantity = product.AvailableQuantity,
                Photos = product.Photos,
                Farmer = product.Farmer,
                AverageRating = avgRating,
                Reviews = product.Reviews.Select(r => new ReviewViewModel
                {
                    Rating = r.Rating,
                    Comment = r.Comment,
                    BuyerName = r.BuyerName
                }).ToList()
            };

            return View(viewModel);
        }


        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity)
        {
            var product = _context.Products.Find(productId);
            if (product == null || quantity <= 0 || quantity > product.AvailableQuantity)
            {
                return BadRequest("Invalid quantity or product.");
            }
            var order = new Orders
            {
                ProductID = productId,
                BuyerID = HttpContext.Session.GetString("UserID"),
                QuantityToBuy = quantity,
                TotalPrice = (int)(product.PricePerUnit * quantity),
                IsPaid = false,
                OrderDate = DateTime.Now
            };

            _context.Orders.Add(order);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Successfully Added to Cart!";
            return RedirectToAction("ProductDetails", new { id = productId });
        }

        public IActionResult Cart()
        {
            
            string loggedInUserId = HttpContext.Session.GetString("UserID");

            var cartOrders = _context.Orders
                .Where(o => o.BuyerID == loggedInUserId && !o.IsPaid)
                .Select(o => new
                {
                    o.OrderID,
                    o.QuantityToBuy,
                    o.TotalPrice,
                    Product = _context.Products
                        .Where(p => p.ProductID == o.ProductID)
                        .Select(p => new { p.Name, p.Photos, p.PricePerUnit })
                        .FirstOrDefault()
                })
                .ToList<dynamic>();

            return View(cartOrders);
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int orderId)
        {
            var order = _context.Orders.Find(orderId);
            if (order != null)
            {
                _context.Orders.Remove(order);
                _context.SaveChanges();
            }
            return RedirectToAction("Cart");
        }

        [HttpPost]
        public IActionResult PayNow(int orderId)
        {
            var order = _context.Orders
                .Include(o => o.Product) 
                .FirstOrDefault(o => o.OrderID == orderId);

            if (order == null || order.IsPaid)
            {
                return BadRequest("Invalid order or already paid.");
            }

            if (order.Product.AvailableQuantity < order.QuantityToBuy)
            {
                return BadRequest("Insufficient stock for this product.");
            }
            order.Product.AvailableQuantity -= order.QuantityToBuy;

            order.IsPaid = true;

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Payment successful!";
            return RedirectToAction("Index");
        }


        public IActionResult Payment(int orderId)
        {
            var order = _context.Orders
                .Where(o => o.OrderID == orderId && !o.IsPaid)
                .Select(o => new
                {
                    o.OrderID,
                    o.TotalPrice,
                    ProductName = o.Product.Name
                })
                .FirstOrDefault();

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost]
        public IActionResult ProcessPayment(int orderId, string paymentMethod)
        {
            var order = _context.Orders.Find(orderId);
            if (order == null || order.IsPaid)
            {
                return BadRequest("Invalid order.");
            }

            var transaction = new Transactions
            {
                OrderID = order.OrderID,
                Amount = order.TotalPrice,
                PaymentMethod = paymentMethod,
                PaymentStatus = true, 
                PaymentDate = DateTime.Now
            };

            order.IsPaid = true;
            order.IsDelivered = false;

            _context.Transactions.Add(transaction);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Payment successful!";
            return RedirectToAction("Cart");
        }
        public IActionResult DeliveredOrders()
        {
            
            string loggedInUserId = HttpContext.Session.GetString("UserID");

            var deliveredOrders = _context.Orders
                .Where(o => o.BuyerID == loggedInUserId && o.IsDelivered)
                .Select(o => new
                {
                    o.OrderID,
                    o.QuantityToBuy,
                    o.TotalPrice,
                    o.IsReviewed,
                    Product = _context.Products
                        .Where(p => p.ProductID == o.ProductID)
                        .Select(p => new { p.Name, p.Photos, p.PricePerUnit })
                        .FirstOrDefault()
                })
                .ToList<dynamic>();

            return View(deliveredOrders);
        }

        [HttpPost]
        public IActionResult SubmitReview(int orderId, int rating, string comment)
        {
            var order = _context.Orders.Find(orderId);
            if (order == null || order.IsReviewed)
            {
                return BadRequest("Invalid order or already reviewed.");
            }

            var review = new RatingAndReview
            {
                ProductID = order.ProductID,
                BuyerID = order.BuyerID,
                Rating = rating,
                Comment = comment,
                ReviewDate = DateTime.Now
            };

            order.IsReviewed = true; 

            _context.Reviews.Add(review);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Review submitted successfully!";
            return RedirectToAction("DeliveredOrders");
        }


        public IActionResult TrackOrders()
        {
            var userId = HttpContext.Session.GetString("UserID");
            var user = _context.Users.Include(u => u.Address).FirstOrDefault(u => u.UserID.ToString() == userId);

            if (user == null) return NotFound("User not found");

            List<Orders> orders;

            if (user.IsFarmer)
            {
                orders = _context.Orders
                    .Include(o => o.Product)
                    .Include(o => o.User)
                    .Where(o => o.Product.FarmerID == user.UserID && !o.IsDelivered)
                    .ToList();
            }
            else
            {
                orders = _context.Orders
                    .Include(o => o.Product)
                    .ThenInclude(p => p.User)
                    .Include(o => o.User)
                    .Where(o => o.BuyerID == user.UserID && !o.IsDelivered)
                    .ToList();
            }

            var orderRoutes = new List<OrderRouteViewModel>();

            foreach (var order in orders)
            {
                var farmerLocation = _context.Locations.FirstOrDefault(l => l.PostalCode == order.Product.User.AddressID);
                var buyerLocation = _context.Locations.FirstOrDefault(l => l.PostalCode == order.User.AddressID);

                

                var farmerSecondaryHub = FindNearestSecondaryHub(farmerLocation);
                var farmerPrimaryHub = _context.PrimaryHubs.FirstOrDefault(ph => ph.PrimaryHubID == farmerSecondaryHub.PrimaryHubID);
                var buyerSecondaryHub = FindNearestSecondaryHub(buyerLocation);
                var buyerPrimaryHub = _context.PrimaryHubs.FirstOrDefault(ph => ph.PrimaryHubID == buyerSecondaryHub.PrimaryHubID);

                if ( buyerSecondaryHub == null)
                    return NotFound("Sorry ,There is no Hub near your location");
                if (farmerSecondaryHub == null)
                    return NotFound("Sorry ,There is no Hub near the farmer");

                // Fetching location details for each hub
                var farmerSecondaryHubLocation = _context.Locations.FirstOrDefault(l => l.PostalCode == farmerSecondaryHub.LocationID);
                var farmerPrimaryHubLocation = _context.Locations.FirstOrDefault(l => l.PostalCode == farmerPrimaryHub.LocationID);
                var buyerPrimaryHubLocation = _context.Locations.FirstOrDefault(l => l.PostalCode == buyerPrimaryHub.LocationID);
                var buyerSecondaryHubLocation = _context.Locations.FirstOrDefault(l => l.PostalCode == buyerSecondaryHub.LocationID);

                var route = new OrderRouteViewModel
                {
                    OrderID = order.OrderID,
                    RoutePoints = new List<string>
                    {
                        $"Secondary Hub: {farmerSecondaryHubLocation?.District}, {farmerSecondaryHubLocation?.Upazila}, {farmerSecondaryHubLocation?.Thana}",
                        $"Primary Hub: {farmerPrimaryHubLocation?.District}, {farmerPrimaryHubLocation?.Upazila}, {farmerPrimaryHubLocation?.Thana}",
                        $"Primary Hub: {buyerPrimaryHubLocation?.District}, {buyerPrimaryHubLocation?.Upazila}, {buyerPrimaryHubLocation?.Thana}",
                        $"Secondary Hub: {buyerSecondaryHubLocation?.District}, {buyerSecondaryHubLocation?.Upazila}, {buyerSecondaryHubLocation?.Thana}"
                    }
                };
                orderRoutes.Add(route);
            }

            return View(orderRoutes);
        }
        private SecondaryHub FindNearestSecondaryHub(Location location)
        {
            return _context.SecondaryHubs
                .Include(sh => sh.Location)
                .OrderByDescending(sh => sh.Location.District == location.District)
                .ThenByDescending(sh => sh.Location.Upazila == location.Upazila)
                .ThenByDescending(sh => sh.Location.Thana == location.Thana)
                .FirstOrDefault();
        }

    }
}
