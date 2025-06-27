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
        public DbSet<ReservationItem> ReservationItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Konfigurace pro spojovací tabulku ReservationItem
            modelBuilder.Entity<ReservationItem>()
                .HasKey(ri => new { ri.ReservationId, ri.HotelServiceId });

            modelBuilder.Entity<ReservationItem>()
                .HasOne(ri => ri.Reservation)
                .WithMany(r => r.ReservationItems)
                .HasForeignKey(ri => ri.ReservationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ReservationItem>()
                .HasOne(ri => ri.HotelService)
                .WithMany(hs => hs.ReservationItems)
                .HasForeignKey(ri => ri.HotelServiceId);
        }
    }
}