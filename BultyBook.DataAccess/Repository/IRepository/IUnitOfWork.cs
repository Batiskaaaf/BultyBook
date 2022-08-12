using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BultyBook.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        public ICoverTypeRepository CoverType { get; }
        public ICategoryRepository Category { get; }
        public IProductRepository Product { get; }

        void Save();
    }
}
