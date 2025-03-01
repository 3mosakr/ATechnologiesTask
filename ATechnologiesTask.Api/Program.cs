using ATechnologiesTask.Api.Controllers;
using ATechnologiesTask.Api.Core.Handlers;
using ATechnologiesTask.Api.Core.HandlersAbstractions;
using ATechnologiesTask.Api.Services.Abstracts;
using ATechnologiesTask.Api.Services.Implementations;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddHttpClient<IpController>();
builder.Services.AddSingleton<IBlockedCountriesService, BlockedCountriesService>(); // Register the service
builder.Services.AddScoped<IBlockedCountriesHandler, BlockedCountriesHandler>(); // Register the service
builder.Services.AddSingleton<IIpService, IpService>();
builder.Services.AddScoped<IIpHandler, IpHandler>();
builder.Services.AddSingleton<ILogService, LogService>();
builder.Services.AddSingleton<ILogsHandler, LogsHandler>();

builder.Services.AddHostedService<ATechnologiesTask.Api.Services.BackgroundService>();


// forward headers configuration for reverse proxy
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
