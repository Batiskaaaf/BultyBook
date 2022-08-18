namespace BultyBook.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        public ICoverTypeRepository CoverType { get; }
        public ICategoryRepository Category { get; }
        public IProductRepository Product { get; }
        public ICompanyRepository Company { get; }
        public IShoppingCartRepository ShoppingCart { get; }
        public IApplicationUserRepository ApplicationUser { get; }
        public IOrderDetailRepository OrderDetail { get; }
        public IOrderHeaderRepository OrderHeader { get; }

        void Save();
    }
}
