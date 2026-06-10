using Microsoft.EntityFrameworkCore;
using Sentinel.Application;
using Sentinel.Infrastructure;
using Sentinel.Infrastructure.GrpcServices;
using Sentinel.Infrastructure.Persistence;
using Sentinel.Infrastructure.Services;
using Sentinel.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Application & Infrastructure DI
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// gRPC
builder.Services.AddGrpc();

// CORS (React Dashboard + Mobile)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowDashboard", policy =>
    {
        policy.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Auto-migrate in Development
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<SentinelDbContext>();
    await db.Database.MigrateAsync();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors("AllowDashboard");

app.MapControllers();
app.MapGrpcService<LogIngestionGrpcService>();
app.MapHub<AlertHub>("/hubs/alerts");

app.MapGet("/health", () => Results.Ok(new { status = "Healthy", timestamp = DateTime.UtcNow }));

app.Run();
