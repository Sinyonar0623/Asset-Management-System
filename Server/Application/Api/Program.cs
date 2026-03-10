using System.Text;
using Asset;
using Auth;
using Auth.Authentication.Jwt;
using Carter;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Parameter;
using Request;
using Shared.Data;
using Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);

var assetAssembly = typeof(AssetModule).Assembly;
var authAssembly = typeof(AuthModule).Assembly;
var parameterAssembly = typeof(ParameterModule).Assembly;
var RequestAssembly = typeof(RequestModule).Assembly;

builder.Services.AddCarterWithAssemblies(assetAssembly, authAssembly, parameterAssembly, RequestAssembly);
builder.Services.AddMediatRWithAssemblies(assetAssembly, authAssembly, parameterAssembly, RequestAssembly);

var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        if (allowedOrigins.Length == 0)
        {
            return;
        }

        policy.WithOrigins(allowedOrigins)
            .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS")
            .WithHeaders("Authorization", "Content-Type");
    });
});

var jwtSection = builder.Configuration.GetSection(JwtOptions.SectionName);
var jwtIssuer = jwtSection["Issuer"] ?? string.Empty;
var jwtAudience = jwtSection["Audience"] ?? string.Empty;
var jwtKey = jwtSection["Key"] ?? string.Empty;

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddScoped<ISqlConnectionFactory>(provider =>
    new SqlConnectionFactory(builder.Configuration.GetConnectionString("Database")!)
);

builder.Services.AddAssetModule(builder.Configuration);
builder.Services.AddAuthModule(builder.Configuration);
builder.Services.AddParameterModule(builder.Configuration);
builder.Services.AddRequestModule(builder.Configuration);

var app = builder.Build();

app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapCarter();

app.Run();
