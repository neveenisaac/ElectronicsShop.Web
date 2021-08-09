using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ElectronicsShop.DAL.Entities;
using ElectronicsShop.DAL.ViewModels;
using ElectronicsShop.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
namespace ElectronicsShop.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        #region CTOR & Definitions
        private ProductService _productService;
        private CategoryService _categoryService;
        private readonly IWebHostEnvironment webHostEnvironment;
        public AdminController(CategoryService categoryService, ProductService productService, IWebHostEnvironment hostEnvironment)
        {
            _categoryService = categoryService;
            _productService = productService;
            webHostEnvironment = hostEnvironment;
        }
        #endregion

        #region get
        [Route("Admin/InsertProduct")]
        [HttpGet]
        public IActionResult InsertProduct()
        {
            var product = new ProductViewModel();
            product.categories = _categoryService.GetAllCategory().ToList();
            return View(product);
        }

        [Route("Admin/InsertCategory")]
        [HttpGet]
        public IActionResult InsertCategory()
        {
            return View(new Category());
        }
        #endregion

        #region Insert
        [Route("Admin/InsertProduct")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult save()
        {
            return View();
        }
        [Route("Admin/InsertCategory")]
        [AllowAnonymous]
        [HttpPost]

        public IActionResult Save(Category cat)
        {
            if (ModelState.IsValid)
            {
                var result = _categoryService.InsertCategoryAsync(cat);


                if (result != 200)
                {
                    return Content("error occur try again");
                }
                return Content("save success");
            }
            else
            {

                return View(cat);
            }

        }
        #endregion

        #region UploadImage
        private string UploadedFile(ProductViewModel modelview)
        {
            string uniqueFileName = null;

            if (modelview.imagefile != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + modelview.imagefile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    modelview.imagefile.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
        #endregion

        #region logout
        public async Task<IActionResult> Logout()
        {
            var role = Request.Cookies["Role"];
            if (role == "Admin")
            {
                Response.Cookies.Delete("JWToken", new CookieOptions()
                {
                    Expires = DateTime.Now.AddDays(-1)

                });
                Response.Cookies.Delete("Role", new CookieOptions()
                {
                    Expires = DateTime.Now.AddDays(-1)

                });

                return RedirectToAction("Index", "Home");
            }
            return Unauthorized();

        }
        #endregion
    }
}