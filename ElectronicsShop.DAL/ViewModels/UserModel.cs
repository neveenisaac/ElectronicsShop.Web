using System;
using System.Collections.Generic;
using System.Text;

namespace ElectronicsShop.DAL.ViewModels
{
   public class UserModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string fulladdress { get; set; }
        public string phonenumber { get; set; }
        public string birthdate { get; set; }
    }
}
