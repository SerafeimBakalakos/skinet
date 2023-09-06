using System.Text;
using Core.Entities.Identity;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices(
            this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<AppIdentityDbContext>(opt =>
            {
                opt.UseSqlite(config.GetConnectionString("IdentityConnection"));
            });
            
            services.AddIdentityCore<AppUser>(opt =>
            {
                // Idenity password options
                // opt.Password.RequiredLength = 10;
            })
            .AddEntityFrameworkStores<AppIdentityDbContext>()
            .AddSignInManager<SignInManager<AppUser>>(); // Idenity's UserManager can also be used for sign-in, but SignInManager has more features.

            // Always: authentication before authorization
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme) // type of authentication: JWT
                .AddJwtBearer(opt =>
                {
                    // How to validate the token
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true, // ensure that the token was signed by the same server
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Token:Key"])), // what it is checking against
                        ValidIssuer = config["Token:Issuer"],
                        ValidateIssuer = true,
                        ValidateAudience = false // e.g. only accept tokens from our client app
                    };
                }); 

            services.AddAuthorization(); 

            return services;
        }
    }
}