using Task.Domain.Entities;
using Task.Domain.Interfaces;

namespace Task.Infrastructure.Repositories;

public class InMemoryProductRepository : IProductRepository
{
    private readonly List<Product> _products = new List<Product>
    {
        new Product { Id = 1, Name = "Sample Product", Price = 9.99m }
    };
    private int _nextId = 2;

    public System.Threading.Tasks.Task<IEnumerable<Product>> GetAllAsync() =>
        System.Threading.Tasks.Task.FromResult<IEnumerable<Product>>(_products);

    public System.Threading.Tasks.Task<Product?> GetByIdAsync(int id) =>
        System.Threading.Tasks.Task.FromResult(_products.FirstOrDefault(p => p.Id == id));

    public System.Threading.Tasks.Task<Product> CreateAsync(Product product)
    {
        product.Id = _nextId++;
        _products.Add(product);
        return System.Threading.Tasks.Task.FromResult(product);
    }

    public System.Threading.Tasks.Task<bool> DeleteAsync(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product is null) return System.Threading.Tasks.Task.FromResult(false);
        _products.Remove(product);
        return System.Threading.Tasks.Task.FromResult(true);
    }
}