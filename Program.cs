using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orb.Web.Data;
using Orb.Web.Services;
using System;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Set the DataDirectory to the application's root folder.
AppDomain.CurrentDomain.SetData("DataDirectory", Directory.GetCurrentDirectory());

try
{
    // Configure configuration sources
    builder.Configuration
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables();

    if (builder.Configuration.GetValue<string[]>("CommandLineArgs") != null)
    {
        builder.Configuration.AddCommandLine(builder.Configuration.GetValue<string[]>("CommandLineArgs"));
    }

    // Configure logging
    builder.Logging.ClearProviders();
    builder.Logging.AddConsole();
    builder.Logging.AddDebug();
    builder.Logging.AddEventSourceLogger();
    // Uncomment to add file logging
    // builder.Logging.AddFile("logs/orb-{Date}.log");

    // Add database context
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

    // Add services to the container
    builder.Services.AddControllersWithViews();
    builder.Services.AddRazorPages();

    // Add HttpContext accessor
    builder.Services.AddHttpContextAccessor();

    // Add cookie authentication
    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.LogoutPath = "/Account/Logout";
            options.AccessDeniedPath = "/Account/AccessDenied";
            options.ExpireTimeSpan = TimeSpan.FromDays(30);
        });

    // Add authorization policies
    builder.Services.AddAuthorization(options =>
    {
        // Example policy
        options.AddPolicy("RequireAdminRole", policy =>
            policy.RequireRole("Admin"));
    });

    builder.Services.AddScoped<ApplicationDbContext>();
    builder.Services.AddScoped<BlogService>();
    builder.Services.AddScoped<UserService>();

    var app = builder.Build();

    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    app.MapRazorPages();

    app.Run();
}
catch (Exception ex)
{
    // Fallback logging if startup fails
    File.WriteAllText("startup_error.log", ex.ToString());
    throw;
}
