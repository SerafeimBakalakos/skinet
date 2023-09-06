using API.Errors;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace API.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,
            IConfiguration config)
        {
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(); // Documents our API controllers into a JSON file
            services.AddDbContext<StoreContext>(opt =>
            {
                opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });
            services.AddSingleton<IConnectionMultiplexer>(c =>
            {
                var options = ConfigurationOptions.Parse(config.GetConnectionString("Redis"));
                return ConnectionMultiplexer.Connect(options);
            });

            // Scoped: lifetime of http request where service is created.
            // Transient: lifetime of method where service is created. Too short for data access services.
            // Singleton: lifetime of application. Good for caching, but too long for data access services.
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IBasketRepository, BasketRepository>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Override aspects of the behaviour of ApiController attribute
            // ApiController adds validation errors to the ModelState of the response
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    // We can access the ModelState (which is a Dictionary) here.
                    // We will just gather the error messages from it.
                    string[] errors = actionContext.ModelState
                        .Where(e => e.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value.Errors)
                        .Select(x => x.ErrorMessage).ToArray();

                    // Then return an error response with these messages
                    var errorResponse = new ApiValidationErrorResponse
                    {
                        Errors = errors
                    };
                    return new BadRequestObjectResult(errorResponse);
                };
            });

            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", policy =>
                {
                    // The browser accesses a web page, served by our client app. This page will try to send requests to the ApiServer, which may be on a different origin. 
                    // Browser sequrity prevents a page to make requests to a different domain than the one that served the page. This also prevents malicious sites from accessing sensitive data from the ApiServer.
                    // CORS relaxes this single-origin policy.
                    // See https://learn.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-7.0


                    // No restrictions to headers and methods.
                    // Our client-app will run on a different domain (https://localhost:4200) than our API server (https://localhost:5001), thus we must allow our client-app domain as a cross-origin
                    policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200");
                });
            });

            return services;
        }
    }
}