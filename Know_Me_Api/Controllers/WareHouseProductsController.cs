using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Know_Me_Api.Models;

namespace Know_Me_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WareHouseProductsController : ControllerBase
    {
        private readonly DBContext _context;

        public WareHouseProductsController(DBContext context)
        {
            _context = context;
        }

        // GET: api/WareHouseProducts
        [HttpGet]
        public IEnumerable<WareHouseProducts> GetWareHouseProducts()
        {
            return _context.WareHouseProducts;
        }

        // GET: api/WareHouseProducts/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetWareHouseProducts([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var wareHouseProducts = await _context.WareHouseProducts.FindAsync(id);

            if (wareHouseProducts == null)
            {
                return NotFound();
            }

            return Ok(wareHouseProducts);
        }

        // PUT: api/WareHouseProducts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWareHouseProducts([FromRoute] int id, [FromBody] WareHouseProducts wareHouseProducts)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != wareHouseProducts.Id)
            {
                return BadRequest();
            }

            _context.Entry(wareHouseProducts).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WareHouseProductsExists(id))
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

        // POST: api/WareHouseProducts
        [HttpPost]
        public async Task<IActionResult> PostWareHouseProducts([FromBody] WareHouseProducts wareHouseProducts)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.WareHouseProducts.Add(wareHouseProducts);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWareHouseProducts", new { id = wareHouseProducts.Id }, wareHouseProducts);
        }

        // DELETE: api/WareHouseProducts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWareHouseProducts([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var wareHouseProducts = await _context.WareHouseProducts.FindAsync(id);
            if (wareHouseProducts == null)
            {
                return NotFound();
            }

            _context.WareHouseProducts.Remove(wareHouseProducts);
            await _context.SaveChangesAsync();

            return Ok(wareHouseProducts);
        }

        private bool WareHouseProductsExists(int id)
        {
            return _context.WareHouseProducts.Any(e => e.Id == id);
        }
    }
}