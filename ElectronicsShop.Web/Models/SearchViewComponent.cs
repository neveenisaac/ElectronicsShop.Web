using ElectronicsShop.DAL.Context;
using ElectronicsShop.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appointments.Web.Models
{
    public class SearchViewComponent : ViewComponent
    {
        private CategoryService _categoryService;
        public SearchViewComponent(CategoryService categoryService)
        {
            _categoryService = categoryService;
            
        }
     
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = _categoryService.GetAllCategory();
            return View(categories);
        }
    }
}
