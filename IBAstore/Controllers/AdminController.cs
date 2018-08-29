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
using System.Web.Helpers;
using System.Web.Mvc;
using Excel = Microsoft.Office.Interop.Excel;

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
        public async Task<ActionResult> CreateManufacturer(Manufacturer manufacturer)
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
            SelectList parentcategory = new SelectList(db.Categories, "Id", "Name");
            ViewBag.ParentCategory = parentcategory;
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
            SelectList parentcategory = new SelectList(db.Categories, "Id", "Name");
            ViewBag.ParentCategory = parentcategory;
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
            List<Product> product = db.Products.Include(c => c.Manufacturer).Include(c => c.StatusProduct).ToList();
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
            if (product == null)
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
            if (newProduct.StatusProductId == 2)
            {
                var requests = db.ProductRequests.Where(p => p.ProductId == newProduct.Id).Include(u => u.User).ToList();
                if (requests != null)
                {
                    foreach (var r in requests)
                    {
                        WebMail.SmtpServer = "smtp.gmail.com";
                        WebMail.SmtpPort = 587;
                        WebMail.EnableSsl = true;
                        WebMail.UserName = "newsibastore@gmail.com";
                        WebMail.Password = "iba123456";
                        WebMail.From = "newsibastore@gmail.com";
                        WebMail.Send(r.User.Email,
                            "Наличие " + newProduct.Name,
                            "Здравствуйте, " + r.User.Name + ". Товар " + newProduct.Name + ", на который вы оставили заявку в наличии!");
                        db.ProductRequests.Remove(r);
                    }
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
        public PartialViewResult SummaryOrders()
        {
            var orders = db.Orders.Count();
            return PartialView(orders);
        }
        public PartialViewResult SummarySales()
        {
            var sales = db.SaleStats.Count();
            return PartialView(sales);
        }
        public PartialViewResult SummaryUsers()
        {
            return PartialView(UserManager.Users.Count());
        }
        public ActionResult GetOrder(int? status, string id)
        {

            var order = db.Orders.Include(o => o.StatusOrder).Include(o => o.Cart.User);
            if (status != null && status != 0)
            {
                order = order.Where(s => s.StatusOrderId == status);
            }
            if (id != null)
            {
                order = order.Where(u => u.Cart.UserId == id);
            }
            var orders = order.ToList();
            var statusorders = db.StatusOrders.ToList();
            statusorders.Insert(0, new StatusOrder { Name = "Все", Id = 0 });
            SelectList so = new SelectList(statusorders, "Id", "Name");
            ViewBag.StatusOrder = so;
            return View(orders);
        }
        public ActionResult DetailsOrder(int id)
        {
            var orders = db.Orders.Include(o => o.StatusOrder).Include(o => o.PaymentMethod).Include(o => o.TypeDelivery).Include(o => o.Cart.User).ToList();
            var order = orders.Find(o => o.Id == id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }
        public ActionResult EditOrder(int id)
        {
            SelectList status = new SelectList(db.StatusOrders, "Id", "Name");
            ViewBag.StatusOrder = status;
            var orders = db.Orders.Include(o => o.StatusOrder).Include(o => o.PaymentMethod).Include(o => o.TypeDelivery).Include(o => o.Cart.User).ToList();
            var order = orders.Find(o => o.Id == id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }
        [HttpPost]
        public async Task<ActionResult> EditOrder(Order order)
        {            
            db.Entry(order).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return RedirectToAction("GetOrder");
        }
        public async Task<ActionResult> DeleteOrder(int id)
        {
            var order = db.Orders.Find(id);
            if (order != null)
            {
                db.Orders.Remove(order);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("GetOrder");
        }
        public ActionResult GetSale(int? product, DateTime? firstdate, DateTime? seconddate)
        {
            var sale = db.SaleStats.Include(s => s.Product).Include(s => s.User);
            if (firstdate != null)
            {
                sale = sale.Where(s => s.Date > firstdate);
            }
            if (seconddate != null)
            {
                sale = sale.Where(s => s.Date < seconddate);
            }
            if (product != null && product != 0)
            {
                sale = sale.Where(s => s.ProductId == product);
            }
            var sales = sale.ToList();
            var products = db.Products.OrderBy(n => n.Name).ToList();
            products.Insert(0, new Product { Name = "Все", Id = 0 });
            SelectList pr = new SelectList(products, "Id", "Name");
            ViewBag.Products = pr;
            return View(sales);
        }
        public ActionResult GetUserStat()
        {
            return View(UserManager.Users);
        }
        public FileResult GenerateExcelPrice()
        {
            string Path = AppDomain.CurrentDomain.BaseDirectory + @"Prices\";
            string Name = "Price_" + DateTime.Now.ToShortDateString() + ".xlsx";
            string FilePath = Path + Name;            
            Excel.Application excelApp = new Excel.Application();
            excelApp.Workbooks.Add();
            Excel._Worksheet worksheet = excelApp.ActiveSheet;
            worksheet.Cells[1, "A"] = "Id";
            worksheet.Cells[1, "B"] = "Производитель";
            worksheet.Cells[1, "C"] = "Название";
            worksheet.Cells[1, "D"] = "Описание";
            worksheet.Cells[1, "E"] = "Цена";
            worksheet.Cells[1, "F"] = "Статус";
            List<Product> products = db.Products.Include(c => c.Manufacturer).Include(c => c.StatusProduct).ToList();
            int row = 1;
            foreach (var p  in products)
            {
                row++;
                worksheet.Cells[row, "A"] = p.Id.ToString();
                worksheet.Cells[row, "B"] = p.Manufacturer.Name;
                worksheet.Cells[row, "C"] = p.Name;
                worksheet.Cells[row, "D"] = p.Description;
                worksheet.Cells[row, "E"] = p.Cost.ToString() + " руб";
                worksheet.Cells[row, "F"] = p.StatusProduct.Name;                
            }
            worksheet.Range["A1"].AutoFormat(Excel.XlRangeAutoFormat.xlRangeAutoFormatClassic1);            
            worksheet.SaveAs(string.Format(FilePath));            
            excelApp.Quit();
            excelApp.Quit();
            return File(FilePath, "application/vnd.ms-excel", Name);
        }
        public ActionResult CreateNews()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateNews(News news)
        {
            var users = db.Users.Where(u => u.GetNews == true).ToList();
            foreach(var u in users)
            {
                WebMail.SmtpServer = "smtp.gmail.com";
                WebMail.SmtpPort = 587;
                WebMail.EnableSsl = true;
                WebMail.UserName = "newsibastore@gmail.com";
                WebMail.Password = "iba123456";
                WebMail.From = "newsibastore@gmail.com";
                WebMail.Send(u.Email, news.Subject, news.Body);
            }
            return RedirectToAction("Index");
        }
    }    
}