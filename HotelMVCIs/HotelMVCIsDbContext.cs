using HotelMVCIs.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelMVCIs.Data
{
    public class HotelMVCIsDbContext : DbContext
    {
        public HotelMVCIsDbContext(DbContextOptions<HotelMVCIsDbContext> options) : base(options)
        {
        }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<HotelService> HotelServices { get; set; }
    }
}