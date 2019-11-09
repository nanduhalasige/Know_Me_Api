using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Know_Me_Api.Models;
using Microsoft.AspNetCore.Authorization;

namespace Know_Me_Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly DBContext _context;

        public ProductsController(DBContext context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet]
        public IEnumerable<Products> GetProducts()
        {
            return _context.Products;
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProducts([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var products = await _context.Products.FindAsync(id);

            if (products == null)
            {
                return NotFound();
            }

            return Ok(products);
        }

        // PUT: api/Products/5
        //[Route("UpdateProduct/{}")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProducts([FromRoute] Guid id, [FromBody] Products products)
        {
            //var idConverted = Guid.Parse(Request.QueryString.Value.Split('=')[1]);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            products.productId = id;

            //var prods = await _context.Products.FindAsync(id);
            //prods.quantity = products.quantity;
            //prods.manufacturerName = products.manufacturerName;
            //prods.modelName = products.modelName;
            //prods.price = products.price;
            _context.Entry(products).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return CreatedAtAction("GetProducts", GetProducts());
        }

        // POST: api/Products
        [HttpPost]
        [Route("AddNewProduct")]
        public async Task<IActionResult> AddNewProduct([FromBody] Products products)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Products.Add(products);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProducts", GetProducts());
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducts([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var products = await _context.Products.FindAsync(id);
            if (products == null)
            {
                return NotFound();
            }

            _context.Products.Remove(products);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProducts", GetProducts());
        }

        private bool ProductsExists(Guid id)
        {
            return _context.Products.Any(e => e.productId == id);
        }
    }
}