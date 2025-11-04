using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.DTOs;
using OrderManagement.Application.Services;

namespace OrderManagement.API.Controllers
{
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _orderService;
        private readonly IValidator<CreateOrderDto> _validator;

        public OrdersController(OrderService orderService, IValidator<CreateOrderDto> validator)
        {
            _orderService = orderService;
            _validator = validator;
        }

        /// <summary>
        /// Crear un nuevo pedido
        /// </summary>
        /// <param name="dto">Datos del pedido</param>
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            var validationResult = await _validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));

            try
            {
                var orderId = await _orderService.CreateOrderAsync(dto);

                return CreatedAtAction(nameof(GetOrderById), new { id = orderId }, new { orderId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtener un pedido por Id
        /// </summary>
        /// <param name="id">Id del pedido</param>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            return Ok(new { message = $"GET /api/orders/{id}" });
        }
    }
}
