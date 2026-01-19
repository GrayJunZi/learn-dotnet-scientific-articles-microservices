using Auth.API;
using Auth.Persistence;
using FastEndpoints;
using FastEndpoints.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApiServices(builder.Configuration)
    .AddPersistenceServices(builder.Configuration);

var app = builder.Build();

app.UseSwagger()
    .UseSwaggerUI()
    .UseSwaggerGen()
    .UseHttpsRedirection()
    .UseRouting()
    .UseFastEndpoints();

app.Run();