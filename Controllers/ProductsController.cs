using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShelfAPI.Data;
using ShelfAPI.DTOs;
using ShelfAPI.Models;

namespace ShelfAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lista todos os produtos.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> GetAll()
    {
        var products = await _context.Products
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => MapToResponse(p))
            .ToListAsync();

        return Ok(products);
    }

    /// <summary>
    /// Busca um produto por ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductResponse>> GetById(int id)
    {
        var product = await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product is null)
            return NotFound(new { message = $"Produto com ID {id} não encontrado." });

        return Ok(MapToResponse(product));
    }

    /// <summary>
    /// Cria um novo produto.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductResponse>> Create([FromBody] CreateProductRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Stock = request.Stock,
            CreatedAt = DateTime.UtcNow
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var response = MapToResponse(product);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, response);
    }

    /// <summary>
    /// Atualiza um produto existente.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductResponse>> Update(int id, [FromBody] UpdateProductRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var product = await _context.Products.FindAsync(id);

        if (product is null)
            return NotFound(new { message = $"Produto com ID {id} não encontrado." });

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.Stock = request.Stock;
        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(MapToResponse(product));
    }

    /// <summary>
    /// Deleta um produto.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product is null)
            return NotFound(new { message = $"Produto com ID {id} não encontrado." });

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // ── Helper ──

    private static ProductResponse MapToResponse(Product product) => new()
    {
        Id = product.Id,
        Name = product.Name,
        Description = product.Description,
        Price = product.Price,
        Stock = product.Stock,
        CreatedAt = product.CreatedAt,
        UpdatedAt = product.UpdatedAt
    };
}
