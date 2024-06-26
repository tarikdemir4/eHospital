﻿using eHospitalServer.Business.Services;
using eHospitalServer.DataAccess.Context;
using eHospitalServer.DataAccess.Options;
using eHospitalServer.DataAccess.Repositories;
using eHospitalServer.DataAccess.Services;
using eHospitalServer.Entities.Models;
using eHospitalServer.Entities.Repositories;
using GenericRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace eHospitalServer.DataAccess;
public static class DependencyInjection
{
    public static IServiceCollection AddDataAccess(
        this IServiceCollection services,
        IConfiguration configuration)
    {

        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options
                   .UseNpgsql(configuration.GetConnectionString("PostgreSQL"))
                   .UseSnakeCaseNamingConvention();
        });

        services.AddScoped<IUnitOfWork>(srv=>srv.GetRequiredService<ApplicationDbContext>());

        services
            .AddIdentity<User,IdentityRole<Guid>>(cfr =>
            {
                cfr.Password.RequiredLength = 1;
                cfr.Password.RequireNonAlphanumeric = false;
                cfr.Password.RequireUppercase = false;
                cfr.Password.RequireLowercase = false;
                cfr.Password.RequireDigit = false;
                cfr.SignIn.RequireConfirmedEmail = true;
                cfr.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                cfr.Lockout.MaxFailedAccessAttempts = 3;
                cfr.Lockout.AllowedForNewUsers = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
        services.ConfigureOptions<JwtTokenOptionsSetup>();


        services.AddAuthentication()
                .AddJwtBearer();
        services.AddAuthorizationBuilder();


       services.AddScoped<JwtProvider>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAppointmentService, AppointmentService>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();


        return services;
    }
}
