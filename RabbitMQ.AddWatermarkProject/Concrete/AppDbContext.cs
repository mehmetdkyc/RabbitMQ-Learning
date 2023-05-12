using Microsoft.EntityFrameworkCore;
using RabbitMQ.AddWatermarkProject.Models;

namespace RabbitMQ.AddWatermarkProject.Concrete
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
    }
}
