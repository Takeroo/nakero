using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SETZ.Models;

namespace SETZ.Controllers
{
    public class HomeController : Controller
    {
        DB_A28A4D_setzEntities setzDB = new DB_A28A4D_setzEntities();

        public ActionResult Index()
        {
            List<ProductModel> model = new List<ProductModel>();
            foreach (var pr in setzDB.Products.OrderByDescending(p=>p.ID).Take(8).ToList())
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

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult NavBar()
        {
            return View(generateNavBar());
        }

        public ActionResult SideBar(int? subSection, int? section)
        {
            return View(generateSideBar(subSection, section));
        }

        public SideBarModel generateSideBar(int? subSection, int ? section)
        {
            SideBarModel model = new SideBarModel();
            model.brands = new List<Brand>();
            model.categories = new List<CategoryModel>();
            
            if (subSection != null && subSection > 0)
            {
                foreach (var categ in setzDB.Categories.Where(c => c.SubSectionID == subSection))
                {
                    CategoryModel categoryModel = new CategoryModel();
                    categoryModel.category = categ;
                    categoryModel.subCategories = new List<SubCategory>();
                    foreach (var subc in setzDB.SubCategories.Where(s => s.CategoryID == categ.ID))
                    {
                        categoryModel.subCategories.Add(subc);
                    }
                    model.categories.Add(categoryModel);
                }
            }
            else
            {
                foreach (var subs in setzDB.SubSections.Where(s => s.SectionID == section))
                {
                    foreach (var categ in setzDB.Categories.Where(c => c.SubSectionID == subs.ID))
                    {
                        CategoryModel categoryModel = new CategoryModel();
                        categoryModel.category = categ;
                        categoryModel.subCategories = new List<SubCategory>();
                        foreach (var subc in setzDB.SubCategories.Where(s => s.CategoryID == categ.ID))
                        {
                            categoryModel.subCategories.Add(subc);
                        }
                        model.categories.Add(categoryModel);
                    }
                }
            }

            model.brands = setzDB.Brands.OrderByDescending(b=>b.ID).Take(10).ToList();
            return model;
        }

        public List<SectionModel> generateNavBar()
        {
            List < SectionModel > sectionModelList = new List<SectionModel>();
            foreach(var section in setzDB.Sections.OrderBy(s => s.Order))
            {
                if (section.Visibility)
                {
                    SectionModel sectionModel = new SectionModel();
                    sectionModel.section = section;
                    sectionModel.subSectionModels = new List<SubSectionModel>();
                    foreach (var subSection in setzDB.SubSections.Where(subs => subs.SectionID == section.ID).OrderBy(ss => ss.Order))
                    {
                        if (subSection.Visibility)
                        {
                            SubSectionModel subSectionModel = new SubSectionModel();
                            subSectionModel.subSection = subSection;
                            subSectionModel.categoryModels = new List<CategoryModel>();
                            foreach (var category in setzDB.Categories.Where(c => c.SubSectionID == subSection.ID).OrderBy(c => c.Order))
                            {
                                if (category.Visibility)
                                {
                                    CategoryModel categoryModel = new CategoryModel();
                                    categoryModel.category = category;
                                    //categoryModel.subCategories = setzDB.SubCategories.Where(s => s.CategoryID == category.ID).ToList();
                                    subSectionModel.categoryModels.Add(categoryModel);
                                }
                            }
                            sectionModel.subSectionModels.Add(subSectionModel);
                        }
                    }
                    sectionModelList.Add(sectionModel);
                }
            }
            return sectionModelList;
        }
    }
}