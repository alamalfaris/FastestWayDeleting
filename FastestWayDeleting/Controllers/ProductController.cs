using FastestWayDeleting.Database;
using FastestWayDeleting.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastestWayDeleting.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public ProductController(DatabaseContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> GetProducts()
        {
            var products = await _context.Product.ToListAsync();
            return Ok(products);
        }

        /// <summary>
        /// Method DeleteWithConventionalApproach cons:
        /// request 2x to database
        /// pros:
        /// already handle when product not found or deleted
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        //[HttpDelete("{productId}")]
        //public async Task<ActionResult> DeleteWithConventionalApproach(int productId)
        //{
        //    // find product by id
        //    var product = await _context.Product.FindAsync(productId);

        //    if (product is null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Product.Remove(product);
        //    await _context.SaveChangesAsync();
        //    return NoContent();
        //}

        /*
         * Method DeleteWithChangeStateApproach cons:
         * raised DbUpdateConcurrencyException if get productId already deleted
         * must handle DbUpdateConcurrencyException with try catch
         * pros:
         * only once request to database
         */
        //[HttpDelete("{productId}")]
        //public async Task<ActionResult> DeleteWithChangeStateApproach(int productId)
        //{
        //    var product = new Product() { Id = productId };

        //    var productEntity = _context.Product.Attach(product);
        //    productEntity.State = EntityState.Deleted; 
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        /*
         * Method DeleteWithExecuteDeleteApproach
         * more efficient than 2 methods above
         * but ExecuteDeleteAsync only available on EFCore 7 and above
         */
        [HttpDelete("{productId}")]
        public async Task<ActionResult> DeleteWithExecuteDeleteApproach(int productId)
        {
            await _context.Product
                .Where(m => m.Id == productId)
                .ExecuteDeleteAsync();

            return NoContent();
        }
    }
}
