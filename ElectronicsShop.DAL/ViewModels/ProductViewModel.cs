using ElectronicsShop.DAL.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace ElectronicsShop.DAL.ViewModels
{
   public class ProductViewModel
    {
        public int prdID { get; set; }
        public string prdName { get; set; }
        public int prdPrice { get; set; }
        public int prdQuantity { get; set; }
        public string prdDescription { get; set; }
        public string img { get; set; }
        public IFormFile imagefile { get; set; }
        public int catID { get; set; }
        public List<Category> categories { get; set; } = new List<Category>();
    }
}
