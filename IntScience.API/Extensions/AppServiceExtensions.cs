using IntScience.Repository;
using Microsoft.EntityFrameworkCore;

namespace IntScience.API.Extensions;

public static class AppServiceExtensions
{
    public static void ConfigureCors(this IServiceCollection services, IConfiguration config)
    {
        var corsOrigins = config.GetSection("CorsOrigins").Get<string>() ?? "";

        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
            {
                builder.AllowAnyMethod().AllowAnyHeader();
                builder = corsOrigins == "" ? builder.AllowAnyOrigin() : builder.WithOrigins(corsOrigins);
            });
        });
    }

    public static void ConfigureDatabase(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(config.GetConnectionString("ApplicationConnection"));
        });
    }

    public static void ConfigureIISIntegration(this IServiceCollection services)
    {
        services.Configure<IISOptions>(options => { });
    }
}
