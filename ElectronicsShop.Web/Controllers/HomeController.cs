using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ElectronicsShop.Web.Models;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;
using ElectronicsShop.Service;
using ElectronicsShop.DAL.Entities;
using ElectronicsShop.DAL.ViewModels;

namespace ElectronicsShop.Web.Controllers
{
    public class HomeController : Controller
    {
        #region CTOR & Definitions
        private readonly ILogger<HomeController> _logger;
        private ProductService _productService;
        public HomeController(ILogger<HomeController> logger , ProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }
        #endregion

        #region Get
        public IActionResult Index(int pagenumber = 1)
        {
                var products = _productService.GetAllProduct();
                var ProductPagelist = paginatedList<Product>.CreateAsync(products, pagenumber, 4);
     
              return View(ProductPagelist);
        }
        [Route("Home/ViewDetails")]
        public IActionResult ViewDetails(int id)
        {
            var product = _productService.GetProductbyId(id);
            return View(product);
        }

        #endregion

        #region search Product With CategoryId
        [HttpGet]
        [Route("Home/filterProduct")]
        public IActionResult filterProduct(int catId)
        {
            var products = _productService.GetProductByCatId(catId);
            var Product = paginatedList<Product>.CreateAsync(products, 1, 5);
            return View("Index", Product);
        }
        #endregion

        #region changeLanguage 
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );
            Response.Cookies.Append("SelectLanguage"
              , culture,
              new CookieOptions
              {
                  Expires = DateTimeOffset.UtcNow.AddYears(1)
              }
          );

            return LocalRedirect(returnUrl);
        }
        #endregion

    }

}
