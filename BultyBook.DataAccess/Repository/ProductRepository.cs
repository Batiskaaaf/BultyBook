using BultyBook.DataAccess.Data;
using BultyBook.DataAccess.Repository.IRepository;
using BultyBook.Models;


namespace BultyBook.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext context;

        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }

        public void Update(Product product)
        {
            var objFromContext = context.Products.FirstOrDefault(x => x.Id == product.Id);

            if (objFromContext == null)
                return;

            objFromContext.Name = product.Name;
            objFromContext.ISBN = product.ISBN;
            objFromContext.Price = product.Price;
            objFromContext.Price100 = product.Price100;
            objFromContext.Price50 = product.Price50;
            objFromContext.ListPrice = product.ListPrice;
            objFromContext.Description = product.Description;
            objFromContext.CategoryId = product.CategoryId;
            objFromContext.Author = product.Author;
            objFromContext.CoverTypeId = product.CoverTypeId;

            if(product.ImageUrl != null)
                objFromContext.ImageUrl = product.ImageUrl;

        }
    }
}
