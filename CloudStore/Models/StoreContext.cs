using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Data.Entity;

namespace CloudStore.Models
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
        public DbSet<Line> Lines { get; set; }
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

            PaymentMethod pm1 = new PaymentMethod { PaymentMethodName = "Cash" };
            PaymentMethod pm2 = new PaymentMethod { PaymentMethodName = "Credit card" };
            db.PaymentMethods.Add(pm1);
            db.PaymentMethods.Add(pm2);

            StatusOrder so1 = new StatusOrder { StatusOrderName = "Accepted processed" };
            StatusOrder so2 = new StatusOrder { StatusOrderName = "Processing" };
            StatusOrder so3 = new StatusOrder { StatusOrderName = "Processed" };
            StatusOrder so4 = new StatusOrder { StatusOrderName = "Completing" };
            StatusOrder so5 = new StatusOrder { StatusOrderName = "Completed" };
            StatusOrder so6 = new StatusOrder { StatusOrderName = "Handed over for delivery" };
            StatusOrder so7 = new StatusOrder { StatusOrderName = "Delivering" };
            StatusOrder so8 = new StatusOrder { StatusOrderName = "Done" };
            StatusOrder so9 = new StatusOrder { StatusOrderName = "Return" };
            StatusOrder so10 = new StatusOrder { StatusOrderName = "Canceled" };
            db.StatusOrders.Add(so1);
            db.StatusOrders.Add(so2);
            db.StatusOrders.Add(so3);
            db.StatusOrders.Add(so4);
            db.StatusOrders.Add(so5);
            db.StatusOrders.Add(so6);
            db.StatusOrders.Add(so7);
            db.StatusOrders.Add(so8);
            db.StatusOrders.Add(so9);
            db.StatusOrders.Add(so10);

            StatusProduct sp1 = new StatusProduct { StatusProductName = "In stock" };
            StatusProduct sp2 = new StatusProduct { StatusProductName = "Not available" };
            StatusProduct sp3 = new StatusProduct { StatusProductName = "Out of production" };
            db.StatusProducts.Add(sp1);
            db.StatusProducts.Add(sp2);
            db.StatusProducts.Add(sp3);

            TypeDelivery td1 = new TypeDelivery { TypeDeliveryName = "Pickup" };
            TypeDelivery td2 = new TypeDelivery { TypeDeliveryName = "Courier" };
            TypeDelivery td3 = new TypeDelivery { TypeDeliveryName = "Mail" };
            db.TypeDeliveries.Add(td1);
            db.TypeDeliveries.Add(td2);
            db.TypeDeliveries.Add(td3);

            base.Seed(db);
        }
    }
}