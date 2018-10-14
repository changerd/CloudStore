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
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));
            var roleManager = new ApplicationRoleManager(new RoleStore<ApplicationRole>(db));            
            var role1 = new ApplicationRole { Name = "Admin" };
            var role2 = new ApplicationRole { Name = "User" };            
            roleManager.Create(role1);
            roleManager.Create(role2);            
            var admin = new ApplicationUser { UserName = "admin" };
            string password = "adminiba";
            var result = userManager.Create(admin, password);            
            if (result.Succeeded)
            {                
                userManager.AddToRole(admin.Id, role1.Name);
                userManager.AddToRole(admin.Id, role2.Name);
                Cart cart = new Cart
                {
                    UserId = admin.Id,
                };
                db.Carts.Add(cart);
            }
            base.Seed(db);
        }
    }
}