using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Models;
using System.Reflection.Emit;
using Models.DatabaseRelatedModels;

namespace DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<PhotoUrlSet> PhotoUrlSets { get; set; }
        public DbSet<Cart> Cart { get; set; }
        public DbSet<CartItem> CartItem { get; set; }
        public DbSet<OrderDetail> OrderDetail { get; set;}
        public DbSet<OrderHeader> OrderHeaders { get; set;}
        public DbSet<Carrier> Carrier { get; set; }
        public DbSet<WebshopConfig> WebshopConfig { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Product>(entity =>
            {
                entity.HasMany(p => p.PhotosUrlSets)
                    .WithOne(pu => pu.Product)
                    .HasForeignKey(pu => pu.ProductId) 
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(p => p.Discount)
                    .WithMany()
                    .HasForeignKey(p => p.DiscountId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            builder.Entity<Cart>(entity =>
            {
                entity.HasOne(c => c.User)
                .WithMany()               
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(c => c.Items)
                .WithOne(ci => ci.Cart)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);
            });


            builder.Entity<WebshopConfig>().HasData(
                new WebshopConfig { Id = 1, SiteName = "Webshop", Currency = "USD", FreeShippingFrom = null }
            );
        }
    }
}
