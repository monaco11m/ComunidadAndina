

using FluentValidation.TestHelper;
using OrderManagement.Application.DTOs;
using OrderManagement.Application.Validators;
using Xunit;

namespace OrderManagement.Tests.Validators
{
    public class CreateOrderValidatorTests
    {
        private readonly CreateOrderValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_Name_Is_Empty()
        {
            var model = new CreateOrderDto { CustomerName = "" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.CustomerName);
        }
    }
}
