using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Repository;
using UserManagement.Infrastructure.Authorization;
using UserManagement.Infrastructure.DBContext;
using UserManagement.Infrastructure.Repository;
using UserManagement.Service;
using UserManagement.StartUp.JWT;
using UserManagement.StartUp.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json");

builder.Services.AddDbContext<UserManagementContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("UserManagementConnection"), b => b.MigrationsAssembly("UserManagement.Infrastructure")));

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureJwtServices(builder.Configuration);
builder.Services.AddSwaggerGen();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtAuthorization, JwtAuthorization>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<JwtMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();
app.MapRazorPages();  // For Razor Pages

app.MapDefaultControllerRoute();

app.Run();
