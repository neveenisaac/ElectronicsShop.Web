using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ElectronicsShop.DAL.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string fulladdress { get; set; }
        public string birthdate { get; set; }
    }
}
