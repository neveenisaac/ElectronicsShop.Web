using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElectronicsShop.DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
namespace ElectronicsShop.Web.Controllers
{
    public class AddToCartController : Controller
    {
        [HttpGet]
        [Route("AddToCart/Add")]
        public IActionResult Add(Product pro)
        {
            var card = HttpContext.Session.GetString("cart");
            List<Product> li = new List<Product>();
            if (card == null)
            {
                li.Add(pro);
                HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(li));
                ViewBag.cart = li.Count();
                HttpContext.Session.SetString("count", JsonConvert.SerializeObject(1));
           
            }
            else
            {
                var products = JsonConvert.DeserializeObject<List<Product>>(card);
                li.AddRange(products);
                li.Add(pro);
                HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(li));
                ViewBag.cart = li.Count();
                var oldcount = HttpContext.Session.GetString("count");
                var GetOldcount = JsonConvert.DeserializeObject<int>(oldcount);
                HttpContext.Session.SetString("count", JsonConvert.SerializeObject(GetOldcount + 1));
            }
            return RedirectToAction("Index", "Home");
        }
        public ActionResult Myorder()
        {
            var card = HttpContext.Session.GetString("cart");
            if (card != null)
            {
                var products = JsonConvert.DeserializeObject<List<Product>>(card);
                return View(products);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            

        }
        public ActionResult Remove(Product pro)
        {
            var card = HttpContext.Session.GetString("cart");
            var products = JsonConvert.DeserializeObject<List<Product>>(card);
            products.RemoveAll(x => x.prdID == pro.prdID);
            HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(products));

            var oldcount = HttpContext.Session.GetString("count");
            var GetOldcount = JsonConvert.DeserializeObject<int>(oldcount);
            HttpContext.Session.SetString("count", JsonConvert.SerializeObject(GetOldcount - 1));
            return RedirectToAction("Myorder", "AddToCart");

        }
    }
}