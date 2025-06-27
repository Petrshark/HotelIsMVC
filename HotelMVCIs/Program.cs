using HotelMVCIs.Data;
using HotelMVCIs.Services;
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

            builder.Services.AddDbContext<HotelMVCIsDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("HotelIsDbConnection")));

            builder.Services.AddScoped<RoomTypeService>();
            builder.Services.AddScoped<RoomService>();
            builder.Services.AddScoped<GuestService>();
            builder.Services.AddScoped<PaymentService>();
            builder.Services.AddScoped<ReservationService>();
            builder.Services.AddScoped<BookingChartService>();
            builder.Services.AddScoped<HotelServiceService>();

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
