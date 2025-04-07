using Microsoft.EntityFrameworkCore;
using ML.NET.Models;

namespace ML.NET.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {
        }

        // DbSet 將會對應到資料庫中的 "Products" 資料表
        public DbSet<Product> Products { get; set; } = null!;

    }
}
