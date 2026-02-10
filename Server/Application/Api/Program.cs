using Asset;
using Shared.Data;
using Shared.Extensions;
using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);

var assetAssembly = typeof(AssetModule).Assembly;

builder.Services.AddCarterWithAssemblies(assetAssembly);
builder.Services.AddMediatRWithAssemblies(assetAssembly);

builder.Services.AddScoped<ISqlConnectionFactory>(provider =>
    new SqlConnectionFactory(builder.Configuration.GetConnectionString("Database")!)
);

builder.Services.AddAssetModule(builder.Configuration);

var app = builder.Build();


app.Run();
