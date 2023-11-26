using Dotnet.Homeworks.Data.DatabaseContext;
using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using Dotnet.Homeworks.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dotnet.Homeworks.DataAccess.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync(CancellationToken cancellationToken)
    {
        return await _context.Products.ToListAsync(cancellationToken: cancellationToken);
    }

    public Task DeleteProductByGuidAsync(Guid id, CancellationToken cancellationToken)
    {
        _context.Products.Remove(new Product { Id = id });
        return Task.CompletedTask;
    }

    public Task UpdateProductAsync(Product product, CancellationToken cancellationToken)
    {
        _context.Products.Update(product);
        return Task.CompletedTask;
    }

    public async Task<Guid> InsertProductAsync(Product product, CancellationToken cancellationToken)
    {
        var entity = await _context.Products.AddAsync(product, cancellationToken);
        return entity.Entity.Id;
    }
}