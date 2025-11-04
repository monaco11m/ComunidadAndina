using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.DTOs;
using OrderManagement.Application.Exceptions;
using OrderManagement.Application.Services;

namespace OrderManagement.API.Controllers
{
    [Route("api/[controller]")]
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
                throw new DomainValidationException(validationResult.Errors.Select(e => e.ErrorMessage));

            var orderId = await _orderService.CreateOrderAsync(dto);
            return CreatedAtAction(nameof(GetOrderById), new { id = orderId }, new { orderId });
        }

        /// <summary>
        /// Obtener un pedido por Id
        /// </summary>
        /// <param name="id">Id del pedido</param>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var result = await _orderService.GetByIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Listas todas las ordenes
        /// </summary>
        [HttpGet()]
        public async Task<IActionResult> GetAllOrders()
        {
            var result = await _orderService.GetAllOrdersAsync();
            return Ok(result);
        }

        /// <summary>
        /// Cancelar pedido por Id
        /// </summary>
        /// <param name="id">Id del pedido</param>
        [HttpPut("{id:guid}/cancel")]
        public async Task<IActionResult> CancelOrder(Guid id)
        {
            var updatedOrder = await _orderService.CancelOrderAsync(id);
            return Ok(updatedOrder);
        }



    }
}
