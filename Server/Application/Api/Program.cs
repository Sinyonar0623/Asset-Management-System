using Auth;
using Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);

var authAssembly = typeof(AuthModule).Assembly;

builder.Services.AddCarterWithAssemblies(authAssembly);
builder.Services.AddMediatRWithAssemblies(authAssembly);

// builder.Services.AddAuthModule();

var app = builder.Build();

app.UseAuthModule();


app.Run();
