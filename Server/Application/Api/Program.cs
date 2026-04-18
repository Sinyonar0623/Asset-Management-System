using System.Text;
using Asset;
using Auth;
using Auth.Authentication.Jwt;
using Carter;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Parameter;
using Request;
using Shared.Data.Audit;
using Shared.Data;
using Shared.Data.Interceptors;
using Shared.Extensions;
using Shared.Messaging.Integration.Extensions;

var builder = WebApplication.CreateBuilder(args);

var assetAssembly = typeof(AssetModule).Assembly;
var authAssembly = typeof(AuthModule).Assembly;
var parameterAssembly = typeof(ParameterModule).Assembly;
var RequestAssembly = typeof(RequestModule).Assembly;

builder.Services.AddCarterWithAssemblies(assetAssembly, authAssembly, parameterAssembly, RequestAssembly);
builder.Services.AddMediatRWithAssemblies(assetAssembly, authAssembly, parameterAssembly, RequestAssembly);
builder.Services.AddMassTransitWithAssemblies(builder.Configuration, assetAssembly, authAssembly, parameterAssembly, RequestAssembly);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentActorProvider, HttpContextCurrentActorProvider>();
builder.Services.AddScoped<ISaveChangesInterceptor, AuditEntityInterceptors>();
builder.Services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventInterceptors>();

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
var databaseConnectionString = builder.Configuration.GetConnectionString("Database");

if (string.IsNullOrWhiteSpace(databaseConnectionString))
{
    throw new InvalidOperationException("Missing ConnectionStrings:Database. Configure it via environment variables or user-secrets.");
}

if (string.IsNullOrWhiteSpace(jwtKey)
    || jwtKey.Length < 32
    || string.Equals(jwtKey, "your-very-long-secret-key-at-least-32-chars", StringComparison.Ordinal))
{
    throw new InvalidOperationException("Invalid Jwt:Key. Configure a strong key (>= 32 chars) via environment variables or user-secrets.");
}

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
builder.Services.AddPermissionAuthorization();

builder.Services.AddScoped<ISqlConnectionFactory>(provider =>
    new SqlConnectionFactory(databaseConnectionString)
);

builder.Services.AddAssetModule(builder.Configuration);
builder.Services.AddAuthModule(builder.Configuration);
builder.Services.AddParameterModule(builder.Configuration);
builder.Services.AddRequestModule(builder.Configuration);

var app = builder.Build();

// app.Use(async (context, next) =>
// {
//     try
//     {
//         await next();
//     }
//     catch (KeyNotFoundException)
//     {
//         if (context.Response.HasStarted)
//         {
//             throw;
//         }

//         context.Response.Clear();
//         context.Response.StatusCode = StatusCodes.Status404NotFound;
//     }
// });

app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();

app.UseRequestModule()
    .UseAuthModule()
    .UseAssetModule()
    .UseParameterModule();

app.MapCarter();

app.Run();
