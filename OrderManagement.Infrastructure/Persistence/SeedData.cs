using OrderManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace OrderManagement.Infrastructure.Persistence
{
    public static class SeedData
    {
        public static async Task InitializeAsync(AppDbContext context)
        {
            if (await context.Products.AnyAsync()) return; 

            var products = new List<Product>
            {
                new Product { Id = Guid.NewGuid(), Name = "Laptop", Price = 1200m, Stock = 10 },
                new Product { Id = Guid.NewGuid(), Name = "Mouse", Price = 25m, Stock = 50 },
                new Product { Id = Guid.NewGuid(), Name = "Keyboard", Price = 45m, Stock = 30 },
                new Product { Id = Guid.NewGuid(), Name = "Monitor", Price = 200m, Stock = 15 }
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
        }
    }
}
