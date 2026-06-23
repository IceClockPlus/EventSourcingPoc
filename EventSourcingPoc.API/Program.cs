using EventSourcingPoc.API.EFContext;
using EventSourcingPoc.API.Handlers;
using EventSourcingPoc.API.Projections;
using Marten;
using Marten.Events.Aggregation;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddScoped<CreateGuaranteeHandler>();
builder.Services.AddScoped<ConfirmGuaranteePriceHandler>();

builder.Services.AddSingleton(TimeProvider.System);

// Add Marten 
builder.Services.AddMarten(options =>
{
    options.Connection(builder.Configuration.GetConnectionString("Marten") ?? throw new ArgumentNullException());
    options.Projections.Add<GuaranteeClientProjection>(JasperFx.Events.Projections.ProjectionLifecycle.Async);
}).AddAsyncDaemon(JasperFx.Events.Daemon.DaemonMode.HotCold);

builder.Services.AddDbContext<GuaranteeContext>(opt =>
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Marten"), builder =>
    {
        builder.MigrationsAssembly(typeof(GuaranteeContext).Assembly.FullName);
    });
});


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
