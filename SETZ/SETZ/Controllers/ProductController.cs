using SETZ.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace SETZ.Controllers
{
    public class ProductController : Controller
    {
        DB_A28A4D_setzEntities setzDB = new DB_A28A4D_setzEntities();
        // GET: Product
        
        
        public ActionResult Index(int? page, int section, int? subSection, int? category, int? subCategory, string sortType, int? brand)
        {
            MainProductModel model = new MainProductModel();


            ViewBag.Section = section;
            ViewBag.SubSection = subSection;
            ViewBag.CurrentCategory = category;
            if (brand != null)
            {
                ViewBag.CurrentBrand = brand;
            }
            else
            {
                ViewBag.CurrentBrand = 0;
            }
            ViewBag.CurrentSubCategory = subCategory;
            ViewBag.CurrentSort = sortType;

            List<ProductItemModel> productsList = new List<ProductItemModel>();
            foreach (var prod in setzDB.Products.Where(g => g.SectionID == section).OrderByDescending(i => i.ID))
            {
                ProductItemModel pr = new ProductItemModel();
                pr.ID = prod.ID;
                pr.Name = prod.Name;
                pr.Articul = prod.Articul;
                pr.Price = prod.Price;
                pr.Discount = prod.Discount;
                pr.Date = prod.Date;
                pr.SubSectionID = prod.SubSectionID;
                pr.CategoryID = prod.CategoryID;
                pr.SubCategoryID = prod.SubCategoryID;
                pr.BrandID = prod.BrandID;
                pr.Rating = prod.Rating;
                pr.BrandName = "Brand";
                if(setzDB.Brands.Find(prod.BrandID)!=null){
                   pr.BrandName = setzDB.Brands.Find(prod.BrandID).Name;
                }
                pr.Image = "default-img.jpg";
                var relativePath = "~/Images/Product/" + prod.Image;
                var absolutePath = HttpContext.Server.MapPath(relativePath);
                if (System.IO.File.Exists(absolutePath))
                    pr.Image = prod.Image;
                productsList.Add(pr);
            }

            IEnumerable<ProductItemModel> products = productsList;
            
            int subS = subSection ?? 0;
            if (subS > 0)
            {
                products = products.Where(s => s.SubSectionID == subS);
            }
            
            int cat = category ?? 0;
            if (cat > 0)
            {
                products = products.Where(s => s.CategoryID == cat);
            }
            

            int sub = subCategory ?? 0;
            if (sub > 0)
            {
                products = products.Where(s => s.SubCategoryID == sub);
            }

            
            int b = brand ?? 0;
            if (b > 0)
            {
                products = products.Where(s => s.BrandID == b);
            }

            switch (sortType)
            {
                case "price_desc":
                    products = products.OrderByDescending(s => s.Price);
                    break;
                case "price":
                    products = products.OrderBy(s => s.Price);
                    break;
                case "discount":
                    products = products.OrderBy(s => s.Discount);
                    break;
            }

            int pageSize = 20;
            int pageNumber = (page ?? 1);
            model.products = products.ToPagedList(pageNumber, pageSize);
            return View(model);
        }
        
        
        public ActionResult Product()
        {
            List<ProductModel> model = new List<ProductModel>();
            foreach(var pr in setzDB.Products.ToList())
            {
                ProductModel mod = new ProductModel();
                mod.product = pr;
                mod.brand = setzDB.Brands.Find(pr.BrandID);
                mod.product.Image = "default-img.jpg";
                var relativePath = "~/Images/Product/" + pr.Image;
                var absolutePath = HttpContext.Server.MapPath(relativePath);
                if (System.IO.File.Exists(absolutePath))
                    mod.product.Image = pr.Image;
                model.Add(mod);
            }
            return View(model);
        }

        public ActionResult Detail(int id)
        {
            ProductModel model = new ProductModel();
            Product product = setzDB.Products.Find(id);
            model.product = product;
            if (product.BrandID != null && product.BrandID > 0)
            {
                model.brand = setzDB.Brands.Find(product.BrandID);
            }
            model.availableColours = new List<ProductColor>();
            model.availableSizes = new List<ProductSize>();

            foreach (var color in setzDB.ProductColors.Where(c => c.ProductID == product.ID).ToList()) {
                model.availableColours.Add(color);
            }
            foreach (var size in setzDB.ProductSizes.Where(c => c.ProductID == product.ID).ToList())
            {
                model.availableSizes.Add(size);
            }


            var relativePath = "~/Images/Product/" + product.Image;
            var absolutePath = HttpContext.Server.MapPath(relativePath);
            if (System.IO.File.Exists(absolutePath))
            {
                model.product.Image = product.Image;
            }
            else
            {
                model.product.Image = "default-img.jpg";
            }

            model.images = new List<Image>();
            foreach(var image in setzDB.Images.Where(i=>i.ProductID == product.ID).ToList())
            {
                model.images.Add(image);
            }
            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Detail(ProductModel model, int? id)
        {
            if(model.quantity == 0)
            {
                model.quantity = 1;
            }
            
            var identity = (System.Web.HttpContext.Current.User as SETZ.MyPrincipal).Identity as SETZ.MyIdentity;
            int userID = identity.User.ID;
            Product product = setzDB.Products.Find(id);

            if(model.selectedColour == null && setzDB.ProductColors.Where(c => c.ProductID == product.ID).Count()>1)
            {
                return RedirectToAction("Detail", "Product", new { @id = id, @error = "Пожалуйста, выберите цвет товара" });
            }
            if(model.selectedSize == null && setzDB.ProductSizes.Where(c => c.ProductID == product.ID).Count() > 1)
            {
                return RedirectToAction("Detail", "Product", new { @id = id, @error = "Пожалуйста, выберите размер товара" });
            }

            if(setzDB.Carts.Where(c => c.UserID == userID).Count() == 0)
            {
                Cart newCart = new Cart();
                newCart.UserID = userID;
                newCart.NumberOfProducts = 0;
                newCart.Date = DateTime.Now;
                newCart.TotalPrice = 0;
                setzDB.Carts.Add(newCart);
                setzDB.SaveChanges();
            }

            Cart cart = setzDB.Carts.Where(c => c.UserID == userID).FirstOrDefault();

            cart.NumberOfProducts++;
            cart.TotalPrice += model.quantity* product.Price;

            CartItem cartItem = new CartItem();
            cartItem.Price = model.quantity * product.Price;
            cartItem.Size = model.selectedSize;
            cartItem.Colour = model.selectedColour;
            cartItem.CartID = cart.ID;
            cartItem.ProductID = product.ID;
            cartItem.Name = product.Name;
            cartItem.Articul = product.Articul;
            cartItem.Quantity = model.quantity;

            if (model.selectedColour == null && setzDB.ProductColors.Where(c => c.ProductID == product.ID).Count()>0)
            {
                cartItem.Colour = setzDB.ProductColors.Where(c => c.ProductID == product.ID).FirstOrDefault().Hex;
            }

            if (model.selectedSize == null && setzDB.ProductSizes.Where(c => c.ProductID == product.ID).Count() > 0)
            {
                cartItem.Size = setzDB.ProductSizes.Where(c => c.ProductID == product.ID).FirstOrDefault().Size;
            }

            setzDB.CartItems.Add(cartItem);
            setzDB.SaveChanges();

            return RedirectToAction("Detail", "Product", new { @id = id, @error = "Ваш товар добавлен в корзину!" });
        }

        //[Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Create()
        {
            PopulateSectionDropDownList();
            PopulateBrandDropDownList();
            CreateProductModel model = new CreateProductModel();
            model.colors = new List<ColorModel>();
            model.sizes = new List<SizeModel>();
            return View(model);
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(CreateProductModel model)
        {

            PopulateSectionDropDownList(model.sectionID);
            PopulateBrandDropDownList(model.brandID);
            
            Product product = new Product();
            product.Name = model.name;
            product.Price = model.price??0;
            product.Articul = model.articul;
            product.About = model.about;
            
            product.Discount = model.discount ?? 0;
            product.MinQuantity = model.quantity;

            product.SectionID = model.sectionID;
            product.SubSectionID = model.subSectionID;
            product.CategoryID = model.categoryID;
            product.SubCategoryID = model.subCategoryID;
            product.BrandID = model.brandID;

            product.Rating = 0;
            product.RatingCount = 0;
            product.OrderCount = 0;
            product.ViewCount = 0;

            product.Date = DateTime.Now;
            setzDB.Products.Add(product);
            setzDB.SaveChanges();

            Product saved = setzDB.Products.ToList().LastOrDefault();

            if (model.File != null)
            {
                string fileName = "product" + saved.ID.ToString() + ".jpg";
                saved.Image = fileName;
                var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Images/Product/"), fileName);
                model.File.SaveAs(path);
            }

            foreach (var file in model.Files)
            {
                if (file != null)
                {
                    Image imageFile = new Image();
                    imageFile.ProductID = saved.ID;
                    setzDB.Images.Add(imageFile);
                    setzDB.SaveChanges();
                    Image savedImage = setzDB.Images.ToList().LastOrDefault();

                    string fileName = "iamge" + savedImage.ID + "product" + saved.ID.ToString() + ".jpg";
                    savedImage.Path = fileName;
                    var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Images/Imgs/"), fileName);
                    file.SaveAs(path);
                    setzDB.SaveChanges();
                }
            }

            updateColros(saved.ID, model.colors);
            updateSizes(saved.ID, model.sizes);

            setzDB.SaveChanges();

            return RedirectToAction("Create", "Product");
        }

        //[Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult AddImages()
        {
            List<HttpPostedFileBase> Files = new List<HttpPostedFileBase>();
            return View(Files);
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult AddImages(int? id, List<HttpPostedFileBase> Files)
        {
            Product saved = setzDB.Products.Find(id);

            foreach (var file in Files)
            {
                if (file != null)
                {
                    Image imageFile = new Image();
                    imageFile.ProductID = saved.ID;
                    setzDB.Images.Add(imageFile);
                    setzDB.SaveChanges();
                    Image savedImage = setzDB.Images.ToList().LastOrDefault();

                    string fileName = "iamge" + savedImage.ID + "product" + saved.ID.ToString() + ".jpg";
                    savedImage.Path = fileName;
                    var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Images/Imgs/"), fileName);
                    file.SaveAs(path);
                    setzDB.SaveChanges();
                }
            }

            return RedirectToAction("Edit", "Product", new { id = saved.ID});
        }

        //[Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            CreateProductModel model = new CreateProductModel();
            Product update = setzDB.Products.Find(id);

            PopulateSectionDropDownList(update.SectionID);
            PopulateBrandDropDownList(update.BrandID);
            model.productID = update.ID;
            model.name = update.Name;
            model.price = update.Price;
            model.articul = update.Articul;
            model.about = update.About;

            model.discount = update.Discount;
            model.quantity = update.MinQuantity;

            model.sectionID = update.SectionID;
            model.subSectionID = update.SubSectionID;
            model.categoryID = update.CategoryID;

            model.subCategoryID = update.SubCategoryID ?? 0;
            model.brandID = update.BrandID ?? 0;

            model.colors = new List<ColorModel>();
            foreach(var color in setzDB.ProductColors.Where(x=> x.ProductID == id))
            {
                ColorModel col = new ColorModel();
                col.color = color.Hex;
                model.colors.Add(col);
            }

            model.sizes = new List<SizeModel>();
            foreach (var size in setzDB.ProductSizes.Where(x => x.ProductID == id))
            {
                SizeModel siz = new SizeModel();
                siz.size = size.Size;
                model.sizes.Add(siz);
            }

            return View(model);
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(CreateProductModel model, int? id)
        {
            Product product = setzDB.Products.Find(id);
            product.Name = model.name;
            product.Price = model.price ?? 0;
            product.Articul = model.articul;
            product.About = model.about;
            
            product.Discount = model.discount;
            product.MinQuantity = model.quantity;

            product.SectionID = model.sectionID;
            product.SubSectionID = model.subSectionID;
            product.CategoryID = model.categoryID;
            product.SubCategoryID = model.subCategoryID;
            product.BrandID = model.brandID;

            if (model.File != null)
            {
                string fileName = "product" + product.ID.ToString() + ".jpg";
                product.Image = fileName;
                var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Images/Product/"), fileName);
                model.File.SaveAs(path);
            }
            setzDB.SaveChanges();

            updateColros(product.ID, model.colors);
            updateSizes(product.ID, model.sizes);

            return RedirectToAction("Detail", "Product", new { id = product.ID});
        }

        //[Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            if (saveChangesError.GetValueOrDefault())
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            Product deleted = setzDB.Products.Find(id);
            if (deleted == null)
            {
                return HttpNotFound();
            }
            return View(deleted);
        }


        //[Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            Product deleted = setzDB.Products.Find(id);
            int section = deleted.SectionID ?? 1;
            try
            {
                string file = "~/Images/Product/" + deleted.Image;

                if ((System.IO.File.Exists(file)))
                {
                    System.IO.File.Delete(file);
                }
                updateColros(deleted.ID, new List<ColorModel>());
                updateSizes(deleted.ID, new List<SizeModel>());
                deleteImagesForProduct(deleted.ID);
                setzDB.Products.Remove(deleted);
                setzDB.SaveChanges();

            }
            catch (DataException/* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
            return RedirectToAction("Index", "Product", new { section = section });
        }

        public ActionResult DeleteImgs(int? id)
        {
            List<Image> images = new List<Image>();

            foreach(var img in setzDB.Images.Where(i => i.ProductID == id))
            {
                images.Add(img);
            }
            return View(images);
        }

        //[Authorize(Roles = "Admin")]
        public void DeleteImage(int id)
        {
            Image deleted = setzDB.Images.Find(id);
            try
            {
                string file = "~/Images/Imgs/" + deleted.Path;

                if ((System.IO.File.Exists(file)))
                {
                    System.IO.File.Delete(file);
                }
                setzDB.Images.Remove(deleted);
                setzDB.SaveChanges();

            }
            catch (DataException/* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
            }
            Response.Redirect(Request.UrlReferrer.ToString());
        }

        private void PopulateSectionDropDownList(object selectedSection = null)
        {
            var sectionQuery = from d in setzDB.Sections
                              orderby d.Name
                              select d;
            ViewBag.sectionID = new SelectList(sectionQuery, "ID", "Name", selectedSection);
        }

        public JsonResult SubSectionList(int? id)
        {
            var subSectionQuery = from d in setzDB.SubSections
                                where d.SectionID == id
                                orderby d.Name
                                select d;
            ViewBag.SubSectionID = new SelectList(subSectionQuery.ToArray(), "ID", "Name");
            return Json(ViewBag.SubSectionID, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CategoryList(int? id)
        {
            var categoryQuery = from d in setzDB.Categories
                                where d.SubSectionID == id
                                orderby d.Name
                                select d;
            ViewBag.CategoryID = new SelectList(categoryQuery.ToArray(), "ID", "Name");


            return Json(ViewBag.CategoryID, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SubCategoryList(int? id)
        {
            var subCategoryQuery = from d in setzDB.SubCategories
                                   where d.CategoryID == id
                                   orderby d.Name
                                   select d;
            ViewBag.SubCategoryID = new SelectList(subCategoryQuery.ToArray(), "ID", "Name");


            return Json(ViewBag.SubCategoryID, JsonRequestBehavior.AllowGet);
        }

        private void PopulateBrandDropDownList(object selectedBrand = null)
        {
            var brandQuery = from d in setzDB.Brands
                             orderby d.Name
                             select d;
            ViewBag.brandID = new SelectList(brandQuery, "ID", "Name", selectedBrand);
        }

        private void updateSizes(int id, List<SizeModel> model)
        {
            List<ProductSize> sizes = setzDB.ProductSizes.Where(x => x.ProductID == id).ToList();
            foreach (var size in sizes)
            {
                setzDB.ProductSizes.Remove(size);
                setzDB.SaveChanges();
            }

            if (model != null)
            {
                foreach (var item in model)
                {
                    if (!string.IsNullOrEmpty(item.size))
                    {
                        ProductSize siz = new ProductSize();
                        siz.ProductID = id;
                        siz.Size = item.size;
                        setzDB.ProductSizes.Add(siz);
                        setzDB.SaveChanges();
                    }
                }
            }
            
        }

        private void updateColros(int id, List<ColorModel> model)
        {
            List<ProductColor> colors = setzDB.ProductColors.Where(x => x.ProductID == id).ToList();
            foreach(var color in colors)
            {
                setzDB.ProductColors.Remove(color);
                setzDB.SaveChanges();
            }

            if (model !=null)
            {
                foreach (var item in model)
                {
                    if (!string.IsNullOrEmpty(item.color))
                    {
                        ProductColor col = new ProductColor();
                        col.ProductID = id;
                        col.Hex = item.color;
                        setzDB.ProductColors.Add(col);
                        setzDB.SaveChanges();
                    }
                }
            }
        }

        public void deleteImagesForProduct(int id)
        {
            int count = setzDB.Images.Where(im => im.ProductID == id).Count();
            for (int i = 0; i < count; i++)
            {
                Image deleted = setzDB.Images.Where(im => im.ProductID == id).FirstOrDefault();
                string file = "~/Images/Imgs/" + deleted.Path;

                if ((System.IO.File.Exists(file)))
                {
                    System.IO.File.Delete(file);
                }
                setzDB.Images.Remove(deleted);
                setzDB.SaveChanges();
            }

        }
    }
}