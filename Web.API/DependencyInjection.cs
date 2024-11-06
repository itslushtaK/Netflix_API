using CloudinaryDotNet;
using Domain.Constants;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Globalization;
using System.Text;
using Web.API.Middlewares;

namespace CodeInvention.ScalingUp.Web.API
{
    public static class DependecyInjection
    {
        private const string CORS_POLICY_NAME = "ScalingUpCORS";
        public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Scaling Up"
                });
                options.AddSecurityDefinition(name: "Bearer", securityScheme: new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Enter the Bearer Authorization string as following: `Bearer Generated-JWT-Token`",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        new List<string>()
                    }
                });

                options.CustomSchemaIds(type => type.FullName?.Replace("+", "."));

            });
            services.Configure<IdentityOptions>(options =>
            {
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
            });

            services.Configure<ApiBehaviorOptions>(config =>
            {
                config.SuppressModelStateInvalidFilter = true;
            });
            var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularApp", policy =>
                {
                    policy.WithOrigins("http://localhost:4200");
                    policy.AllowAnyMethod();
                    policy.AllowAnyHeader();
                });
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                   .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                   {
                       options.SaveToken = true;
                       options.RequireHttpsMetadata = false; 
                       options.TokenValidationParameters = new TokenValidationParameters
                       {
                           ValidateIssuer = true,
                           ValidateAudience = true,
                           ValidateLifetime = true,
                           ValidIssuer = configuration[ConfigurationConstant.AuthenticationIssuer], 
                           ValidAudience = configuration[ConfigurationConstant.AuthenticationAudience],
                           IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration[ConfigurationConstant.AuthenticationSecret])),
                           RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/Role",
                       };
                   });

            // Configure Authorization policies
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy =>
                {
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireRole("Admin");
                });

                options.AddPolicy("UserOnly", policy =>
                {
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireRole("User");
                });

                options.AddPolicy("AdminOrUser", policy =>
                {
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireRole("Admin", "User");
                });
            });

            services.AddControllers();

            services.AddDistributedMemoryCache();

            services.AddSignalR();

            services.AddRazorPages();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(5);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddLocalization();
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("de-DE"),
                    new CultureInfo("en-US"),
                    new CultureInfo("fr-FR"),
                    new CultureInfo("it-IT"),
                    new CultureInfo("sq-AL")
                };

                options.DefaultRequestCulture = new RequestCulture(culture: "de-DE", uiCulture: "de-DE");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            return services;
        }


        public static WebApplication UsePresentation(this WebApplication app)
        {
            app.UseMiddleware<ExceptionMiddleware>();

            //app.UseSwagger(options =>
            //{
            //    options.SerializeAsV2 = true;
            //});
            //app.UseSwaggerUI(options =>
            //{
            //    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            //    options.RoutePrefix = string.Empty;
            //    options.DocExpansion(DocExpansion.None);
            //    options.DefaultModelsExpandDepth(-1);
            //});

            //using (var scope = app.Services.CreateScope())
            //{
            //    var context = (ApplicationDbContext)scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
            //    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            //}

            //Configure Helpers
            //UserHelper.Configure(app.Services.GetRequiredService<IHttpContextAccessor>(), app.Services.GetRequiredService<IConfiguration>());

            app.UseSession();

            app.UseCors();

            var localizeOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(localizeOptions.Value);

            return app;
        }
    }
}
