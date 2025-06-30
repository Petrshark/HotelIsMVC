using HotelMVCIs.Data;
using HotelMVCIs.Models;
using HotelMVCIs.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

// Na�ten� connection stringu
var connectionString = builder.Configuration.GetConnectionString("HotelIsDbConnection");

// Registrace DbContextu
builder.Services.AddDbContext<HotelMVCIsDbContext>(options =>
    options.UseSqlServer(connectionString));

// Registrace a konfigurace ASP.NET Core Identity
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 4;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.SignIn.RequireConfirmedAccount = false;
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ " +
        "�����������������������؊���ݎ";
})
.AddEntityFrameworkStores<HotelMVCIsDbContext>();

builder.Services.ConfigureApplicationCookie(opts => {
    opts.LoginPath = "/Account/Login";
    opts.AccessDeniedPath = "/Account/AccessDenied";
});

// Registrace v�ech va�ich slu�eb
builder.Services.AddScoped<RoomTypeService>();
builder.Services.AddScoped<RoomService>();
builder.Services.AddScoped<GuestService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<ReservationService>();
builder.Services.AddScoped<BookingChartService>();
builder.Services.AddScoped<HotelServiceService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
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
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();