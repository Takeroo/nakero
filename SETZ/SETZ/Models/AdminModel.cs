using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SETZ.Models
{
  
    public class AdminModel
    {
        public List<SectionModel> sectionModels { get; set; }
        public List<BrandModel> brands { get; set; }
        public Section section { get; set; }
        public SubSection subSection { get; set; }
        public Category category { get; set; }
        public SubCategory subCategory { get; set; }
        public BrandModel brandModel { get; set; }
    }

    public class BrandModel
    {
        public Brand brand { get; set; }
        public HttpPostedFileBase File { get; set; }
    }

    public class PictureModel
    {
        public String Type { get; set; }
        public int SectionID { get; set; }
        public String Link { get; set; }
        public String Description { get; set; }
        public HttpPostedFileBase File { get; set; }
    }
}