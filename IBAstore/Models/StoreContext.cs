using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Data.Entity;

namespace IBAstore.Models
{
    public class StoreContext : IdentityDbContext<ApplicationUser/*, ApplicationRole, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim*/>
    {
        public StoreContext() : base("StoreContext") { }
        public static StoreContext Create()
        {
            return new StoreContext();
        }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Category> Categories { get; set; }        
        //public DbSet<ApplicationUser> Users { get; set; }
        //public DbSet<ApplicationRole> Roles { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<StatusOrder> StatusOrders { get; set; }
        public DbSet<StatusProduct> StatusProducts { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<TypeDelivery> TypeDeliveries { get; set; }      
        public DbSet<SaleStat> SaleStats { get; set; }
        public DbSet<ProductRequest> ProductRequests { get; set; }
    }
    public class DBInitializer : CreateDatabaseIfNotExists<StoreContext>
    {
        protected override void Seed(StoreContext db)
        {
            db.Roles.Add(new ApplicationRole { Name = "Admin" });
            db.Roles.Add(new ApplicationRole { Name = "User" });
            var userstore = new UserStore<ApplicationUser>(db);
            var usermanager = new UserManager<ApplicationUser>(userstore);
            //ApplicationUser admin = new ApplicationUser();
            //admin.UserName = "admin";
            //usermanager.Create(admin, "admin");
            //usermanager.AddToRole(admin.Id, "Admin");
            ApplicationUser user = new ApplicationUser { UserName = "admin" };
            IdentityResult result = usermanager.Create(user, "admin");
            if (result.Succeeded)
            {
                usermanager.AddToRole(user.Id, "Admin");
                Cart cart = new Cart
                {
                    UserId = user.Id,
                };
                db.Carts.Add(cart);                
            }
            base.Seed(db);
        }
    }
}