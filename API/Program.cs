using API.Errors;
using API.Extensions;
using API.Middleware;
using Core.Entities.Identity;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

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
app.UseCors("CorsPolicy"); // Must be just before UseAuthorization(). Use the same name as when defining the policy in the services.

app.UseAuthentication(); // Always authentication before authorization
app.UseAuthorization();

app.MapControllers();

// Create the database (unless it exists) and apply pending migrations
using var scope = app.Services.CreateScope(); // Get a temporary Scope to access services
var services = scope.ServiceProvider;
var context = services.GetRequiredService<StoreContext>();
var identityContext = services.GetRequiredService<AppIdentityDbContext>();
var userManager = services.GetRequiredService<UserManager<AppUser>>();
var logger = services.GetRequiredService<ILogger<Program>>();
try
{
    await context.Database.MigrateAsync();
    await identityContext.Database.MigrateAsync();
    await StoreContextSeed.SeedAsync(context);
    await AppIdentityDbContextSeed.SeedUsersAsync(userManager);
}
catch (Exception ex)
{
    logger.LogError(ex, "An error occured during migration");
}

app.Run();
