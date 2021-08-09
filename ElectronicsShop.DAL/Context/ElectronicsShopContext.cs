using ElectronicsShop.DAL.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ElectronicsShop.DAL.Context
{
   public class ElectronicsShopContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ElectronicsShopContext(DbContextOptions<ElectronicsShopContext> options) : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }
        public DbSet<Category> categories { set; get; }
        public DbSet<Product> products { set; get; }
        public DbSet<Order> orders { set; get; }
    }
}
