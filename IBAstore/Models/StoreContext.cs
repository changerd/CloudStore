using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace IBAstore.Models
{
    public class StoreContext : DbContext
    {
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<StatusOrder> StatusOrders { get; set; }
        public DbSet<StatusProduct> StatusProducts { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<TypeDelivery> TypeDeliveries { get; set; }       
    }
}