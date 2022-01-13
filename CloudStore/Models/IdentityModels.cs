using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CloudStore.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public DateTime? Birth { get; set; }
        public bool GetNews { get; set; }
        public virtual ICollection<Cart> Carts { get; set; } 
        public virtual ICollection<SaleStat> SaleStats { get; set; }
        public virtual ICollection<ProductRequest> ProductRequests { get; set; }
        public ApplicationUser()
        {
            Carts = new List<Cart>();
            SaleStats = new List<SaleStat>();
            ProductRequests = new List<ProductRequest>();
        }
    }    
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
                : base(store)
        {
        }
        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options,
                                                IOwinContext context)
        {
            StoreContext db = context.Get<StoreContext>();
            ApplicationUserManager manager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));
            return manager;
        }
    }
    public class ApplicationRole : IdentityRole
    {
    }
    class ApplicationRoleManager : RoleManager<ApplicationRole>
    {
        public ApplicationRoleManager(RoleStore<ApplicationRole> store)
                    : base(store)
        { }
        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options,
                                                IOwinContext context)
        {
            return new ApplicationRoleManager(new
                    RoleStore<ApplicationRole>(context.Get<StoreContext>()));
        }
    }
}