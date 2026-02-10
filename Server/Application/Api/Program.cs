using Asset;
using Auth;
using Shared.Data;
using Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);

var assetAssembly = typeof(AssetModule).Assembly;
var authAssembly = typeof(AuthModule).Assembly;

builder.Services.AddCarterWithAssemblies(assetAssembly, authAssembly);
builder.Services.AddMediatRWithAssemblies(assetAssembly, authAssembly);

builder.Services.AddScoped<ISqlConnectionFactory>(provider =>
    new SqlConnectionFactory(builder.Configuration.GetConnectionString("Database")!)
);

builder.Services.AddAssetModule(builder.Configuration);
builder.Services.AddAuthModule(builder.Configuration);

var app = builder.Build();


app.Run();
