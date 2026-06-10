using Sentinel.Worker;
using Sentinel.Worker.Configuration;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<WorkerSettings>(
    builder.Configuration.GetSection("WorkerSettings"));

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
