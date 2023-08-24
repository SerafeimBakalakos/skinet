using API.Errors;
using API.Middleware;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // Documents our API controllers into a JSON file
builder.Services.AddDbContext<StoreContext>(opt => 
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Scoped: lifetime of http request where service is created.
// Transient: lifetime of method where service is created. Too short for data access services.
// Singleton: lifetime of application. Good for caching, but too long for data access services.
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Override aspects of the behaviour of ApiController attribute
// ApiController adds validation errors to the ModelState of the response
builder.Services.Configure<ApiBehaviorOptions>(options =>  
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

var app = builder.Build();

// Configure the HTTP request pipeline.

// There is a hidden piece of middleware here, called developer exception page.
// Errors are thrown up the pipeline (from commands under this line towards it), until it can be handled
// Thus our exception handling should be done at the start of the pipeline
app.UseMiddleware<ExceptionMiddleware>();

app.UseStatusCodePagesWithReExecute("/errors/{0}"); // This handles requests to endpoints that do not exist

// We will keep it in production too
// if (app.Environment.IsDevelopment())
// {
    // The Swagger JSON file is used to generate the UI
    app.UseSwagger();
    app.UseSwaggerUI();
// }
    
app.UseStaticFiles(); //position before UseAuthorization

//app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

// Create the database (unless it exists) and apply pending migrations
using var scope = app.Services.CreateScope(); // Get a temporary Scope to access services
var services = scope.ServiceProvider;
var context = services.GetRequiredService<StoreContext>();
var logger = services.GetRequiredService<ILogger<Program>>();
try
{
    await context.Database.MigrateAsync();
    await StoreContextSeed.SeedAsync(context);
}
catch (Exception ex)
{
    logger.LogError(ex, "An error occured during migration");
}

app.Run();
