using HotelMVCIs.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelMVCIs
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<Services.RoomTypeService>();
            builder.Services.AddScoped<Services.RoomService>();
            builder.Services.AddScoped<Services.GuestService>();
            builder.Services.AddScoped<Services.ReservationService>();
            builder.Services.AddScoped<Services.BookingChartService>();
            builder.Services.AddScoped<Services.PaymentService>();
            builder.Services.AddScoped<Services.HotelServiceService>();

            builder.Services.AddDbContext<HotelMVCIsDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("HotelIsDbConnection")));

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

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
