using Auth;
using Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);

var authAssembly = typeof(AuthModule).Assembly;

builder.Services.AddCarterWithAssemblies(authAssembly);
builder.Services.AddMediatRWithAssemblies(authAssembly);

var app = builder.Build();


app.Run();
