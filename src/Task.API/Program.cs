using Task.Application.Services;
using Task.Domain.Interfaces;
using Task.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IProductRepository, InMemoryProductRepository>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/api/products", async (ProductService svc) => await svc.GetAllAsync());
app.MapGet("/api/products/{id}", async (int id, ProductService svc) =>
    await svc.GetByIdAsync(id) is { } p ? Results.Ok(p) : Results.NotFound());
app.MapPost("/api/products", async (Task.Domain.Entities.Product product, ProductService svc) =>
    Results.Created($"/api/products/{product.Id}", await svc.CreateAsync(product)));
app.MapDelete("/api/products/{id}", async (int id, ProductService svc) =>
    await svc.DeleteAsync(id) ? Results.NoContent() : Results.NotFound());

app.MapGet("/", () => "Hello Warrior! Visit /swagger for API docs.");
app.Run();