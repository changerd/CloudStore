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
            //db.Roles.Add(new ApplicationRole { Name = "Admin" });
            //db.Roles.Add(new ApplicationRole { Name = "User" });
            //var userstore = new UserStore<ApplicationUser>(db);
            //var usermanager = new UserManager<ApplicationUser>(userstore);
            ////ApplicationUser admin = new ApplicationUser();
            ////admin.UserName = "admin";
            ////usermanager.Create(admin, "admin");
            ////usermanager.AddToRole(admin.Id, "Admin");
            //ApplicationUser user = new ApplicationUser { UserName = "admin" };
            //IdentityResult result = usermanager.Create(user, "admin");
            //if (result.Succeeded)
            //{
            //    usermanager.AddToRole(user.Id, "Admin");
            //    Cart cart = new Cart
            //    {
            //        UserId = user.Id,
            //    };
            //    db.Carts.Add(cart);                
            //}
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));

            
            var role1 = new IdentityRole { Name = "Admin" };
            var role2 = new IdentityRole { Name = "User" };

            
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