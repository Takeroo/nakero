using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.ModelBinding;

namespace SETZ.Models
{
    public class UniqueArticulAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DB_A28A4D_setzEntities setzDB = new DB_A28A4D_setzEntities();
            if (value != null)
            {
                string Articul = value.ToString();
                int count = 0;
                if (setzDB.Products.ToList().Count() > 0)
                {
                    count = setzDB.Products.Where(x => x.Articul == Articul).ToList().Count();
                }
                if (count != 0)
                    return new ValidationResult("Этот  артикул  уже зарегистрирована");
                return ValidationResult.Success;
            }
            return new ValidationResult("Просьба предоставить другой артикул");
        }
    }
    public class ProductModel
    {
        public Product product { get; set; }
        public Brand brand { get; set; }
        public List<ProductSize> availableSizes { get; set; }
        public List<ProductColor> availableColours { get; set; }
        public List<Image> images { get; set; }
        public string selectedSize { get; set; }
        public string selectedColour { get; set; }
        public int quantity { get; set; }
    }
    
    public class ProductItemModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public Nullable<int> Price { get; set; }
        public string Articul { get; set; }
        public string Image { get; set; }
        public Nullable<int> Discount { get; set; }
        public Nullable<int> SubSectionID { get; set; }
        public Nullable<int> CategoryID { get; set; }
        public Nullable<int> SubCategoryID { get; set; }
        public Nullable<int> BrandID { get; set; }
        public string BrandName { get; set; }
        public Nullable<double> Rating { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
    }

    public class CreateProductModel
    {
        public int? productID { get; set; }
        [Required(ErrorMessage = "Выберите раздел")]
        [Range(1, int.MaxValue, ErrorMessage = "Выберите раздел")]
        public int? sectionID { get; set; }

        [Required(ErrorMessage = "Выберите подраздел")]
        [Range(1, int.MaxValue, ErrorMessage = "Выберите подраздел")]
        public int? subSectionID { get; set; }

        [Required(ErrorMessage = "Выберите категорию")]
        [Range(1, int.MaxValue, ErrorMessage = "Выберите категорию")]
        public int? categoryID { get; set; }
    
        public int subCategoryID { get; set; }

        public int brandID { get; set; }

        [Required(ErrorMessage = "Просьба, заполнить поле для названия товара")]
        public String name { get; set; }

        [Required(ErrorMessage = "Просьба, заполнить поле для цены товара")]
        [Range(1, int.MaxValue, ErrorMessage = "Просьба, заполнить поле для цены товара")]
        public int? price { get; set; }
        [UniqueArticul]
        [Required(ErrorMessage = "Просьба, заполнить поле для артикула")]
        public String articul { get; set; }
        public String about { get; set; }

        [Required(ErrorMessage = "Просьба, заполнить поле для цены товара")]
        [Range(1, int.MaxValue, ErrorMessage = "Просьба, заполнить поле для цены товара")]
        public int? quantity { get; set; }
        public int? discount { get; set; }
        
        public List<ColorModel> colors { get; set; }
        public List<SizeModel> sizes { get; set; }
        public HttpPostedFileBase File { get; set; }
        public List<HttpPostedFileBase> Files { get; set; }
    }

    public class ColorModel
    {
        public string color { get; set; }
    }
    public class SizeModel
    {
        public string size { get; set; }
    }
    public class SectionModel
    {
        public Section section { get; set; }
        public List<SubSectionModel> subSectionModels { get; set; }
    }
    public class SubSectionModel
    {
        public SubSection subSection { get; set; }
        public List<CategoryModel> categoryModels { get; set; }
    }
    public class CategoryModel
    {
        public Category category { get; set; }
        public List<SubCategory> subCategories { get; set; }
    }
    
    public class MainProductModel
    {
        public PagedList.IPagedList<ProductItemModel> products { get; set; }
    }

    public class SideBarModel
    {
        public List<CategoryModel> categories { get; set; }
        public List<Brand> brands { get; set; }
    }
}