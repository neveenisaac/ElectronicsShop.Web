using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ElectronicsShop.DAL.Entities
{
    public class Product
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int prdID { get; set; }
        public string prdName { get; set; }
        public string prdLocalName { get; set; }
        public int prdPrice { get; set; }
        public int prdQuantity { get; set; }
        public string prdDescription { get; set; }
        public string prdLocalDescription { get; set; }
        public string img { get; set; }

        [ForeignKey("Categorys")]
        public int catID { get; set; }
        public virtual Category Categorys { get; set; }
    }
}
