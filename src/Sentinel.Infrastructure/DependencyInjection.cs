using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sentinel.Application.Interfaces;
using Sentinel.Infrastructure.Persistence;
using Sentinel.Infrastructure.Services;

namespace Sentinel.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // EF Core + PostgreSQL with JSONB support
        var connectionString = configuration.GetConnectionString("SentinelDb");
        var dataSourceBuilder = new Npgsql.NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();

        services.AddDbContext<SentinelDbContext>(options =>
            options.UseNpgsql(
                dataSource,
                b => b.MigrationsAssembly(typeof(SentinelDbContext).Assembly.FullName)));

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<SentinelDbContext>());

        // External services
        services.AddHttpClient<IAiAnalysisService, N8nWebhookService>();
        services.AddScoped<IAlertNotificationService, SignalRNotificationService>();

        // Background services
        services.AddHostedService<AgentMonitorService>();

        // SignalR
        services.AddSignalR();

        return services;
    }
}
