using Task.Domain.Entities;
using Task.Domain.Interfaces;

namespace Task.Application.Services;

public class ProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<Product>> GetAllAsync() => _repository.GetAllAsync();
    public Task<Product?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);
    public Task<Product> CreateAsync(Product product) => _repository.CreateAsync(product);
    public Task<bool> DeleteAsync(int id) => _repository.DeleteAsync(id);
}