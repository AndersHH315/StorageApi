using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StorageApi.Models;
using StorageApi.DTOs;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly StorageApiContext _context;
    public ProductsController(StorageApiContext context)
    {
        _context = context;
    }

    // GET: api/Product
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProduct()
    {
        var products = from p in _context.Product
                       select new ProductDto
                       {
                           Id = p.Id,
                           Name = p.Name,
                           Price = p.Price,
                           Count = p.Count

                       };
        return await products.ToListAsync();
    }

    // GET: api/Product/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        var product = await _context.Product.Select(p => new ProductDto { 
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Count = p.Count

        }).SingleOrDefaultAsync(p => p.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpGet("stats")]
    public async Task<ActionResult<ProductDto>> GetProductStats()
    {
        var stats = from p in _context.Product
                    group p by p.Name into g
                    select new ProductDto
                    {
                        Name = g.Key,
                        Count = g.Sum(c => c.Count),
                        Price = g.Sum(c => c.Price)
                    };

        if (!stats.Any())
        {
            return NotFound();
        }

        var totalCount = stats.Sum(p => p.Count);
        var totalPrice = stats.Sum(p => p.Price * p.Count);
        var averagePrice = stats.Average(p => p.Price * p.Count);

        return Ok($"Total products: {totalCount}\nTotalvalue: {totalPrice}\nAveragevalue: {averagePrice}");
    }

    [HttpGet("category")]
    public async Task<ActionResult<List<ProductDto>>> GetProductCategory([FromQuery] string category)
    {
        var checkCategory = _context.Product.Select(p => new ProductDto
        {
            Category = p.Category
        }).Where(p => p.Category == category);

        if (checkCategory.IsNullOrEmpty())
        {
            return NotFound();
        }

        return Ok(checkCategory);
    }

    // PUT: api/Product/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutProduct(int? id, Product product)
    {
       
        if (id != product.Id)
        {
            return BadRequest();
        }

        _context.Entry(product).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ProductExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }
    // POST: api/Product
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<CreateProductDto>> PostProduct(Product product)
    {
        _context.Product.Add(product);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetProduct", new { id = product.Id }, product);
    }

    // DELETE: api/Product/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int? id)
    {
        var product = await _context.Product.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        _context.Product.Remove(product);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ProductExists(int? id)
    {
        return _context.Product.Select(e => new ProductDto { Id = e.Id}).Any(e => e.Id == id);
    }
}
