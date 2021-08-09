using ElectronicsShop.BAL.UOW;
using ElectronicsShop.DAL.Context;
using ElectronicsShop.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ElectronicsShop.Service
{
   public class CategoryService
    {
        #region CTOR & Definitions

        private UnitOfWork unitOfWork;
        public CategoryService(ElectronicsShopContext context)
        {
            this.unitOfWork = new UnitOfWork(context);
        }
        #endregion

        #region insert
        public int InsertCategoryAsync(Category model)
        {

            unitOfWork.Repository<Category>().Insert(model);
            var result = unitOfWork.Save();
            return result;
        }
        #endregion

        #region Get
        public IEnumerable<Category> GetAllCategory()
        {
            return (unitOfWork.CategoryRepository.Get());
        }


        #endregion
    }
}
