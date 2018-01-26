using SETZ.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SETZ.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {
        //
        // GET: /Admin/
        DB_A28A4D_setzEntities setzDB = new DB_A28A4D_setzEntities();
        
        public ActionResult Index()
        {
            AdminModel model = new AdminModel();
            model.brands = new List<BrandModel>();
            foreach(var brand in setzDB.Brands)
            {
                BrandModel brandModel = new BrandModel();
                brandModel.brand = brand;
                model.brands.Add(brandModel);
            }
            
            model.sectionModels = new List<SectionModel>();
            foreach (var section in setzDB.Sections.OrderBy(s=>s.Order))
            {
                SectionModel sectionModel = new SectionModel();
                sectionModel.section = section;
                sectionModel.subSectionModels = new List<SubSectionModel>();
                foreach(var subSection in setzDB.SubSections.Where(subs=> subs.SectionID == section.ID).OrderBy(ss => ss.Order))
                {
                    SubSectionModel subSectionModel = new SubSectionModel();
                    subSectionModel.subSection = subSection;
                    subSectionModel.categoryModels = new List<CategoryModel>();

                    foreach (var category in setzDB.Categories.Where(a => a.SubSectionID == subSection.ID).OrderBy(c => c.Order))
                    {
                        CategoryModel categoryModel = new CategoryModel();
                        categoryModel.category = category;
                        categoryModel.subCategories = new List<SubCategory>();
                        foreach (var subCategory in setzDB.SubCategories.Where(b => b.CategoryID == category.ID).OrderBy(sc => sc.Order))
                        {
                            categoryModel.subCategories.Add(subCategory);
                        }
                        subSectionModel.categoryModels.Add(categoryModel);
                    }
                    sectionModel.subSectionModels.Add(subSectionModel);
                }
                model.sectionModels.Add(sectionModel);
            }
            return View(model);
        }


        public ActionResult Picture()
        {
            List<Picture> pictures = setzDB.Pictures.ToList();
            return View(pictures);
        }

        //[Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult CreateBrand()
        {
            BrandModel model = new BrandModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateBrand(AdminModel model)
        {
            
            Brand brand = new Brand();
            brand.Name = model.brandModel.brand.Name;

            setzDB.Brands.Add(brand);
            setzDB.SaveChanges();

            Brand br = setzDB.Brands.ToList().LastOrDefault();

            if (model.brandModel.File != null)
            {
                string fileName = "brand" + br.ID.ToString() + ".jpg";
                br.Image = fileName;
                var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Images/Brand/"), fileName);
                model.brandModel.File.SaveAs(path);
            }

            setzDB.SaveChanges();

            return RedirectToAction("Index", "Admin");
        }

        [HttpGet]
        public ActionResult EditBrand(int? id)
        {
            BrandModel model = new BrandModel();
            model.brand.Name = setzDB.Brands.Find(id).Name;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditBrand(BrandModel model, int? id)
        {
            Brand update = setzDB.Brands.Find(model.brand.ID);
            try
            {
                update.Name = model.brand.Name;


                if (model.File != null)
                {
                    string fileName = "brand" + model.brand.ID.ToString() + ".jpg";
                    var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Images/Brand/"), fileName);
                    model.File.SaveAs(path);
                    update.Image = fileName;
                }
                setzDB.SaveChanges();

                return RedirectToAction("Index", "Admin");
            }
            catch (Exception /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return View(update);
        }


        public ActionResult DeleteBrand(int? id, bool? saveChangesError = false)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            if (saveChangesError.GetValueOrDefault())
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            Brand deleted = setzDB.Brands.Find(id);
            if (deleted == null)
            {
                return HttpNotFound();
            }
            return View(deleted);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteBrand(BrandModel model)
        {
            Brand deleted = setzDB.Brands.Find(model.brand.ID);
            try
            {
                string file = "~/Images/Brand/" + deleted.Image;

                if ((System.IO.File.Exists(file)))
                {
                    System.IO.File.Delete(file);
                }
                setzDB.Brands.Remove(deleted);
                setzDB.SaveChanges();
            }
            catch (DataException/* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                return RedirectToAction("DeleteBrand", new { id = model.brand.ID, saveChangesError = true });
            }
            return RedirectToAction("Index", "Admin");
        }


        [HttpGet]
        public ActionResult CreateSection(int? id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateSection(int? id, Section section)
        {
            setzDB.Sections.Add(section);
            setzDB.SaveChanges();
            return RedirectToAction("Index", "Admin");
        }

        [HttpGet]
        public ActionResult EditSection(int? id)
        {

            return View(setzDB.Sections.Find(id));
        }

        [HttpPost]
        public ActionResult EditSection(Section section, int? id)
        {
            Section update = setzDB.Sections.Find(section.ID);
            try
            {
                update.Name = section.Name;
                update.Order = section.Order;
                update.Visibility = section.Visibility;
                setzDB.SaveChanges();

                return RedirectToAction("Index", "Admin");
            }
            catch (Exception /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return View(update);
        }


        public ActionResult DeleteSection(int? id, bool? saveChangesError = false)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            if (saveChangesError.GetValueOrDefault())
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            Section deleted = setzDB.Sections.Find(id);
            if (deleted == null)
            {
                return HttpNotFound();
            }
            return View(deleted);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteSection(Section section)
        {
            Section deleted = setzDB.Sections.Find(section.ID);
            try
            {
                setzDB.Sections.Remove(deleted);
                setzDB.SaveChanges();
            }
            catch (DataException/* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                return RedirectToAction("DeleteSection", new { id = section.ID, saveChangesError = true });
            }
            return RedirectToAction("Index", "Admin");
        }


        [HttpGet]
        public ActionResult CreateSubSection(int? id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateSubSection(int? id, SubSection subSection)
        {
            subSection.SectionID = id;
            setzDB.SubSections.Add(subSection);
            setzDB.SaveChanges();
            return RedirectToAction("Index", "Admin");
        }

        [HttpGet]
        public ActionResult EditSubSection(int? id)
        {

            return View(setzDB.SubSections.Find(id));
        }

        [HttpPost]
        public ActionResult EditSubSection(SubSection subSection, int? id)
        {
            SubSection update = setzDB.SubSections.Find(subSection.ID);
            try
            {
                update.Name = subSection.Name;
                update.Order = subSection.Order;
                update.Visibility = subSection.Visibility;
                setzDB.SaveChanges();

                return RedirectToAction("Index", "Admin");
            }
            catch (Exception /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return View(update);
        }


        public ActionResult DeleteSubSection(int? id, bool? saveChangesError = false)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            if (saveChangesError.GetValueOrDefault())
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            SubSection deleted = setzDB.SubSections.Find(id);
            if (deleted == null)
            {
                return HttpNotFound();
            }
            return View(deleted);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteSubSection(SubSection subSection)
        {
            SubSection deleted = setzDB.SubSections.Find(subSection.ID);
            try
            {
                setzDB.SubSections.Remove(deleted);
                setzDB.SaveChanges();
            }
            catch (DataException/* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                return RedirectToAction("DeleteSubSection", new { id = subSection.ID, saveChangesError = true });
            }
            return RedirectToAction("Index", "Admin");
        }


        [HttpGet]
        public ActionResult CreateCategory(int? id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateCategory(int? id, Category category)
        {
            category.SubSectionID = id;
            setzDB.Categories.Add(category);
            setzDB.SaveChanges();
            return RedirectToAction("Index", "Admin");
        }

        [HttpGet]
        public ActionResult EditCategory(int? id)
        {

            return View(setzDB.Categories.Find(id));
        }

        [HttpPost]
        public ActionResult EditCategory(Category category, int? id)
        {
            Category update = setzDB.Categories.Find(category.ID);
            try
            {
                update.Name = category.Name;
                update.Order = category.Order;
                update.Visibility = category.Visibility;
                setzDB.SaveChanges();

                return RedirectToAction("Index", "Admin");
            }
            catch (Exception /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return View(update);
        }


        public ActionResult DeleteCategory(int? id, bool? saveChangesError = false)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            if (saveChangesError.GetValueOrDefault())
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            Category deleted = setzDB.Categories.Find(id);
            if (deleted == null)
            {
                return HttpNotFound();
            }
            return View(deleted);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCategory(Category category)
        {
            Category deleted = setzDB.Categories.Find(category.ID);
            try
            {
                setzDB.Categories.Remove(deleted);
                setzDB.SaveChanges();
            }
            catch (DataException/* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                return RedirectToAction("DeleteCategory", new { id = category.ID, saveChangesError = true });
            }
            return RedirectToAction("Index", "Admin");
        }


        [HttpGet]
        public ActionResult CreateSubCategory(int? id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateSubCategory(int? id, SubCategory subcategory)
        {
            subcategory.CategoryID = id;
            setzDB.SubCategories.Add(subcategory);
            setzDB.SaveChanges();
            return RedirectToAction("Index", "Admin");
        }

        [HttpGet]
        public ActionResult EditSubCategory(int? id)
        {

            return View(setzDB.SubCategories.Find(id));
        }

        [HttpPost]
        public ActionResult EditSubCategory(SubCategory subcategory, int? id)
        {
            SubCategory update = setzDB.SubCategories.Find(subcategory.ID);
            try
            {
                update.Name = subcategory.Name;
                update.Order = subcategory.Order;
                update.Visibility = subcategory.Visibility;
                setzDB.SaveChanges();

                return RedirectToAction("Index", "Admin");
            }
            catch (Exception /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return View(update);
        }


        public ActionResult DeleteSubCategory(int? id, bool? saveChangesError = false)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            if (saveChangesError.GetValueOrDefault())
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            SubCategory deleted = setzDB.SubCategories.Find(id);
            if (deleted == null)
            {
                return HttpNotFound();
            }
            return View(deleted);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteSubCategory(SubCategory subCategory)
        {
            SubCategory deleted = setzDB.SubCategories.Find(subCategory.ID);
            try
            {
                setzDB.SubCategories.Remove(deleted);
                setzDB.SaveChanges();
            }
            catch (DataException/* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                return RedirectToAction("DeleteSubCategory", new { id = subCategory.ID, saveChangesError = true });
            }
            return RedirectToAction("Index", "Admin");
        }

        private void PopulateSectionDropDownList(object selectedSection = null)
        {
            List<Section> sectionList = new List<Section>();

            Section man = new Section();
            man.Name = "Мужчинам";
            man.ID = 2;
            sectionList.Add(man);

            Section woman = new Section();
            woman.Name = "Женщинам";
            woman.ID = 1;
            sectionList.Add(woman);

            Section child = new Section();
            child.Name = "Детям";
            child.ID = 3;
            sectionList.Add(child);

            ViewBag.sectionID = new SelectList(sectionList, "ID", "Name", selectedSection);
        }

    }
}
