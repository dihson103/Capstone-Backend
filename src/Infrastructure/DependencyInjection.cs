﻿using Application.Abstractions.Services;
using Application.UserCases.Commands.MonthlyCompanySalaries.Creates;
using Application.UserCases.Commands.MonthlyEmployeeSalaries;
using Contract.Abstractions.Messages;
using Contract.Services.MonthEmployeeSalary.Creates;
using Contract.Services.MonthlyCompanySalary.Creates;
using Infrastructure.AuthOptions;
using Infrastructure.BackgtoundServiceOptions;
using Infrastructure.Options;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System.Text;
using System.Text.Json;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration
        )
    {
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<IJwtService, JwtService>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
             .AddJwtBearer(options =>
             {
                 var jwtOptions = configuration.GetSection("Jwt").Get<JwtOptions>();
                 options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                 {
                     ValidateIssuer = true,
                     ValidateAudience = true,
                     ValidateLifetime = true,
                     ValidateIssuerSigningKey = true,
                     ValidIssuer = jwtOptions.Issuer,
                     ValidAudience = jwtOptions.Audience,
                     IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                         Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
                 };

                 options.Events = new JwtBearerEvents
                 {
                     OnAuthenticationFailed = context =>
                     {
                         context.Response.StatusCode = 401;
                         context.Response.ContentType = "application/json";
                         var result = JsonSerializer.Serialize(new { message = "Authentication failed" });
                         return context.Response.WriteAsync(result);
                     }
                 };
             });
        services.AddAuthorization();

        services.AddAuthorizationBuilder()
            .AddPolicy("Require-Admin", policy => policy.RequireClaim("Role", "MAIN_ADMIN"))
            .AddPolicy("Require-User", policy => policy.RequireClaim("Role", "USER"))
            .AddPolicy("Require-Counter", policy => policy.RequireClaim("Role", "COUNTER"))
            .AddPolicy("Require-Driver", policy => policy.RequireClaim("Role", "DRIVER"))
            .AddPolicy("Require-Branch-Admin", policy => policy.RequireClaim("Role", "BRANCH_ADMIN"))
            .AddPolicy("Require-Driver-MainAdmin", policy =>
                        policy.RequireAssertion(
                            context =>
                            context.User.HasClaim(
                                c => c.Type == "Role" &&
                                (c.Value == "MAIN_ADMIN" || c.Value == "DRIVER"))))
            .AddPolicy("RequireAdminOrBranchAdmin", policy =>
                        policy.RequireAssertion(
                            context =>
                            context.User.HasClaim(
                                c => c.Type == "Role" &&
                                (c.Value == "MAIN_ADMIN" || c.Value == "BRANCH_ADMIN"))))
            .AddPolicy("RequireAdminOrCounter", policy => policy.RequireAssertion(
                                           context =>
                                            context.User.HasClaim(
                                            c => c.Type == "Role" &&
                                            (c.Value == "MAIN_ADMIN" || c.Value == "COUNTER" || c.Value == "BRANCH_ADMIN"))))
            .AddPolicy("RequireAdminOrDriver", policy => policy.RequireAssertion(
                context => context.User.HasClaim(
                    c => c.Type == "Role" &&
                    (c.Value == "MAIN_ADMIN" || c.Value == "DRIVER" || c.Value == "BRANCH_ADMIN"))))
            .AddPolicy("RequireAnyRole", policy => policy.RequireAssertion(
                               context => context.User.HasClaim(
                                c => c.Type == "Role")))
            .AddPolicy("Require-Admin-Or-Admin-Branch", policy =>
                    policy.RequireAssertion(context =>
                        context.User.HasClaim(c =>
                            c.Type == "Role" && (c.Value == "MAIN_ADMIN" || c.Value == "BRAND_ADMIN"))
                    ));



        services.AddStackExchangeRedisCache(options =>
        {
            var connection = configuration.GetConnectionString("Redis");
            options.Configuration = connection;

        });

        services.AddScoped<IRedisService, RedisService>();
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<ICloudStorage, GoogleCloudStorage>();
        services.AddScoped<ICommandHandler<CreateMonthEmployeeSalaryCommand>, CreateMonthEmployeeSalaryCommandHandler>();
        services.AddScoped<ICommandHandler<CreateMonthlyCompanySalaryCommand>, CreateMonthlyCompanySalaryCommandHandler>();


        var apiSMSKey = "_82dj-Hon6EN0whmrvOIdWnDs9wYU-pU";
        var sender = "910266a06a597649";

        services.AddSingleton<ISpeedSMSAPI>(new SpeedSMSAPI(apiSMSKey, sender));

        services.ConfigureOptions<JwtOptionsSetup>();

        services.AddQuartz(options =>
        {
            options.UseMicrosoftDependencyInjectionJobFactory();

        });
        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });

        services.ConfigureOptions<LoggingBackgroundJobSetup>();
        services.ConfigureOptions<MonthlyCompanySalaryBackgroundJobSetup>();

        return services;
    }
}
