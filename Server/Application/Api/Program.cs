using Asset;
using Auth;
using Carter;
using Parameter;
using Shared.Data;
using Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);

var assetAssembly = typeof(AssetModule).Assembly;
var authAssembly = typeof(AuthModule).Assembly;
var parameterAssembly = typeof(ParameterModule).Assembly;

builder.Services.AddCarterWithAssemblies(assetAssembly, authAssembly, parameterAssembly);
builder.Services.AddMediatRWithAssemblies(assetAssembly, authAssembly, parameterAssembly);

builder.Services.AddScoped<ISqlConnectionFactory>(provider =>
    new SqlConnectionFactory(builder.Configuration.GetConnectionString("Database")!)
);

builder.Services.AddAssetModule(builder.Configuration);
builder.Services.AddAuthModule(builder.Configuration);
builder.Services.AddParameterModule(builder.Configuration);

var app = builder.Build();

app.MapCarter();

app.Run();
