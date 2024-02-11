using Application.Common.Hubs;
using Application.Common.Interfaces;
using Azure.Storage.Blobs;
using Common.Configurations;
using Domain.Entities;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SendGrid.Extensions.DependencyInjection;
using SendGrid.Helpers.Reliability;
using System;
using System.Text;

namespace Infrastructure
{
    public static class RegisterInfrastructure
    {
        public static IServiceCollection AddInfrastrucutre(this IServiceCollection services,
                                                           IConfiguration configuration)
        {
            services.AddOptions();
            services.AddSignalR();
            services.Configure<JwtConfiguration>(configuration.GetSection(JwtConfiguration.SECTION_NAME));
            services.Configure<ApplicationConfiguration>(configuration.GetSection(ApplicationConfiguration.SECTION_NAME));
            services.Configure<EmailConfiguration>(configuration.GetSection(EmailConfiguration.SECTION_NAME));
            services.Configure<TwilioConfiguration>(configuration.GetSection(TwilioConfiguration.SECTION_NAME));

            services.AddDbContext<BrokerDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("BrokerMechanics"));
            });
            services.AddScoped<IBrokerDbContext>(provider => provider.GetService<BrokerDbContext>());

            // For Identity
            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<BrokerDbContext>()
                .AddDefaultTokenProviders();

            // Adding Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            // Adding Jwt Bearer
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = configuration["JWT:Audience"],
                    ValidIssuer = configuration["JWT:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"])),
                    ClockSkew = TimeSpan.Zero
                };
            });

            // Sendgrid
            services.AddSendGrid(options =>
            {
                options.ApiKey = configuration["Email:Key"];
                options.ReliabilitySettings = new ReliabilitySettings(1, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(3));
            });

            // Hangfire currently not used. Ref for future.
            //services.AddHangfire(x => x.UseSqlServerStorage(configuration.GetConnectionString("BrokerMechanics")));
            //services.AddHangfireServer();

            var section = configuration.GetSection(AzureFileStoreConfiguration.SECTION_NAME);
            var config = section.Get<AzureFileStoreConfiguration>();
            services.AddSingleton(provider => new BlobContainerClient(config.ConnectionString, config.ContainerName));

            services.AddMemoryCache();

            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IMemoryCacheService, MemoryCacheService>();
            services.AddScoped<ITwilioService, TwilioService>();
            services.AddScoped<IAzureFileService, AzureFileService>();
            services.AddScoped<INotificationHubService, NotificationHubService>();
            services.AddScoped<IContextService>(option => new ContextService(configuration.GetConnectionString("BrokerMechanics")));

            return services;
        }
    }
}
