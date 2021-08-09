using ElectronicsShop.BAL.Repositories;
using ElectronicsShop.DAL.Context;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using ElectronicsShop.DAL.Entities;

namespace ElectronicsShop.BAL.UOW
{
   public class UnitOfWork : IUnitOfWork
    {
        private readonly ElectronicsShopContext Context;


        public UnitOfWork(ElectronicsShopContext dbContext)
        {
            Context = dbContext;
        }

        public GenericRepository<T> Repository<T>() where T : class, new()
        {
            return new GenericRepository<T>(Context);
        }

        #region ProductRepository

        private GenericRepository<Product> productRepository;
        public GenericRepository<Product> ProductRepository
        {
            get
            {

                if (this.productRepository == null)
                {
                    this.productRepository = new GenericRepository<Product>(Context);
                }
                return productRepository;
            }
        }
        #endregion
        #region CategoryRepository

        private GenericRepository<Category> categoryRepository;
        public GenericRepository<Category> CategoryRepository
        {
            get
            {

                if (this.categoryRepository == null)
                {
                    this.categoryRepository = new GenericRepository<Category>(Context);
                }
                return categoryRepository;
            }
        }
        #endregion
        #region OrderRepository

        private GenericRepository<Order> orderRepository;
        public GenericRepository<Order> OrderRepository
        {
            get
            {

                if (this.orderRepository == null)
                {
                    this.orderRepository = new GenericRepository<Order>(Context);
                }
                return orderRepository;
            }
        }
        #endregion
        public virtual int Save()
        {
            int returnValue = 200;
            using (var dbContextTransaction = Context.Database.BeginTransaction())
                //  {
                try
                {
                    Context.SaveChanges();
                    dbContextTransaction.Commit();
                }
                catch (DbUpdateException ex)
                {
                    var sqlException = ex.GetBaseException() as SqlException;

                    if (sqlException != null)
                    {
                        var number = sqlException.Number;

                        if (number == 547)
                        {
                            returnValue = 501;

                        }
                        else
                            returnValue = 500;
                    }
                }
                catch (Exception ex)
                {
                    //Log Exception Handling message                      
                    returnValue = 500;
                    dbContextTransaction.Rollback();
                }
            //    }

            return returnValue;
        }

        public virtual async Task<int> SaveAsync()
        {
            int returnValue = 200;
            using (var dbContextTransaction = Context.Database.BeginTransaction())
            {
                try
                {
                    await Context.SaveChangesAsync();
                    dbContextTransaction.Commit();
                }
                catch (DbUpdateException ex)
                {
                    var sqlException = ex.GetBaseException() as SqlException;

                    if (sqlException != null)
                    {
                        var number = sqlException.Number;

                        if (number == 547)
                        {
                            returnValue = 501;

                        }
                        else
                            returnValue = 500;
                    }
                }
                catch (Exception)
                {
                    //Log Exception Handling message                      
                    returnValue = 500;
                    dbContextTransaction.Rollback();
                }
            }

            return returnValue;
        }

        private bool disposed = false;



        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    Context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

}
