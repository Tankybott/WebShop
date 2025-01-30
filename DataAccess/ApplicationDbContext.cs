using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;
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

            builder.Entity<Category>().HasData(
                // Root categories
                new Category { Id = 1, Name = "Vehicles", ParentCategoryId = null },
                new Category { Id = 2, Name = "Electronics", ParentCategoryId = null },
                new Category { Id = 3, Name = "Books", ParentCategoryId = null },

                // Subcategories under Vehicles
                new Category { Id = 4, Name = "Cars", ParentCategoryId = 1 },
                new Category { Id = 5, Name = "Motorcycles", ParentCategoryId = 1 },

                // Subcategories under Cars
                new Category { Id = 6, Name = "SUVs", ParentCategoryId = 4 },
                new Category { Id = 7, Name = "Sedans", ParentCategoryId = 4 },
                new Category { Id = 8, Name = "Trucks", ParentCategoryId = 4 },

                // Subcategories under Electronics
                new Category { Id = 9, Name = "Mobile Phones", ParentCategoryId = 2 },
                new Category { Id = 10, Name = "Laptops", ParentCategoryId = 2 },

                // Further subcategories under Mobile Phones
                new Category { Id = 11, Name = "Smartphones", ParentCategoryId = 9 },
                new Category { Id = 12, Name = "Feature Phones", ParentCategoryId = 9 },

                // Subcategories under Books
                new Category { Id = 13, Name = "Fiction", ParentCategoryId = 3 },
                new Category { Id = 14, Name = "Non-Fiction", ParentCategoryId = 3 },

                new Category { Id = 15, Name = "Maciek", ParentCategoryId = 13 }

            );
        }
    }
}
