using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NG_Core_Auth.Data;
using NG_Core_Auth.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NG_Core_Auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public ProductController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: api/<ProductController>
        [HttpGet("[action]")]
        [Authorize(Policy = "RequireLoggedIn")]
        public IActionResult GetProducts()
        {
            return Ok(_db.Products.ToList());
        }

        [HttpPost("[action]")]
        [Authorize(Policy = "RequireAdminstratorRole")]
        public async Task<IActionResult> AddProduct([FromBody] ProductModel formdata)
        {
            var newproduct = new ProductModel
            {
                Name = formdata.Name,
                ImageUrl = formdata.ImageUrl,
                Description = formdata.Description,
                OutOfStock = formdata.OutOfStock,
                Price = formdata.Price,
            };

            await _db.Products.AddAsync(newproduct);

            await _db.SaveChangesAsync();
            return Ok();
        }

        //Updating the Product Method
        [HttpPut("[action]/{id}")]
        [Authorize(Policy = "RequireAdminstratorRole")]
        public async Task<IActionResult> UpdateProduct([FromRoute] int id, [FromBody] ProductModel formdata)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var findProduct = _db.Products.FirstOrDefault(p => p.ProductId == id);

            if(findProduct == null)
            {
                return NotFound();
            }

            // if the product was found
            findProduct.Name = formdata.Name;
            findProduct.Description = formdata.Description;
            findProduct.ImageUrl = formdata.ImageUrl;
            findProduct.OutOfStock = formdata.OutOfStock;
            findProduct.Price = formdata.Price;

            _db.Entry(findProduct).State = EntityState.Modified;

            await _db.SaveChangesAsync();

            return Ok(new JsonResult("The Product with id " + id + " is updated"));

        }

        [HttpDelete("[action]/{id}")]
        [Authorize(Policy = "RequireAdminstratorRole")]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // find the product
            var findProduct = await _db.Products.FindAsync(id);

            if(findProduct == null)
            {
                return NotFound();
            }

            _db.Products.Remove(findProduct);

            await _db.SaveChangesAsync();

            // Finally return the result to client
            return Ok(new JsonResult("The product with id " + id + " is Deleted"));
        }
    }
}
