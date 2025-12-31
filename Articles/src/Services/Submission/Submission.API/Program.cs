using Scalar.AspNetCore;
using Submission.API;
using Submission.API.Endpoints;
using Submission.Application;
using Submission.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApiServices(builder.Configuration)
    .AddApplicationServices(builder.Configuration)
    .AddPersistenceServices(builder.Configuration);

var app = builder.Build();

app.MapScalarApiReference();

app.MapAllEndpoints();

app.Run();