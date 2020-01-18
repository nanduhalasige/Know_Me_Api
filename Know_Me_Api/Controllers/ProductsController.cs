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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            products.productId = id;
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


        [Route("GetWareHouseProducts")]
        [HttpGet]
        public WareHouseData GetWareHouseProducts()
        {
            WareHouseData wareHouseData = new WareHouseData()
            {
                wareHouse = _context.WareHouse.ToList(),
                wareHouseProducts = _context.WareHouseProducts.ToList()
            };
            return wareHouseData;
        }


        [Route("UpdateStock/{id}")]
        [HttpPut]
        public async Task<IActionResult> UpdateStock([FromRoute] Guid id, [FromBody] WareHouseStockUpdate wareHouseStock)
        {
            var products = await _context.Products.FindAsync(id);
            var warehouseProds = new WareHouseProducts();

            if (wareHouseStock.wareHouseId != 0 && wareHouseStock.wareHouseId != null)
            {
                warehouseProds = _context.WareHouseProducts.Where(x => x.productId.Equals(id.ToString()) && x.wareHouseId.Equals(wareHouseStock.wareHouseId)).FirstOrDefault();
                if (wareHouseStock.change == "+")
                {
                    warehouseProds.quantity += wareHouseStock.quantity;
                }
                else
                {
                    warehouseProds.quantity -= wareHouseStock.quantity;
                }
                _context.Entry(warehouseProds).State = EntityState.Modified;
                var wareHouseAllStock = _context.WareHouseProducts.Where(x => x.productId.Equals(id.ToString())).ToList();
                products.quantity = wareHouseAllStock.Sum(s => s.quantity);
                _context.Entry(products).State = EntityState.Modified;
            }
            else
            {
                if (wareHouseStock.change == "+")
                {
                    products.quantity += wareHouseStock.quantity;
                }
                else
                {
                    products.quantity -= wareHouseStock.quantity;
                }
                products.modifiedOn = DateTime.Now;
                products.modifiedBy = wareHouseStock.modifiedBy;
                products.userId = wareHouseStock.userId;

                _context.Entry(products).State = EntityState.Modified;
            }
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
            //CreateFieldsOnWareHouseProducts(products.productId);
            var warehouses = _context.WareHouse.ToList();
            foreach (var item in warehouses)
            {
                var tempWhProds = new WareHouseProducts()
                {
                    productId = products.productId.ToString(),
                    quantity = 0,
                    wareHouseId = item.WareHouseId
                };
                _context.WareHouseProducts.Add(tempWhProds);
            }
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

    public class WareHouseStockUpdate : StockUpdate
    {
        public int? wareHouseId { get; set; }
    }

    public class WareHouseProductSelect
    {
        public int wareHouseId { get; set; }
        public string productId { get; set; }
    }

    public class WareHouseData
    {
        public List<WareHouse> wareHouse { get; set; }
        public List<WareHouseProducts> wareHouseProducts { get; set; }
    }

}