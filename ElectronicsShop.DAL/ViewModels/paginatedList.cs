using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
namespace ElectronicsShop.DAL.ViewModels
{
  public  class paginatedList<T>:List<T>
    {
        public int PageIndex { get;private set;}
        public int totalPages { get; set; }
        public paginatedList(List<T>items,int count,int pageSize,int pageIndex)
        {
            PageIndex = pageIndex;
            totalPages = (int)Math.Ceiling(count / (double)pageSize);
            this.AddRange(items);
        }
        public bool previousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }
        public bool nextPage
        {
            get
            {
                return (PageIndex < totalPages);
            }
        }
        public static paginatedList<T> CreateAsync(IEnumerable<T> source,int pageIndex,int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new paginatedList<T>(items,count, pageSize, pageIndex);
        }
    }
}
