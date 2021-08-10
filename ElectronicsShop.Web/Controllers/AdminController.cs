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
        private readonly UserService _userService;
        public AdminController(CategoryService categoryService, ProductService productService, IWebHostEnvironment hostEnvironment, UserService userService)
        {
            _categoryService = categoryService;
            _productService = productService;
            webHostEnvironment = hostEnvironment;
            _userService = userService;
        }
        #endregion

        #region get
        [Route("Admin/InsertProduct")]
        [HttpGet]
        public IActionResult InsertProduct()
        {
            var product = new Product();
            ViewBag.categories = _categoryService.GetAllCategory();
            return View(product);
        }

        [Route("Admin/InsertCategory")]
        [HttpGet]
        public IActionResult InsertCategory()
        {
            return View(new Category());
        }
        [Route("Admin/RegisterAdmin")]
        public IActionResult RegisterAdmin()
        {
            var role = Request.Cookies["Role"];
            if (role == "Admin")
            {
                UserModel model = new UserModel();
                return View(model);
            }
            return Unauthorized();
        }
        #endregion

        #region Insert
        [Route("Admin/InsertProduct")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult save(Product pro)
        {
            if (ModelState.IsValid)
            {
                var result = _productService.InsertProductAsync(pro);


                if (result != 200)
                {
                    return Content("error occur try again");
                }
                return Content("save success");
            }
            else
            {

                return View(pro);
            }
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
        [Route("Admin/RegisterAdmin")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> RegisterAdminAsync(UserModel model)
        {
            var role = Request.Cookies["Role"];
            if (role == "Admin")
            {
                model.Role = "Admin";
                var x = await _userService.RegisterAsync(model);
                if (x.ToString() == "Succeeded")
                { 
                    return RedirectToAction("InsertProduct", "Admin");
                }
                else
                {

                    if (x.ToString() == "Duplicate UserName")
                    {
                        ViewBag.error = "Duplicate UserName";
                    }
                    else if (x.ToString() == "Invalid UserName")
                    {
                        ViewBag.error = "Invalid UserName";
                    }
                    else
                    {
                        ViewBag.error= "PasswordTooShort,PasswordRequiresDigit,special characters";

                    }
                    return View("RegisterAdmin", model);
                }
            }
            return Unauthorized();
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