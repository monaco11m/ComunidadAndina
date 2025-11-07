
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OrderManagement.API.Middlewares;
using OrderManagement.Application.Factories;
using OrderManagement.Application.Interfaces;
using OrderManagement.Application.Services;
using OrderManagement.Application.Validators;
using OrderManagement.Infrastructure.Messaging;
using OrderManagement.Infrastructure.Persistence;
using OrderManagement.Infrastructure.Persistence.Repositories;
using OrderManagement.Infrastructure.Services;
using Serilog;

namespace OrderManagement.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog((context, config) =>
            config.ReadFrom.Configuration(context.Configuration)
            .WriteTo.Console());

            builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

            //DIP

            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IOrderFactory, OrderFactory>();
            builder.Services.AddScoped<IMessagePublisher, RabbitMqPublisher>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IProductService,ProductService>();
            builder.Services.AddValidatorsFromAssemblyContaining<CreateOrderValidator>();
            builder.Services.AddSingleton<EmailService>();
            builder.Services.AddHostedService<RabbitMqConsumerService>();


            // Add services to the container.

            builder.Services.AddControllers();


            builder.Services.Configure<RabbitMqSettings>(
            builder.Configuration.GetSection("RabbitMQ"));

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(opt =>
            {
                opt.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });



            var app = builder.Build();

            app.UseErrorHandlingMiddleware();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("AllowAll");

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();


            //seed data
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<AppDbContext>();

                await context.Database.MigrateAsync();
                await SeedData.InitializeAsync(context);
            }

            app.Run();
        }
    }
}
