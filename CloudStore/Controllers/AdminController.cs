using CloudStore.Models;
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

namespace CloudStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {

        StoreContext db = new StoreContext();
        const string SPInStock = "В наличии";

        public void HierarchyCategory(Product product, Category cat)
        {
            if (cat.ParentCategoryId != null)
            {
                var parentcategory = db.Categories.Find(cat.ParentCategoryId);
                product.Categories.Add(parentcategory);
                HierarchyCategory(product, parentcategory);
            }
        }

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

        #region Roles
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
                    Name = model.RoleName,
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
                return View(new EditRoleModel { Id = role.Id, RoleName = role.Name });
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
                    role.Name = model.RoleName;
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
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(role);
        }

        [HttpPost, ActionName("DeleteRole")]
        public async Task<ActionResult> DeleteRoleConfirmed(string id)
        {
            ApplicationRole role = await RoleManager.FindByIdAsync(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            IdentityResult result = await RoleManager.DeleteAsync(role);
            return RedirectToAction("GetRole");


        }
        public async Task<ActionResult> GiveRole(string id)
        {
            ApplicationRole role = await RoleManager.FindByIdAsync(id);
            string[] memberIDs = role.Users.Select(x => x.UserId).ToArray();

            IEnumerable<ApplicationUser> members = UserManager.Users.Where(x => memberIDs.Any(y => String.Equals(y, x.Id)));

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
                    result = await UserManager.RemoveFromRoleAsync(userId, model.RoleName);
                    if (!result.Succeeded)
                    {
                        return View("Error", result.Errors);
                    }
                }
                return RedirectToAction("GetRole");
            }
            return View("Error", new string[] { "Роль не найдена" });
        }
        #endregion

        #region Users
        public ActionResult GetUser()
        {
            return View(UserManager.Users);
        }
        public async Task<ActionResult> DetailsUser(string id)
        {
            var user = await UserManager.FindByIdAsync(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }

        public async Task<ActionResult> DeleteUser(string id)
        {
            ApplicationUser user = await UserManager.FindByIdAsync(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }

        [HttpPost, ActionName("DeleteUser")]
        public async Task<ActionResult> DeleteUserConfirmed(string id)
        {
            ApplicationUser user = await UserManager.FindByIdAsync(id);
            Cart query = db.Carts.Where(c => String.Equals(c.UserId, id)).FirstOrDefault();
            
            if (user == null)
            {
                return HttpNotFound();
            }

            db.Carts.Remove(query);
            await db.SaveChangesAsync();
            IdentityResult result = await UserManager.DeleteAsync(user);
            return RedirectToAction("GetUser");
        }
        #endregion

        #region Manufacturers
        public async Task<ActionResult> GetManufacturer()
        {
            List<Manufacturer> manufacturer = await db.Manufacturers.ToListAsync();
            return View(manufacturer);
        }

        public ActionResult CreateManufacturer()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateManufacturer(Manufacturer manufacturer)
        {
            var m = db.Manufacturers.Any(p => string.Compare(p.ManufacturerName, manufacturer.ManufacturerName) == 0);
            if (m)
            {
                ModelState.AddModelError("ManufacturerName", "Такой производитель уже существует.");
            }
            if (ModelState.IsValid)
            {
                db.Manufacturers.Add(manufacturer);
                await db.SaveChangesAsync();
                return RedirectToAction("GetManufacturer");
            }
            return View(manufacturer);
        }

        public async Task<ActionResult> EditManufacturer(int id)
        {
            Manufacturer manu = await db.Manufacturers.FindAsync(id);
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
            Manufacturer manu = await db.Manufacturers.FindAsync(id);
            if (manu == null)
            {
                return HttpNotFound();
            }
            return View(manu);
        }

        [HttpPost, ActionName("DeleteManufacturer")]
        public async Task<ActionResult> DeleteManufacturerConfirmed(int id)
        {
            Manufacturer manu = await db.Manufacturers.FindAsync(id);
            if (manu == null)
            {
                return HttpNotFound();
            }
            db.Manufacturers.Remove(manu);
            await db.SaveChangesAsync();
            return RedirectToAction("GetManufacturer");
        }
        #endregion

        #region Status Products
        public async Task<ActionResult> GetStatusProduct()
        {
            List<StatusProduct> status = await db.StatusProducts.ToListAsync();
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
            StatusProduct status = await db.StatusProducts.FindAsync(id);
            if (status == null)
            {
                return HttpNotFound();
            }
            return View(status);
        }

        [HttpPost, ActionName("DeleteStatusProduct")]
        public async Task<ActionResult> DeleteStatusProductConfirmed(int id)
        {
            StatusProduct status = await db.StatusProducts.FindAsync(id);
            if (status == null)
            {
                return HttpNotFound();
            }
            db.StatusProducts.Remove(status);
            await db.SaveChangesAsync();
            return RedirectToAction("GetStatusProduct");
        }
        #endregion

        #region Categories
        public async Task<ActionResult> GetCategory()
        {
            List<Category> category = await db.Categories.ToListAsync();
            return View(category);
        }

        public async Task<ActionResult> CreateCategory()
        {
            SelectList parentcategory = new SelectList(await db.Categories.ToListAsync(), "Id", "CategoryName");
            ViewBag.ParentCategory = parentcategory;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateCategory(Category category)
        {
            var cat = await db.Categories.AnyAsync(c => string.Compare(c.CategoryName, category.CategoryName) == 0);
            if (cat)
            {
                ModelState.AddModelError("CategoryName", "Такая категория уже существует");
            }
            if (ModelState.IsValid)
            {
                db.Categories.Add(category);
                await db.SaveChangesAsync();
                return RedirectToAction("GetCategory");
            }
            return await CreateCategory();
        }

        public async Task<ActionResult> EditCategory(int id)
        {
            SelectList parentcategory = new SelectList(await db.Categories.ToListAsync(), "Id", "CategoryName");
            ViewBag.ParentCategory = parentcategory;
            Category category = await db.Categories.FindAsync(id);
            
            if (category == null)
            {
                return HttpNotFound();
            }

            return View(category);            
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
            Category category = await db.Categories.FindAsync(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        [HttpPost, ActionName("DeleteCategory")]
        public async Task<ActionResult> DeleteCategoryConfirmed(int id)
        {
            Category category = await db.Categories.FindAsync(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            db.Categories.Remove(category);
            await db.SaveChangesAsync();
            return RedirectToAction("GetCategory");
        }
        #endregion

        #region Products
        public async Task<ActionResult> GetProduct()
        {
            List<Product> product = await db.Products.ToListAsync();
            return View(product);
        }

        public async Task<ActionResult> CreateProduct(Product product)
        {
            List<Category> category = await db.Categories.ToListAsync();
            SelectList manufacturer = new SelectList(await db.Manufacturers.ToListAsync(), "Id", "ManufacturerName");
            SelectList statusproduct = new SelectList(await db.StatusProducts.ToListAsync(), "Id", "StatusProductName");
            ViewBag.Category = category;            
            ViewBag.Manufacturer = manufacturer;
            ViewBag.StatusProduct = statusproduct;            
            return View(product);
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

            if (product.Photo == null)
            {
                ModelState.AddModelError("Photo", "Для товара не выбрана фотография.");
            }

            if (selectedCategory != null)
            {
                foreach (var c in db.Categories.Where(co => selectedCategory.Contains(co.Id)))
                {
                    product.Categories.Add(c);
                    HierarchyCategory(product, c);
                }
            }
            else
            {
                ModelState.AddModelError("Categories", "Для товара не выбрана категория.");
            }

            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                await db.SaveChangesAsync();
                return RedirectToAction("GetProduct");
            }

            return await CreateProduct(product);
        }

        public async Task<ActionResult> DetailsProduct(int id)
        {            
            var product = await db.Products.FindAsync(id);

            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }

        public async Task<ActionResult> EditProduct(int id)
        {
            Product product = await db.Products.FindAsync(id);            
            List<Category> category = await db.Categories.ToListAsync();
            SelectList manufacturer = new SelectList(await db.Manufacturers.ToListAsync(), "Id", "ManufacturerName");
            SelectList statusproduct = new SelectList(await db.StatusProducts.ToListAsync(), "Id", "StatusProductName");
            
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
            Product newProduct = await db.Products.FindAsync(product.Id);
            newProduct.ProductName = product.ProductName;
            newProduct.ManufacturerId = product.ManufacturerId;
            newProduct.Description = product.Description;
            newProduct.Cost = product.Cost;
            newProduct.StatusProductId = product.StatusProductId;
            newProduct.Stock = product.Stock;
            
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

            var stp = await db.StatusProducts.FirstOrDefaultAsync(s => string.Equals(s.StatusProductName, SPInStock)); 
            
            if (newProduct.StatusProductId == stp.Id)
            {
                var requests = await db.ProductRequests.Where(p => p.ProductId == newProduct.Id).Include(u => u.User).ToListAsync();
                
                if (requests != null)
                {
                    foreach (var r in requests)
                    {
                        WebMail.SmtpServer = "smtp.gmail.com";
                        WebMail.SmtpPort = 587;
                        WebMail.EnableSsl = true;
                        WebMail.UserName = "newsCloudStore@gmail.com";
                        WebMail.Password = "iba123456";
                        WebMail.From = "newsCloudStore@gmail.com";
                        WebMail.Send(r.User.Email,
                            "Наличие " + newProduct.ProductName,
                            "Здравствуйте, " + r.User.FullName + ". Товар " + newProduct.ProductName + ", на который вы оставили заявку в наличии!");
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
            Product product = await db.Products.FindAsync(id);

            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("DeleteProduct")]
        public async Task<ActionResult> DeleteProductConfirmed(int id)
        {
            Product product = await db.Products.FindAsync(id);

            if (product == null)
            {
                return HttpNotFound();
            }

            db.Products.Remove(product);
            await db.SaveChangesAsync();
            return RedirectToAction("GetProduct");
        }
        #endregion

        #region Payment Methods
        public async Task<ActionResult> GetPaymentMethod()
        {
            List<PaymentMethod> pm = await db.PaymentMethods.ToListAsync();
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
            PaymentMethod pm = await db.PaymentMethods.FindAsync(id);

            if (pm == null)
            {
                return HttpNotFound();
            }

            return View(pm);
        }

        [HttpPost, ActionName("DeletePaymentMethod")]
        public async Task<ActionResult> DeletePaymentMethodConfirmed(int id)
        {
            PaymentMethod pm = await db.PaymentMethods.FindAsync(id);

            if (pm == null)
            {
                return HttpNotFound();
            }

            db.PaymentMethods.Remove(pm);
            await db.SaveChangesAsync();
            return RedirectToAction("GetPaymentMethod");
        }
        #endregion

        #region Statuses Order
        public async Task<ActionResult> GetStatusOrder()
        {
            List<StatusOrder> status = await db.StatusOrders.ToListAsync();
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
            StatusOrder status = await db.StatusOrders.FindAsync(id);

            if (status == null)
            {
                return HttpNotFound();
            }

            return View(status);
        }

        [HttpPost, ActionName("DeleteStatusOrder")]
        public async Task<ActionResult> DeleteStatusOrderConfirmed(int id)
        {
            StatusOrder status = await db.StatusOrders.FindAsync(id);

            if (status == null)
            {
                return HttpNotFound();
            }

            db.StatusOrders.Remove(status);
            await db.SaveChangesAsync();
            return RedirectToAction("GetStatusOrder");
        }
        #endregion

        #region Types Dilevery
        public async Task<ActionResult> GetTypeDelivery()
        {
            List<TypeDelivery> status = await db.TypeDeliveries.ToListAsync();
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
            TypeDelivery type = await db.TypeDeliveries.FindAsync(id);

            if (type == null)
            {
                return HttpNotFound();
            }

            return View(type);
        }

        [HttpPost, ActionName("DeleteTypeDelivery")]
        public async Task<ActionResult> DeleteTypeDeliveryConfirmed(int id)
        {
            TypeDelivery type = await db.TypeDeliveries.FindAsync(id);

            if (type == null)
            {
                return HttpNotFound();
            }

            db.TypeDeliveries.Remove(type);
            await db.SaveChangesAsync();
            return RedirectToAction("GetTypeDelivery");
        }
        #endregion

        #region Summary stats
        public async Task<PartialViewResult> SummaryOrders()
        {
            var orders = await db.Orders.CountAsync();
            return PartialView(orders);
        }

        public async Task<PartialViewResult> SummarySales()
        {
            var sales = await db.SaleStats.CountAsync();
            return PartialView(sales);
        }

        public async Task<PartialViewResult> SummaryUsers()
        {
            return PartialView(await UserManager.Users.CountAsync());
        }
        #endregion

        #region Orders
        public async Task<ActionResult> GetOrder(int? status, string id)
        {

            IQueryable<Order> ordersQuery = db.Orders;

            if (status != null && status != 0)
            {
                ordersQuery = ordersQuery.Where(s => s.StatusOrderId == status);
            }

            if (!string.IsNullOrEmpty(id))
            {
                ordersQuery = ordersQuery.Where(u => u.Cart.UserId == id);
            }

            var orders = await ordersQuery.ToListAsync();
            var statusorders = await db.StatusOrders.ToListAsync();
            statusorders.Insert(0, new StatusOrder { StatusOrderName = "Все", Id = 0 });
            SelectList so = new SelectList(statusorders, "Id", "StatusOrderName");
            ViewBag.StatusOrder = so;
            return View(orders);
        }

        public async Task<ActionResult> DetailsOrder(int id)
        {            
            var order = await db.Orders.FindAsync(id);

            if (order == null)
            {
                return HttpNotFound();
            }

            return View(order);
        }

        public async Task<ActionResult> EditOrder(int id)
        {
            SelectList status = new SelectList(await db.StatusOrders.ToArrayAsync(), "Id", "StatusOrderName");
            ViewBag.StatusOrder = status;
                        
            var order = await db.Orders.FindAsync(id);

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
            Order order = await db.Orders.FindAsync(id);

            if (order == null)
            {
                return HttpNotFound();
            }

            return View(order);
        }

        [HttpPost, ActionName("DeleteOrder")]
        public async Task<ActionResult> DeleteOrderConfirmed(int id)
        {
            Order order = await db.Orders.FindAsync(id);

            if (order == null)
            {
                return HttpNotFound();
            }

            db.Orders.Remove(order);
            await db.SaveChangesAsync();
            return RedirectToAction("GetOrder");
        }
        #endregion

        #region Sale Stat
        public async Task<ActionResult> GetSale(int? product, DateTime? firstdate, DateTime? seconddate)
        {
            IQueryable<SaleStat> salesQuery = db.SaleStats;

            if (firstdate != null)
            {
                salesQuery = salesQuery.Where(s => s.Date > firstdate);
            }

            if (seconddate != null)
            {
                salesQuery = salesQuery.Where(s => s.Date < seconddate);
            }

            if (product != null && product != 0)
            {
                salesQuery = salesQuery.Where(s => s.ProductId == product);
            }

            var sales = await salesQuery.ToListAsync();

            var products = await db.Products.OrderBy(n => n.ProductName).ToListAsync();
            products.Insert(0, new Product { ProductName = "Все", Id = 0 });
            SelectList pr = new SelectList(products, "Id", "ProductName");
            ViewBag.Products = pr;
            
            return View(sales);
        }

        public async Task<ActionResult> GetUserStat()
        {
            return View(await UserManager.Users.ToListAsync());
        }
        #endregion

        #region Excel price
        public async Task<FileResult> GenerateExcelPrice()
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
            List<Product> products = await db.Products.ToListAsync();
            int row = 1;
            foreach (var p in products)
            {
                row++;
                worksheet.Cells[row, "A"] = p.Id.ToString();
                worksheet.Cells[row, "B"] = p.Manufacturer.ManufacturerName;
                worksheet.Cells[row, "C"] = p.ProductName;
                worksheet.Cells[row, "D"] = p.Description;
                worksheet.Cells[row, "E"] = p.Cost.ToString() + " руб";
                worksheet.Cells[row, "F"] = p.StatusProduct.StatusProductName;
            }
            worksheet.Range["A1"].AutoFormat(Excel.XlRangeAutoFormat.xlRangeAutoFormatClassic1);
            worksheet.SaveAs(string.Format(FilePath));
            excelApp.Quit();
            
            return File(FilePath, "application/vnd.ms-excel", Name);
        }
        #endregion

        #region News
        public ActionResult CreateNews()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateNews(News news)
        {
            var users = await db.Users.Where(u => u.GetNews == true).ToListAsync();
            foreach (var u in users)
            {
                WebMail.SmtpServer = "smtp.gmail.com";
                WebMail.SmtpPort = 587;
                WebMail.EnableSsl = true;
                WebMail.UserName = "newsCloudStore@gmail.com";
                WebMail.Password = "iba123456";
                WebMail.From = "newsCloudStore@gmail.com";
                WebMail.Send(u.Email, news.Subject, news.Body);
            }
            return RedirectToAction("Index");
        }
        #endregion
    }
}