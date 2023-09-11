using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Extensions
{
    public static class SwaggerServiceExtensions
    {
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(); // Documents our API controllers into a JSON file
            return services;
        }

        public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
        {
            // We will keep it in production too
            // if (app.Environment.IsDevelopment())
            // {
                // The Swagger JSON file is used to generate the UI
                app.UseSwagger();
                app.UseSwaggerUI();
            // }
            return app;
        }
    }
}