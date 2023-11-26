using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using Dotnet.Homeworks.Infrastructure.Cqrs.Queries;
using Dotnet.Homeworks.Shared.Dto;

namespace Dotnet.Homeworks.Features.Products.Queries.GetProducts;

internal sealed class GetProductsQueryHandler : IQueryHandler<GetProductsQuery, GetProductsDto>
{
    private readonly IProductRepository _productRepository;

    public GetProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<GetProductsDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var products = await _productRepository.GetAllProductsAsync(cancellationToken);
            
            var productDtos = products
                .Select(product => new GetProductDto(product.Id, product.Name)).ToList();

            var result = new GetProductsDto(productDtos);

            return new Result<GetProductsDto>(result, true);
        }
        catch (Exception ex)
        {
            return new Result<GetProductsDto>(null, true, ex.Message);
        }
    }
}