using Application;
using Application.Common.Hubs;
using Common;
using Common.Configurations;
using Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Api
{
    public class Startup
    {
        private IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add services to the container
            services.AddApplication();
            services.AddCommon();
            services.AddInfrastrucutre(_configuration);

            services.AddControllers();

            services.AddCors();

            // swagger after controllers
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Broker API", Version = $"v{Assembly.GetExecutingAssembly().GetName().Version}" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference=new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{ }
                    }
                });
            });
        }

        public void Configure(IApplicationBuilder app,
                              IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            if (env.IsDevelopment())
            {
                // Enable middleware to serve generated Swagger as a JSON endpoint
                app.UseSwagger();
                // Enable middleware to serve swager - ui(HTML, JS, CSS, etc..),
                // specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Broker API");
                    options.RoutePrefix = "";
                });
                //app.UseHangfireDashboard();
            }

            app.UseRouting();
            app.UseCors(options =>
            {
                var section = _configuration.GetSection(CorsConfiguration.SECTION_NAME);
                var corsConfig = new CorsConfiguration();
                section.Bind(corsConfig);

                options.SetIsOriginAllowedToAllowWildcardSubdomains()
                       .WithOrigins(corsConfig.Origins)
                       .AllowAnyHeader()
                       .AllowAnyMethod()
                       .WithExposedHeaders(corsConfig.ExposedHeaders);
            });

            // auth before endpoints
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<NotificationHubService>("/notificationHub");
            });
        }
    }
}
