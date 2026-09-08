using System.Text.Json;

using AdminPanel.Application.Interfaces;
using AdminPanel.Application.Security;
using AdminPanel.Infrastructure.Repositories;
using AdminPanel.Ui.Services;
using AdminPanel.Ui.ViewModels;
using AdminPanel.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
//...


builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["ApiSettings:BaseUrl"]);
});


//...
builder.Services.AddScoped<UserAccessService>();
//builder.Services.AddScoped<IUserRepository, UserRepository>();
//builder.Services.AddScoped<PasswordHasher>();


//...
//Enables access to current user info throughout every part of UI project
builder.Services.AddHttpContextAccessor();

//...
//Here you tell the system that you want to use Cookie system in order to know my users
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
        options.Cookie.Name = "AdminPanelAuth";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
    });



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
