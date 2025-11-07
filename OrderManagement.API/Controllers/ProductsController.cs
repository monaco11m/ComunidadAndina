using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.Interfaces;
using OrderManagement.Application.Services;

namespace OrderManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;
        public ProductsController(IProductService productService)
        {
            _service = productService;
        }

        /// <summary>
        /// Listas todas los productos
        /// </summary>
        [HttpGet()]
        public async Task<IActionResult> GetAllProducts()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }
    }
}
