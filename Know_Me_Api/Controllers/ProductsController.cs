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
            return _context.Products.Where(x => x.IsActive == true).OrderByDescending(x => x.quantity);
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
            products.modifiedOn = Convert.ToDateTime(products.modifiedOn);
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

        [Route("UpdateStock/{id}")]
        [HttpPut]
        public async Task<IActionResult> UpdateStock([FromRoute] Guid id, [FromBody] StockUpdate stock)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var products = await _context.Products.FindAsync(id);

            if (stock.change == "+")
            {
                products.quantity += stock.quantity;
            }
            else
            {
                products.quantity -= stock.quantity;
            }
            products.modifiedOn = DateTime.Now;
            products.modifiedBy = stock.modifiedBy;
            products.userId = stock.userId;

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
            products.modifiedOn = Convert.ToDateTime(products.modifiedOn);
            products.IsActive = true;
            _context.Products.Add(products);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProducts", GetProducts());
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducts([FromRoute] Guid id, [FromBody] deleteUesr user)
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
            products.IsActive = false;
            products.modifiedOn = DateTime.Now;
            products.modifiedBy = user.modifiedBy;
            products.userId = user.userId;

            _context.Entry(products).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProducts", GetProducts());
        }

        [HttpPost]
        [Route("SyncAllData")]
        public async Task<IActionResult> SyncAllData([FromBody]SyncData syncData)
        {
            var device = syncData.device;
            var userId = syncData.userId;
            var prods = syncData.products;
            var syncProd = syncData.productsOnSync;
            if (!prods.SequenceEqual(syncProd))
            {
                var products = GetProducts().ToList();
                var intersect = prods.Intersect(syncProd);
                var difProd = prods.Except(intersect);
                if (difProd.Any())
                {
                    foreach (var item in difProd)
                    {
                        var tempProd = products.Find(x => x.productId.Equals(item.productId));
                        if (tempProd == null)
                        {
                            _context.Add(tempProd);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            if (tempProd.modifiedOn < item.modifiedOn || tempProd.quantity != item.quantity || tempProd.modifiedBy != item.modifiedBy)
                            {
                                var tempQt = syncProd.Find(x => x.productId.Equals(item.productId)).quantity;
                                item.quantity = tempProd.quantity == tempQt
                                    ? item.quantity
                                    : tempProd.quantity - (tempQt - item.quantity);
                                item.quantity = item.quantity < 0 ? 0 : item.quantity;
                                try
                                {
                                    tempProd.IsActive = item.IsActive;
                                    tempProd.quantity = item.quantity;
                                    tempProd.modifiedBy = item.modifiedBy;
                                    tempProd.modifiedOn = item.modifiedOn;
                                    tempProd.userId = item.userId;
                                    _context.Entry(tempProd).State = EntityState.Modified;
                                    await _context.SaveChangesAsync();
                                }
                                catch (Exception ex)
                                {
                                    throw ex;
                                }
                            }
                        }
                    }
                }

            }
            return CreatedAtAction("GetProducts", GetProducts());
        }

        private bool ProductsExists(Guid id)
        {
            return _context.Products.Any(e => e.productId == id);
        }
    }

    public class StockUpdate
    {
        public int quantity { get; set; }
        public string change { get; set; }
        public string userId { get; set; }
        public string modifiedBy { get; set; }
    }

    public class deleteUesr
    {
        public string userId { get; set; }
        public string modifiedBy { get; set; }
    }

    public class SyncData
    {
        public List<Products> products { get; set; }
        public List<Products> productsOnSync { get; set; }
        public string userId { get; set; }
        public string device { get; set; }
    }
}