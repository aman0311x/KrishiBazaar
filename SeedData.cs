using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using KrishiBazaar.Models;

namespace KrishiBazaar.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new AppDbContext(serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>()))
            {
                // 🔹 Check if Hubs data already exists
                if (!context.Hubs.Any())
                {
                    context.Hubs.AddRange(new List<Hub>
                    {
                        new Hub { Name = "ctg hub 1", Division = "ctg", Capacity = 50 },
                        new Hub { Name = "ctg Hub 2", Division = "ctg", Capacity = 50 },
                        new Hub { Name = "Chittagong Hub 1", Division = "Chittagong", Capacity = 50 },
                        new Hub { Name = "Chittagong Hub 2", Division = "Chittagong", Capacity = 50 },
                    });

                    context.SaveChanges();
                }

                // 🔹 Check if Products data already exists
                if (!context.Products.Any())
                {
                    context.Products.AddRange(new List<Product>
                    {
                        new Product { ProductName = "Rice", Quantity = 100, Price = 50, QuantityUnit = "kg", FarmerId = "seller1" },
                        new Product { ProductName = "Wheat", Quantity = 80, Price = 40, QuantityUnit = "kg", FarmerId = "seller2" }
                    });

                    context.SaveChanges();
                }
            }
        }
    }
}
