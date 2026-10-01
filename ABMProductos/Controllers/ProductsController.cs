using ABMProductos.Models.DTOs.Requests;
using ABMProductos.Models.DTOs.Responses;
using ABMProductos.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ABMProductos.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;

    public ProductsController(IProductService service)
    {
        _service = service;
    }

    [HttpGet]
    public ActionResult<List<ProductForReadDto>> GetAll()
    {
        var products = _service.GetAllProducts();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public ActionResult<ProductForReadDto> GetById(int id)
    {
        var product = _service.GetProductById(id);
        if (product is null) return NotFound();

        return Ok(product);
    }

    [HttpPost]
    public ActionResult<ProductForReadDto> Create([FromBody] ProductForCreateDto dto)
    {
        try
        {
            var created = _service.CreateProduct(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] ProductForUpdateDto dto)
    {
        if (_service.GetProductById(id) is null) return NotFound();

        try
        {
            _service.UpdateProduct(id, dto);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        if (_service.GetProductById(id) is null) return NotFound();

        _service.DeleteProduct(id);
        return NoContent();
    }

    [HttpGet("search")]
    public ActionResult<List<ProductForReadDto>> Search([FromQuery] string name)
    {
        var products = _service.SearchProductsByName(name);
        return Ok(products);
    }

    [HttpGet("stats")]
    public ActionResult<ProductStatsDto> GetStats()
    {
        var stats = _service.GetStats();
        return Ok(stats);
    }
}