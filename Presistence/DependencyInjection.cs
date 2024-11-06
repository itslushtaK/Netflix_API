using Application.Common.Abstractions;
using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Presistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresistence(this IServiceCollection services , IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("Default")));
            //services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("ProdConnection")));



            //var mongoDbSettings = new MongoDbSettings();
            //configuration.GetSection("MongoDb").Bind(mongoDbSettings);
            //services.AddSingleton(mongoDbSettings);
            //services.AddSingleton<IMongoDbContext, MongoDbContext>();


            services.Configure<CloudinarySettings>(cloudinarySettings =>
            {
                cloudinarySettings.CloudName = configuration["CloudinarySettings:CloudName"];
                cloudinarySettings.ApiKey = configuration["CloudinarySettings:ApiKey"];
                cloudinarySettings.ApiSecret = configuration["CloudinarySettings:ApiSecret"];
            });


            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
            services.AddScoped<IFileStorageService, FileStorageService>();
            services.AddTransient<IPhotoService , PhotoService>();
            services.AddTransient<IMailService, SendGridMailService>();

            //services.AddScoped<ApplicationDbContextInitializer>();

            return services;
        }
    }
}
