using IBAstore.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IBAstore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        StoreContext db = new StoreContext();
        private ApplicationUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }
        private ApplicationRoleManager RoleManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            }
        }
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetRole()
        {
            return View(RoleManager.Roles);
        }
        //[HttpGet]
        //public ActionResult Create()
        //{
        //    return View();
        //}
        //[HttpPost]
        //public ActionResult Create(Category category)
        //{
        //    db.Categories.Add(category);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");                
        //}
        public ActionResult CreateRole()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> CreateRole(CreateRoleModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await RoleManager.CreateAsync(new ApplicationRole
                {
                    Name = model.Name,                    
                });
                if (result.Succeeded)
                {
                    return RedirectToAction("GetRole");
                }
                else
                {
                    ModelState.AddModelError("", "Что-то пошло не так");
                }
            }
            return View(model);
        }
        public async Task<ActionResult> EditRole(string id)
        {
            ApplicationRole role = await RoleManager.FindByIdAsync(id);
            if (role != null)
            {
                return View(new EditRoleModel { Id = role.Id, Name = role.Name });
            }
            return RedirectToAction("GetRole");
        }
        [HttpPost]
        public async Task<ActionResult> EditRole(EditRoleModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationRole role = await RoleManager.FindByIdAsync(model.Id);
                if (role != null)
                {                    
                    role.Name = model.Name;
                    IdentityResult result = await RoleManager.UpdateAsync(role);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("GetRole");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Что-то пошло не так");
                    }
                }
            }
            return View(model);
        }
        public async Task<ActionResult> DeleteRole(string id)
        {
            ApplicationRole role = await RoleManager.FindByIdAsync(id);
            if (role != null)
            {
                IdentityResult result = await RoleManager.DeleteAsync(role);
            }
            return RedirectToAction("GetRole");
        }
        public async Task<ActionResult> GiveRole(string id)
        {
            ApplicationRole role = await RoleManager.FindByIdAsync(id);
            string[] memberIDs = role.Users.Select(x => x.UserId).ToArray();

            IEnumerable<ApplicationUser> members = UserManager.Users.Where(x => memberIDs.Any(y => y == x.Id));

            IEnumerable<ApplicationUser> nonMembers = UserManager.Users.Except(members);

            return View(new RoleEditModel
            {
                Role = role,
                Members = members,
                NonMembers = nonMembers
            });
        }

        [HttpPost]
        public async Task<ActionResult> GiveRole(RoleModificationModel model)
        {
            IdentityResult result;
            if (ModelState.IsValid)
            {
                foreach (string userId in model.IdsToAdd ?? new string[] { })
                {
                    result = await UserManager.AddToRoleAsync(userId, model.RoleName);

                    if (!result.Succeeded)
                    {
                        return View("Error", result.Errors);
                    }
                }
                foreach (string userId in model.IdsToDelete ?? new string[] { })
                {
                    result = await UserManager.RemoveFromRoleAsync(userId,
                    model.RoleName);

                    if (!result.Succeeded)
                    {
                        return View("Error", result.Errors);
                    }
                }
                return RedirectToAction("GetRole");

            }
            return View("Error", new string[] { "Роль не найдена" });
        }
        public ActionResult GetUser()
        {
            return View(UserManager.Users);
        }        
        public ActionResult DetailsUser(string id)
        {
            var user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }        
        public async Task<ActionResult> DeleteUser(string id)
        {
            Cart query = db.Carts.Where(c => c.UserId == id).FirstOrDefault();
            if (query != null)
            {
                db.Carts.Remove(query);
                await db.SaveChangesAsync();
            }
            ApplicationUser user = await UserManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await UserManager.DeleteAsync(user);                
            }
            return RedirectToAction("GetUser");
        }
        public ActionResult GetManufacturer()
        {
            List<Manufacturer> manufacturer = db.Manufacturers.ToList();
            return View(manufacturer);
        }        
        public ActionResult CreateManufacturer()
        {
            return View();
        }
        [HttpPost]
        public async Task <ActionResult> CreateManufacturer(Manufacturer manufacturer)
        {
            db.Manufacturers.Add(manufacturer);
            await db.SaveChangesAsync();
            return RedirectToAction("GetManufacturer");
        }
        public ActionResult EditManufacturer(int id)
        {            
            Manufacturer manu = db.Manufacturers.Find(id);
            if (manu != null)
            {
                return View(manu);
            }            
            return HttpNotFound();
        }
        [HttpPost]
        public async Task<ActionResult> EditManufacturer(Manufacturer manu)
        {
            db.Entry(manu).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return RedirectToAction("GetManufacturer");
        }
        public async Task<ActionResult> DeleteManufacturer(int id)
        {
            Manufacturer manu = db.Manufacturers.Find(id);
            if (manu != null)
            {
                db.Manufacturers.Remove(manu);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("GetManufacturer");
        }
        public ActionResult GetStatusProduct()
        {
            List<StatusProduct> status = db.StatusProducts.ToList();
            return View(status);
        }
        public ActionResult CreateStatusProduct()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> CreateStatusProduct(StatusProduct status)
        {
            db.StatusProducts.Add(status);
            await db.SaveChangesAsync();
            return RedirectToAction("GetStatusProduct");
        }        
        public async Task<ActionResult> DeleteStatusProduct(int id)
        {
            StatusProduct status = db.StatusProducts.Find(id);
            if (status != null)
            {
                db.StatusProducts.Remove(status);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("GetStatusProduct");
        }
        public ActionResult GetCategory()
        {
            List<Category> category = db.Categories.ToList();
            return View(category);
        }
        public ActionResult CreateCategory()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> CreateCategory(Category category)
        {
            db.Categories.Add(category);
            await db.SaveChangesAsync();
            return RedirectToAction("GetCategory");
        }
        public ActionResult EditCategory(int id)
        {
            Category category = db.Categories.Find(id);
            if (category != null)
            {
                return View(category);
            }
            return HttpNotFound();
        }
        [HttpPost]
        public async Task<ActionResult> EditCategory(Category category)
        {
            db.Entry(category).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return RedirectToAction("GetCategory");
        }
        public async Task<ActionResult> DeleteCategory(int id)
        {
            Category category = db.Categories.Find(id);
            if (category != null)
            {
                db.Categories.Remove(category);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("GetCategory");
        }
        public ActionResult GetProduct()
        {
            List<Product> product = db.Products.Include(c =>c.Manufacturer).Include(c => c.StatusProduct).ToList();
            return View(product);
        }
        public ActionResult CreateProduct()
        {
            List<Category> category = db.Categories.ToList();
            SelectList manufacturer = new SelectList(db.Manufacturers, "Id", "Name");
            SelectList statusproduct = new SelectList(db.StatusProducts, "Id", "Name");
            ViewBag.Category = category;
            ViewBag.Manufacturer = manufacturer;
            ViewBag.StatusProduct = statusproduct;
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> CreateProduct(Product product, int[] selectedCategory, HttpPostedFileBase uploadImage)
        {
            if (uploadImage != null)
            {
            byte[] imageData = null;            
            using (var binaryReader = new BinaryReader(uploadImage.InputStream))
            {
                imageData = binaryReader.ReadBytes(uploadImage.ContentLength);
            }
            
            product.Photo = imageData;
            }
            if (selectedCategory != null)
            {                
                foreach (var c in db.Categories.Where(co => selectedCategory.Contains(co.Id)))
                {
                    product.Categories.Add(c);
                }
            }
            db.Products.Add(product);
            await db.SaveChangesAsync();
            return RedirectToAction("GetProduct");
        }
        public ActionResult DetailsProduct(int id)
        {
            ////var works = context.Works
            //       .Select(x => new WorkListView
            //        {
            //            Id = x.Id,
            //            ServerName = x.Server.Name,
            //            ProgramName = x.Program.Name
            //        })
            //       .ToList();

            //var sproduct = db.Products.Select(x => new ProductView
            //{
            //    Id = id,
            //    Name = x.Name,
            //    ManufacturerName = x.Manufacturer.Name,
            //    Photo = x.Photo,
            //    Description = x.Description,
            //    Cost = x.Cost,
            //    StatusProductName = x.StatusProduct.Name
            //}).ToList();
            //var product = sproduct.Find(i => i.Id == id);
            var productt = db.Products.Include(c => c.Manufacturer).Include(c => c.StatusProduct).Include(c => c.Categories).ToList();
            var product = productt.Find(i => i.Id == id);            
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }
        public ActionResult EditProduct(int id)
        {
            Product product = db.Products.Find(id);
            //var productt = db.Products.Include(c => c.Manufacturer).Include(c => c.StatusProduct).Include(c => c.Categories).ToList();
            //var product = productt.Find(i => i.Id == id);
            List<Category> category = db.Categories.ToList();
            SelectList manufacturer = new SelectList(db.Manufacturers, "Id", "Name");
            SelectList statusproduct = new SelectList(db.StatusProducts, "Id", "Name");
            ViewBag.Category = category;
            ViewBag.Manufacturer = manufacturer;
            ViewBag.StatusProduct = statusproduct;
            if(product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }
        [HttpPost]
        public async Task<ActionResult> EditProduct(Product product, int[] selectedCategory, HttpPostedFileBase uploadImage)
        {
            Product newProduct = db.Products.Find(product.Id);
            newProduct.Name = product.Name;
            newProduct.ManufacturerId = product.ManufacturerId;            
            newProduct.Description = product.Description;
            newProduct.Cost = product.Cost;
            newProduct.StatusProductId = product.StatusProductId;
            if (uploadImage != null)
            {
                byte[] imageData = null;
                using (var binaryReader = new BinaryReader(uploadImage.InputStream))
                {
                    imageData = binaryReader.ReadBytes(uploadImage.ContentLength);
                }

                newProduct.Photo = imageData;
            }
            if (selectedCategory != null)
            {
                foreach (var c in db.Categories.Where(co => selectedCategory.Contains(co.Id)))
                {
                    newProduct.Categories.Add(c);
                }
            }
            db.Entry(newProduct).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return RedirectToAction("GetProduct");
        }
        public async Task<ActionResult> DeleteProduct(int id)
        {
            Product product = db.Products.Find(id);
            if (product != null)
            {
                db.Products.Remove(product);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("GetProduct");
        }


        public ActionResult GetPaymentMethod()
        {
            List<PaymentMethod> pm = db.PaymentMethods.ToList();
            return View(pm);
        }
        public ActionResult CreatePaymentMethod()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> CreatePaymentMethod(PaymentMethod pm)
        {
            db.PaymentMethods.Add(pm);
            await db.SaveChangesAsync();
            return RedirectToAction("GetPaymentMethod");
        }
        public async Task<ActionResult> DeletePaymentMethod(int id)
        {
            PaymentMethod pm = db.PaymentMethods.Find(id);
            if (pm != null)
            {
                db.PaymentMethods.Remove(pm);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("GetPaymentMethod");
        }
        public ActionResult GetStatusOrder()
        {
            List<StatusOrder> status = db.StatusOrders.ToList();
            return View(status);
        }
        public ActionResult CreateStatusOrder()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> CreateStatusOrder(StatusOrder status)
        {
            db.StatusOrders.Add(status);
            await db.SaveChangesAsync();
            return RedirectToAction("GetStatusOrder");
        }
        public async Task<ActionResult> DeleteStatusOrder(int id)
        {
            StatusOrder status = db.StatusOrders.Find(id);
            if (status != null)
            {
                db.StatusOrders.Remove(status);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("GetStatusOrder");
        }
        public ActionResult GetTypeDelivery()
        {
            List<TypeDelivery> status = db.TypeDeliveries.ToList();
            return View(status);
        }
        public ActionResult CreateTypeDelivery()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> CreateTypeDelivery(TypeDelivery type)
        {
            db.TypeDeliveries.Add(type);
            await db.SaveChangesAsync();
            return RedirectToAction("GetTypeDelivery");
        }
        public async Task<ActionResult> DeleteTypeDelivery(int id)
        {
            TypeDelivery type = db.TypeDeliveries.Find(id);
            if (type != null)
            {
                db.TypeDeliveries.Remove(type);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("GetTypeDelivery");
        }
    }
}