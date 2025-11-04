
namespace OrderManagement.Application.Interfaces
{
    public interface IUnitOfWork
    {
        IOrderRepository Orders { get; }
        IProductRepository Products { get; }
        Task<int> CommitAsync();
        Task RollbackAsync();
    }
}
