using System;
using System.Collections.Generic;
using System.Text;

namespace ElectronicsShop.DAL.ViewModels
{
  public  class ResetPasswordModel
    {
        public string Password { get; set; }
        public string UserId { get; set; }
        public string Token { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
