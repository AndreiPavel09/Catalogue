using Backend.Data;
using Backend.Repositories;
using Backend.Repositories.Implementations;
using Backend.Repositories.Interfaces;
using Backend.Services;
using Backend.Services.Implementations;
using Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backend
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Register repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<IGradeRepository, GradeRepository>();
            services.AddScoped<ICourseRepository, CourseRepository>();

            // Register services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IGradeService, GradeService>();
            services.AddScoped<ICourseService, CourseService>();

            return services;
        }
    }
}