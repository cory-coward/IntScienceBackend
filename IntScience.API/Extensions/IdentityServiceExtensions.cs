using IntScience.API.Services;
using IntScience.Repository;
using IntScience.Repository.IdentityModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace IntScience.API.Extensions;

public static class IdentityServiceExtensions
{
    public static void ConfigureIdentity(this IServiceCollection services, IConfiguration config)
    {
        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.User.RequireUniqueEmail = true;
        })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager<SignInManager<ApplicationUser>>();

        var jwtSecret = config.GetSection("JwtSecurityKey").Get<string>();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));

        services.AddAuthentication(authOptions =>
        {
            authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(bearerOptions =>
            {
                bearerOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = config.GetSection("JwtValidIssuer").Get<string>(),
                    ValidAudience = config.GetSection("JwtValidAudience").Get<string>(),
                    IssuerSigningKey = key,
                    ClockSkew = TimeSpan.FromMinutes(5)
                };
            });

        services.AddScoped<IJwtService, JwtService>();
    }
}
