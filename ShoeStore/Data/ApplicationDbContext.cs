using Microsoft.EntityFrameworkCore;
using ShoeStore.Model.Entity;

namespace ShoeStore.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }
        public DbSet<Users> Users { get; set; }
        public DbSet<ShippingAddress> ShippingAddresses { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<MasterColorPalette> MasterColorPalettes { get; set; }
        public DbSet<MasterCategories> MasterCategories { get; set; }
        public DbSet<MasterSizeChart> MasterSizeCharts { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductDetails> ProductDetails { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<SizeVariant> SizeVariants { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Users - ShippingAddress relationship (One-to-Many)
            modelBuilder.Entity<ShippingAddress>()
                .HasOne(sa => sa.Users)
                .WithMany(u => u.Address)
                .HasForeignKey(sa => sa.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Categories - Products relationship (One-to-Many)
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Categories)
                .WithMany(c => c.Product)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Product - ProductDetails relationship (One-to-One)
            modelBuilder.Entity<ProductDetails>()
                .HasOne(pd => pd.Product)
                .WithOne(p => p.ProductDetails)
                .HasForeignKey<ProductDetails>(pd => pd.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Product - ProductVariant relationship (One-to-Many)
            modelBuilder.Entity<ProductVariant>()
                .HasOne(pv => pv.Product)
                .WithMany(p => p.ProductVariant)
                .HasForeignKey(pv => pv.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure ProductVariant - SizeVariant relationship (One-to-Many)
            modelBuilder.Entity<SizeVariant>()
                .HasOne(sv => sv.ProductVariant)
                .WithMany(pv => pv.SizeVariant)
                .HasForeignKey(sv => sv.VariantsId)
                .OnDelete(DeleteBehavior.Cascade);

            // ShoppingCart - Users relationship (One-to-One)
            modelBuilder.Entity<ShoppingCart>()
                .HasOne(sc => sc.Users)
                .WithMany()
                .HasForeignKey(sc => sc.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // CartItem - ShoppingCart relationship
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.ShoppingCart)
                .WithMany(sc => sc.CartItems)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            // CartItem - ProductVariant relationship
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.ProductVariant)
                .WithMany()
                .HasForeignKey(ci => ci.VariantsId)
                .OnDelete(DeleteBehavior.Restrict);

            // CartItem - SizeVariant relationship
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.SizeVariant)
                .WithMany()
                .HasForeignKey(ci => ci.SizesId)
                .OnDelete(DeleteBehavior.Restrict);

            // Wishlist - Users relationship
            modelBuilder.Entity<Wishlist>()
                .HasOne(w => w.Users)
                .WithMany()
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Wishlist - ProductVariant relationship
            modelBuilder.Entity<Wishlist>()
                .HasOne(w => w.ProductVariant)
                .WithMany()
                .HasForeignKey(w => w.VariantsId)
                .OnDelete(DeleteBehavior.Cascade);
            // Order - Users relationship
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Users)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order - ShippingAddress relationship
            modelBuilder.Entity<Order>()
                .HasOne(o => o.ShippingAddress)
                .WithMany()
                .HasForeignKey(o => o.ShippingAddressId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order - StatusHistory relationship
            modelBuilder.Entity<OrderStatusHistory>()
                .HasOne(osh => osh.Order)
                .WithMany(o => o.StatusHistory)
                .HasForeignKey(osh => osh.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Order - OrderItems relationship
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            // OrderItem - SizeVariant relationship
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.SizeVariant)
                .WithMany()
                .HasForeignKey(oi => oi.SizesId)
                .OnDelete(DeleteBehavior.Restrict);


            // Unique order number
            modelBuilder.Entity<Order>()
                .HasIndex(o => o.OrderNumber)
                .IsUnique();

            // Unique constraint: One wishlist item per product per user
            modelBuilder.Entity<Wishlist>()
                .HasIndex(w => new { w.UserId, w.VariantsId })
                .IsUnique();

            modelBuilder.Entity<Users>()
                .HasIndex(u => u.UserEmail)
                .IsUnique();

            modelBuilder.Entity<Categories>()
                .HasIndex(c => c.CategoryName)
                .IsUnique();

            modelBuilder.Entity<MasterCategories>()
                .HasIndex(c => c.CategoryName)
                .IsUnique();

            modelBuilder.Entity<MasterColorPalette>()
                .HasIndex(c => c.ColorName)
                .IsUnique();

            modelBuilder.Entity<MasterSizeChart>()
                .HasIndex(s => s.Size)
                .IsUnique();

            // Configure decimal precision for prices
            modelBuilder.Entity<SizeVariant>()
                .Property(sv => sv.MRP)
                .HasPrecision(18, 2);

            modelBuilder.Entity<SizeVariant>()
                .Property(sv => sv.Price)
                .HasPrecision(18, 2);

            //// Configure default values
            //modelBuilder.Entity<Users>()
            //    .Property(u => u.CreatedAt)
            //    .HasDefaultValueSql("GETUTCDATE()");

            //modelBuilder.Entity<Users>()
            //    .Property(u => u.IsActive)
            //    .HasDefaultValue(true);

            //modelBuilder.Entity<ShippingAddress>()
            //    .Property(sa => sa.Country)
            //    .HasDefaultValue("India");

            //modelBuilder.Entity<ShippingAddress>()
            //    .Property(sa => sa.IsDefault)
            //    .HasDefaultValue(false);

            //modelBuilder.Entity<ShippingAddress>()
            //    .Property(sa => sa.IsActive)
            //    .HasDefaultValue(true);
        }
       
    }
}

