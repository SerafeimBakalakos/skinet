using Core.Entities.Identity;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

            services.AddAuthentication(); // Always: authentication before authorization
            services.AddAuthorization();

            return services;
        }
    }
}