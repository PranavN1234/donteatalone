using System.Text;
using API.data;
using API.Interfaces;
using API.Services;
using API.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using API.Middleware;
using Microsoft.AspNetCore.Identity;
using API.Entities;
using API;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);
var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseMiddleware<ExceptionMiddleware>();



app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("https://localhost:4200"));

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapHub<PresenceHub>("hubs/presence");
app.MapHub<MessageHub>("hubs/message");


using var scope = app.Services.CreateScope();

var services = scope.ServiceProvider;

try{
    var context = services.GetRequiredService<DataContext>();
    await context.Database.MigrateAsync();
    var userManager = services.GetRequiredService<UserManager<Appuser>>();
    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
    await Seed.SeedUsers(userManager, roleManager);
}catch(Exception ex){
    var logger = services.GetService<ILogger<Program>>();

    logger.LogError(ex, "An error has occured during migration");
}


app.Run();
