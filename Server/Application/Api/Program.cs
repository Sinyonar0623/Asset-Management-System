using Asset;
using Auth;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Parameter;
using Shared.Data;
using Shared.Data.Interceptors;
using Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);

var assetAssembly = typeof(AssetModule).Assembly;
var authAssembly = typeof(AuthModule).Assembly;
var parameterAssembly = typeof(ParameterModule).Assembly;

builder.Services.AddCarterWithAssemblies(assetAssembly, authAssembly, parameterAssembly);
builder.Services.AddMediatRWithAssemblies(assetAssembly, authAssembly, parameterAssembly);
builder.Services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventInterceptors>();
builder.Services.AddScoped<ISaveChangesInterceptor, AuditEntityInterceptors>();

builder.Services.AddScoped<ISqlConnectionFactory>(provider =>
    new SqlConnectionFactory(builder.Configuration.GetConnectionString("Database")!)
);

builder.Services.AddAssetModule(builder.Configuration);
builder.Services.AddAuthModule(builder.Configuration);
builder.Services.AddParameterModule(builder.Configuration);

var app = builder.Build();


app.Run();
