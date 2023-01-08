using System;
using System.Text;
using API.Data;
using API.Services;
using API.Services.Implementations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace API.Extensions
{
	public static class ApplicationServiceExtensions
	{
		public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
		{
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebAPIv5", Version = "v1" });
            });
            
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });
            services.AddScoped<ITokenService, TokenService>();
            

            return services;
        }
	}
}

