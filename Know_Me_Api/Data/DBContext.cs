using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Know_Me_Api.Models;

namespace Know_Me_Api.Models
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options)
            : base(options)
        {
        }

        public DbSet<UserInfo> UserInfo { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<WareHouse> WareHouse { get; set; }
        public DbSet<WareHouseProducts> WareHouseProducts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserInfo>().ToTable("UserInfo");
            modelBuilder.Entity<Role>().ToTable("Role");
            modelBuilder.Entity<Products>().ToTable("Products");
            modelBuilder.Entity<WareHouse>().ToTable("WareHouse");
            modelBuilder.Entity<WareHouseProducts>().ToTable("WareHouseProducts");

        }


    }
}
