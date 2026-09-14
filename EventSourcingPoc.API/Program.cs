using EventSourcingPoc.API.EFContext;
using EventSourcingPoc.API.Handlers;
using EventSourcingPoc.API.Projections;
using EventSourcingPoc.API.Services;
using Marten;
using Marten.Events.Aggregation;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Polecat;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
        context.ProblemDetails.Extensions.Add("requestId", context.HttpContext.TraceIdentifier);
        var activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
        context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
    };
});

builder.Services.AddScoped<IBrokerService, BrokerService>();
builder.Services.AddScoped<IInsuranceService, InsuranceService>();
builder.Services.AddScoped<IBondsService, BondsService>();


// Scan all the command and query handlers in the assembly and register them as scoped services
builder.Services.Scan(scan => 
    scan.FromAssemblyOf<Program>()
    .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>)), publicOnly: false)
        .AsImplementedInterfaces()
        .WithScopedLifetime()
    .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<>)), publicOnly: false)
        .AsImplementedInterfaces()
        .WithScopedLifetime()
    .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<,>)), publicOnly: false)
        .AsImplementedInterfaces()
        .WithScopedLifetime()
);

builder.Services.AddScoped<CreateGuaranteeHandler>();
builder.Services.AddScoped<IssueGuaranteeHandler>();
builder.Services.AddScoped<ConfirmGuaranteePriceHandler>();
builder.Services.AddScoped<UpdateGuaranteeInformationHandler>();

builder.Services.AddSingleton(TimeProvider.System);

// Add Polecat
builder.Services.AddPolecat(options =>
{
    options.Connection(builder.Configuration.GetConnectionString("DbPersistence") ?? throw new ArgumentNullException());
    
});

// Add Marten 
builder.Services.AddMarten(options =>
{
    options.Connection(builder.Configuration.GetConnectionString("Marten") ?? throw new ArgumentNullException());
    options.Projections.Add<GuaranteeClientProjection>(JasperFx.Events.Projections.ProjectionLifecycle.Async);
}).AddAsyncDaemon(JasperFx.Events.Daemon.DaemonMode.HotCold);

builder.Services.AddDbContext<GuaranteeContext>(opt =>
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Marten"), 
        npgsqlOptions => npgsqlOptions.MigrationsHistoryTable("ef_migrations_history"))
    .UseSnakeCaseNamingConvention();
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
