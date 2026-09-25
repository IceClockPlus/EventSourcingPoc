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
    .AddClasses(c => c
            .AssignableTo(typeof(IQueryHandler<,>))
            .Where(type => !type.ContainsGenericParameters)
        , publicOnly: false)
        .AsImplementedInterfaces()
        .WithScopedLifetime()
    .AddClasses(c => c
            .AssignableTo(typeof(ICommandHandler<>))
            .Where(type => !type.ContainsGenericParameters)
        , publicOnly: false)
        .AsImplementedInterfaces()
        .WithScopedLifetime()
    .AddClasses(c => c
            .AssignableTo(typeof(ICommandHandler<,>))
            .Where(type => !type.ContainsGenericParameters)
        , publicOnly: false)
        .AsImplementedInterfaces()
        .WithScopedLifetime()
);


string eventStoreConnectionString = builder.Configuration.GetConnectionString("EventStore") ?? throw new InvalidOperationException("EventStore connection string is not configured.");

builder.Services.AddDbContext<EventStoreContext>(options =>
    options.UseSqlServer(eventStoreConnectionString, sqlOptions => 
    {
        sqlOptions.EnableRetryOnFailure();
    }));


string dbReadConnectionString = builder.Configuration.GetConnectionString("ReadModel") ?? throw new InvalidOperationException("ReadModel connection string is not configured.");

builder.Services.AddDbContext<ProjectionContext>(options =>
{
    options.UseSqlServer(dbReadConnectionString, sqlOptions => 
    {
        sqlOptions.EnableRetryOnFailure();
    });

});


// Recover AUTOMATIC_MIGRATION from environment variable, default to false if not set
var automaticMigration = builder.Configuration.GetValue<bool?>("AUTOMATIC_MIGRATION") ?? false;

// Apply migrations automatically if AUTOMATIC_MIGRATION is true
if (automaticMigration)
{
    using (var scope = builder.Services.BuildServiceProvider().CreateScope())
    {
        var eventStoreDbContext = scope.ServiceProvider.GetRequiredService<EventStoreContext>();
        eventStoreDbContext.Database.Migrate();

        var readModelDbContext = scope.ServiceProvider.GetRequiredService<ProjectionContext>();
        readModelDbContext.Database.Migrate();
    }    
}


builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddSingleton<EventTypeMap>();
builder.Services.AddScoped<EventStore>();
builder.Services.AddScoped<ICommandDispatcher, CommandDispatcher>();
builder.Services.AddScoped<IQueryDispatcher, QueryDispatcher>();

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
