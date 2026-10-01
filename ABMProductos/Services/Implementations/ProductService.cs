using ABMProductos.Entities;
using ABMProductos.Models.DTOs.Requests;
using ABMProductos.Models.DTOs.Responses;
using ABMProductos.Repositories.Interfaces;
using ABMProductos.Services.Interfaces;

namespace ABMProductos.Services.Implementations;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public List<ProductForReadDto> GetAllProducts()
    {
        var products = _repository.GetAllProducts();
        var result = new List<ProductForReadDto>();

        foreach (var product in products)
        {
            result.Add(MapToReadDto(product));
        }

        return result;
    }

    public ProductForReadDto? GetProductById(int id)
    {
        var product = _repository.GetProductById(id);
        if (product is null) return null;

        return MapToReadDto(product);
    }

    public ProductForReadDto CreateProduct(ProductForCreateDto dto)
    {
        if (NameExists(dto.Name))
        {
            throw new InvalidOperationException($"Ya existe un producto con el nombre '{dto.Name}'.");
        }

        var product = new Product
        {
            Name = dto.Name,
            Price = dto.Price
        };

        _repository.AddProduct(product);

        return MapToReadDto(product);
    }

    public void UpdateProduct(int id, ProductForUpdateDto dto)
    {
        if (NameExists(dto.Name, id))
        {
            throw new InvalidOperationException($"Ya existe un producto con el nombre '{dto.Name}'.");
        }

        var product = new Product
        {
            Id = id,
            Name = dto.Name,
            Price = dto.Price
        };

        _repository.UpdateProduct(product);
    }

    public void DeleteProduct(int id)
    {
        var product = _repository.GetProductById(id);
        if (product is null) return;

        _repository.DeleteProduct(product);
    }

    public List<ProductForReadDto> SearchProductsByName(string name)
    {
        var products = _repository.SearchProductsByName(name);
        var result = new List<ProductForReadDto>();

        foreach (var product in products)
        {
            result.Add(MapToReadDto(product));
        }

        return result;
    }

    public ProductStatsDto GetStats()
    {
        var products = _repository.GetAllProducts();

        if (products.Count == 0)
        {
            return new ProductStatsDto
            {
                Total = 0,
                AveragePrice = 0,
                MostExpensiveName = string.Empty
            };
        }

        return new ProductStatsDto
        {
            Total = products.Count,
            AveragePrice = Math.Round(products.Average(p => p.Price), 2),
            MostExpensiveName = products.OrderByDescending(p => p.Price).First().Name
        };
    }

    private bool NameExists(string name, int? excludeId = null)
    {
        return _repository.GetAllProducts()
            .Any(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && p.Id != excludeId);
    }

    private ProductForReadDto MapToReadDto(Product product)
    {
        return new ProductForReadDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price
        };
    }
}