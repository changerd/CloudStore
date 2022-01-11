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

            PaymentMethod pm1 = new PaymentMethod { PaymentMethodName = "Наличными" };
            PaymentMethod pm2 = new PaymentMethod { PaymentMethodName = "Банковской картой" };
            db.PaymentMethods.Add(pm1);
            db.PaymentMethods.Add(pm2);

            StatusOrder so1 = new StatusOrder { StatusOrderName = "Принят" };
            StatusOrder so2 = new StatusOrder { StatusOrderName = "Обрабатывается" };
            StatusOrder so3 = new StatusOrder { StatusOrderName = "Обработан" };
            StatusOrder so4 = new StatusOrder { StatusOrderName = "Комплектуется" };
            StatusOrder so5 = new StatusOrder { StatusOrderName = "Скомплектован" };
            StatusOrder so6 = new StatusOrder { StatusOrderName = "Передан на доставку" };
            StatusOrder so7 = new StatusOrder { StatusOrderName = "Доставляется" };
            StatusOrder so8 = new StatusOrder { StatusOrderName = "Выполнен" };
            StatusOrder so9 = new StatusOrder { StatusOrderName = "Возврат" };
            StatusOrder so10 = new StatusOrder { StatusOrderName = "Отменён" };
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

            StatusProduct sp1 = new StatusProduct { StatusProductName = "В наличии" };
            StatusProduct sp2 = new StatusProduct { StatusProductName = "Нет в наличии" };
            StatusProduct sp3 = new StatusProduct { StatusProductName = "Снят с производства" };
            db.StatusProducts.Add(sp1);
            db.StatusProducts.Add(sp2);
            db.StatusProducts.Add(sp3);

            TypeDelivery td1 = new TypeDelivery { TypeDeliveryName = "Самовывоз" };
            TypeDelivery td2 = new TypeDelivery { TypeDeliveryName = "Курьер" };
            TypeDelivery td3 = new TypeDelivery { TypeDeliveryName = "Почта" };
            db.TypeDeliveries.Add(td1);
            db.TypeDeliveries.Add(td2);
            db.TypeDeliveries.Add(td3);

            base.Seed(db);
        }
    }
}