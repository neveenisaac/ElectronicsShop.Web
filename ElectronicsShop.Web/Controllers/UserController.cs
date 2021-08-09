using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ElectronicsShop.DAL.Entities;
using ElectronicsShop.DAL.ViewModels;
using ElectronicsShop.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;

namespace ElectronicsShop.Web.Controllers
{
    public class UserController : Controller
    {
        #region CTOR & Definitions
       
        private readonly UserService _userService;
        private UserManager<ApplicationUser> _userManager;
        private readonly IStringLocalizer<UserController> _localizer;
        public UserController(UserService userService, UserManager<ApplicationUser> userManager, IStringLocalizer<UserController> localizer)
        {
            _userService = userService;
            _userManager = userManager;
            _localizer = localizer;

        }
        #endregion

        #region Get
        [Route("User/Login")]
        public IActionResult Login()
        {
            return View(new LoginModel());
        }
        [Route("User/Register")]
        public IActionResult Register()
        {
            return View(new UserModel());
        }

        [Route("User/ForgetPassword")]
        public IActionResult ForgetPassword()
        {
            ForgotPasswordModel model = new ForgotPasswordModel();
            return View(model);
        }
        [Route("reset_password")]
        public IActionResult ResetPassword(string token, string UserId)
        {
            ResetPasswordModel model = new ResetPasswordModel();
            model.Token = token;
            model.UserId = UserId;
            return View(model);
        }
        #endregion

        #region Insert
        [HttpPost]
        [Route("User/Login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var Result = await _userService.LoginAsync(model);
            if (Result.ToString() == "invalid username or password")
            {
                ViewBag.message = Result.ToString();
                return View("Login", model);
            }


            else
            {
                Type type = Result.GetType();
                PropertyInfo[] fields = type.GetProperties();
                object obj = Result;
                string jwtoken = "";
                string Role = "";
                foreach (var field in fields)
                {
                    string name = field.Name;
                    if (name == "jwtoken")
                    {
                        jwtoken = field.GetValue(obj, null).ToString();
                    }
                    else
                    {
                        Role = field.GetValue(obj, null).ToString();
                    }
                }
                Response.Cookies.Append("JWToken"
                , jwtoken,
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(1)
            });
                Response.Cookies.Append("Role"
                 , Role,
                 new CookieOptions
                 {
                     Expires = DateTimeOffset.UtcNow.AddDays(1)
                 });
                //HttpContext.Session.SetString("JWToken", x.ToString());
                if (Role == "Admin")
                {
                    return RedirectToAction("InsertProduct", "Admin");
                }
                else
                {

                    return RedirectToAction("Index", "Home");
                }
            }

        }
        [Route("User/Register")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> RegisterAsync(UserModel model)
        {
            model.Role = "User";
            var x = await _userService.RegisterAsync(model);
            if (x.ToString() == "Succeeded")
            {

                return RedirectToAction("Index", "Home");
            }
            else
            {
                if (x.ToString() == "Duplicate UserName")
                {
                    ViewBag.error = _localizer["Duplicate UserName"];
                }
                else if (x.ToString() == "Invalid UserName")
                {
                    ViewBag.error = _localizer["Invalid UserName"];
                }
                else
                {
                    ViewBag.error = _localizer["PasswordTooShort,PasswordRequiresDigit,special characters"];

                }
                return View("Register", model);
            }
        }

        [Route("User/ForgetPassword")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ForgetPasswordAsync(ForgotPasswordModel model)
        {
            var result = await _userService.ForgotPasswordAsync(model);
            if (result.ToString() == "MessageSentToEmail")
            {
                ViewBag.Result = _localizer["Message Sent To Email"];
            }
            else if (result.ToString() == "TrySendEmailAgain")
            {
                ViewBag.Result = _localizer["Try Send Email Again"];
            }
            else
            {
                ViewBag.Result = _localizer["invalid Email"];
            }

            return View("ForgetPassword", model);
        }
        [Route("User/ResetPassword")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            var result = await _userService.ResetPasswordAsync(model);
            if (result.ToString() == "PasswordReset")
            {
                ViewBag.ResetMsg = _localizer["Password Reset"];

            }
            else if (result.ToString() == "InvalidToken")
            {
                ViewBag.ResetMsg = _localizer["Invalid Token"];
            }
            else
            {
                ViewBag.ResetMsg = _localizer["Error Password Reset"];
            }

            return View("ResetPassword", model);
        }
        #endregion


        #region logout
        [Route("User/Logout")]
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete("JWToken", new CookieOptions()
            {
                Expires = DateTime.Now.AddDays(-1)

            });
            Response.Cookies.Delete("Role", new CookieOptions()
            {
                Expires = DateTime.Now.AddDays(-1)

            });
            HttpContext.Session.SetString("count", JsonConvert.SerializeObject(null));
            HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(null));
            return RedirectToAction("Index", "Home");
        }
        #endregion






    }


}