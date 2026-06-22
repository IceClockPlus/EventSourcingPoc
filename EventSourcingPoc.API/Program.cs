using EventSourcingPoc.API.Handlers;
using Marten;
using Marten.Events.Aggregation;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddScoped<CreateGuaranteeHandler>();
builder.Services.AddScoped<ConfirmGuaranteePriceHandler>();

// Add Marten 
builder.Services.AddMarten(options =>
{
    options.Connection(builder.Configuration.GetConnectionString("Marten") ?? throw new ArgumentNullException());
    options.Projections.Add<ProductProjection>(JasperFx.Events.Projections.ProjectionLifecycle.Async);
}).AddAsyncDaemon(JasperFx.Events.Daemon.DaemonMode.HotCold);

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


public record ProductCreated(Guid Id, string Name, decimal Price, int Quantity, string? Description = null);
public record ProductAdded(int Quantity);
public record ProductDeleted(int Quantity);


public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}
public partial class ProductProjection : SingleStreamProjection<Product, Guid>
{
    public static Product Create(ProductCreated @event)
    {
        return new Product
        {
            Id = @event.Id,
            Name = @event.Name,
            Description = @event.Description,
            Price = @event.Price,
            Quantity = @event.Quantity
        };
    }

    public void Apply(ProductCreated productCreated, Product product)
    {
        product.Id = productCreated.Id;
        product.Name = productCreated.Name;
        product.Description = productCreated.Description;
        product.Price = productCreated.Price;
        product.Quantity = productCreated.Quantity;
    }

    public void Apply(ProductDeleted productDeleted, Product product)
    {
        product.Quantity -= productDeleted.Quantity;
    }

    public void Apply(ProductAdded productAdded, Product product)
    {
        product.Quantity += productAdded.Quantity;
    }
}