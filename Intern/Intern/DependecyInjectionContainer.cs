using AutoMapper;
using Intern.Common.CustomMiddleware;
using Intern.Common.Helpers;
using Intern.Data;
using Intern.Services;
using Microsoft.EntityFrameworkCore;

namespace Intern.DependencyInjection
{
    public static class DependencyInjectionContainer
    {
        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register DbContext
            services.AddDbContext<ApiDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Register AutoMapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Register services
            services.AddScoped<Authservices>();
            services.AddScoped<EmailService>();
            services.AddScoped<DepartmentService>();
            services.AddScoped<PostService>();
            services.AddScoped<ImageHelper>();
            services.AddScoped<PasswordHelper>();
            services.AddScoped<TokenHelper>();
           
            
        }
    }
}
