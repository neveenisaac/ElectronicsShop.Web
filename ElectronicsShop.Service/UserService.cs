using ElectronicsShop.BAL.Helper;
using ElectronicsShop.DAL.Context;
using ElectronicsShop.DAL.Entities;
using ElectronicsShop.DAL.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ElectronicsShop.Service
{
    public class UserService
    {
        #region CTOR & Definitions
        private readonly ElectronicsShopContext _context;
        private UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private RoleManager<ApplicationRole> _roleManager;
        private readonly IOptions<ApplicationSettings> _appSettings;
        public UserService(
            ElectronicsShopContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager,
            IOptions<ApplicationSettings> appSettings
            )
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _appSettings = appSettings;
        }
        #endregion

        #region Authentication
        public async Task<object> RegisterAsync(UserModel model)
        {
            var applicationuser = new ApplicationUser();
            applicationuser.UserName = model.UserName;
            applicationuser.Email = model.Email;
            applicationuser.PhoneNumber = model.phonenumber;
            applicationuser.birthdate = model.birthdate;
            applicationuser.fulladdress = model.fulladdress;
            ////add here
            var result = await _userManager.CreateAsync(applicationuser, model.Password);
            if (result.Succeeded)
            {
                //var role = new ApplicationRole()
                //{
                //    Name = model.Role
                //};
                //var results = await _roleManager.CreateAsync(role);
                await _userManager.AddToRoleAsync(applicationuser, model.Role);

                return (result);
            }
            else
            {
                foreach (var item in result.Errors)
                {
                    if (item.Code == "DuplicateUserName")
                    {
                        return ("Duplicate UserName");
                    }
                    if (item.Code == "InvalidUserName")
                    {
                        return ("Invalid UserName");
                    }
                }
                return (result);
            }
        }
        public async Task<object> LoginAsync(LoginModel model)
        {
            var user = new ApplicationUser();
            if (model.LoginUserName != null)
            {
                user = await _userManager.FindByNameAsync(model.LoginUserName);
            }
            else
            {
                user = await _userManager.FindByIdAsync(model.LoginUserId);
            }

            if (user != null && await _userManager.CheckPasswordAsync(user, model.LoginPassword))
            {

                // Get Role assigned to the user 
                var UserRoles = await _userManager.GetRolesAsync(user);
                string userRole = UserRoles[0];
                IdentityOptions _options = new IdentityOptions();
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim("UserID",user.Id.ToString()),
                        new Claim(_options.ClaimsIdentity.RoleClaimType,UserRoles.FirstOrDefault())
                    }),
                    Expires = DateTime.UtcNow.AddDays(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("1234567890123456")),
                    SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(securityToken);

                //var info = await _signInManager.GetExternalLoginInfoAsync();
                //await _signInManager.UpdateExternalAuthenticationTokensAsync(info);

                return (new { jwtoken = token, Role = userRole });
                // return ("Succeeded");
            }
            else
                return ("invalid username or password");
        }
        public async Task<object> InsertRoleAsync(RoleModel model)
        {
            var role = new ApplicationRole()
            {
                Name = model.Name
            };
            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                return (result);
            }
            else
            {
                foreach (var item in result.Errors)
                {
                    if (item.Code == "DuplicateRoleName")
                    {
                        return ("DuplicateRoleName");
                    }
                }
                return (result);
            }
        }
        public async Task<object> ForgotPasswordAsync(ForgotPasswordModel forgotPasswordModel)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordModel.Email);
            if (user == null)
            {
                ///redirect to page of entire Email(ForgotPassword Page)+
                return ("InvalidEmailAddress");
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            token = HttpUtility.UrlEncode(token);
            /// redirect to function Reset password take token and email 
            string url = _appSettings.Value.Client_URL + "/reset_password?token=" + token + "&UserId=" + user.Id;
            bool result = MailHelper.SendMail(forgotPasswordModel.Email, "Password Reset Request", url);
            if (result == true)
            {
                return ("MessageSentToEmail");
            }
            //Mail helper return exception 
            return ("TrySendEmailAgain");
        }

        public async Task<object> ResetPasswordAsync(ResetPasswordModel resetPasswordModel)
        {

            var user = await _userManager.FindByIdAsync(resetPasswordModel.UserId);
            if (user == null)
            {
                return ("ErrorPasswordReset");
            }
            var resetPassResult = await _userManager.ResetPasswordAsync(user, resetPasswordModel.Token, resetPasswordModel.Password);
            if (!resetPassResult.Succeeded)
            {
                return ("InvalidToken");
            }

            return ("PasswordReset");
        }
        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
            /// _logger.LogInformation("User is logged out");

        }


        #endregion
    }
}
