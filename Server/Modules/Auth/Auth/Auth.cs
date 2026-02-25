using Auth.Authentication.Jwt;
using Auth.Authentication.Model;
using Auth.Data;
using Auth.Data.Repository;
using Auth.Data.Seeder;
using Auth.Data.UnitOfWork;
using Auth.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Data.Extensions;

namespace Auth;

public static class AuthModule
{
    public static IServiceCollection AddAuthModule(this IServiceCollection service, IConfiguration configuration)
    {
        service.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        service.AddScoped<IPasswordHasher<UserName>, PasswordHasher<UserName>>();
        service.AddScoped<IAuthRepository, AuthRepository>();
        service.AddScoped<IAuthUnitOfWork, AuthUnitOfWork>();
        service.AddScoped<IAuthService, AuthService>();
        service.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        service.AddDbContext<AuthDbContext>((sp, options) =>
        {
            var saveChangesInterceptors = sp.GetServices<ISaveChangesInterceptor>();
            options.AddInterceptors(saveChangesInterceptors);
            options.UseSqlServer(configuration.GetConnectionString("Database"), sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(AuthDbContext).Assembly.GetName().Name);
                sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "auth");
            });
        });

        return service;
    }

    public static IApplicationBuilder UseAuthModule(this IApplicationBuilder app)
    {
        app.UseMigration<AuthDbContext>();
        AuthDataSeeder.SeedAsync(app.ApplicationServices).GetAwaiter().GetResult();

        return app;
    }
}
