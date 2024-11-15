using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using NewsWebsite.Helpers;
using NewsWebsite.Models;
using NewsWebsite.Services;
using SweetAlert2;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<NewswebsiteContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("NEWSWEBSITE"));
});

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = "/Admin";
    options.AccessDeniedPath = "/Admin/AccessDenied";
});

builder.Services.AddSweetAlert2();
// Bind SMTP settings from configuration
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

// Register the email service
builder.Services.AddTransient<IEmailService, EmailService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
