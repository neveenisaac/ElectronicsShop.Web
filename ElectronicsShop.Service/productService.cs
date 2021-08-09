using ElectronicsShop.DAL.Context;
using System;
using System.Collections.Generic;
using System.Text;
using ElectronicsShop.BAL.UOW;
using ElectronicsShop.DAL.Entities;
using System.Linq;


namespace ElectronicsShop.Service
{
  public  class ProductService
    {
        #region CTOR & Definitions

        private UnitOfWork unitOfWork;
        public ProductService(ElectronicsShopContext context)
        {
            this.unitOfWork = new UnitOfWork(context);
        }
        #endregion

        #region Get
        public IEnumerable<Product> GetAllProduct()
        {
            return (unitOfWork.ProductRepository.Get());
        }

        public Product GetProductbyId(int id)
        {
         var product=  unitOfWork.ProductRepository.Get(filter: c => c.prdID == id).FirstOrDefault();
            return product;
        }
        public IEnumerable<Product> GetProductByCatId(int id)
        {
            IEnumerable<Product> products;
            if ( id != 0)

                products = unitOfWork.ProductRepository.Get(filter: c => c.catID == id);
         
            else
            {
                products = unitOfWork.ProductRepository.Get();
            }

            return (products);
        }

        #endregion

        #region insert
        public int InsertProductAsync(Product model)
        {

            unitOfWork.Repository<Product>().Insert(model);
            var result = unitOfWork.Save();
            return result;
        }
        #endregion
    }


}

