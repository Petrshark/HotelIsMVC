using HotelMVCIs.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HotelMVCIs.Data
{
    // Dědíme z IdentityDbContext<AppUser>, abychom zahrnuli tabulky pro uživatele a role
    public class HotelMVCIsDbContext : IdentityDbContext<AppUser>
    {
        public HotelMVCIsDbContext(DbContextOptions<HotelMVCIsDbContext> options) : base(options)
        {
        }

        // DbSet pro každou naši vlastní tabulku v databázi
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<HotelService> HotelServices { get; set; }
        public DbSet<ReservationItem> ReservationItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Je důležité zavolat base metodu, aby se správně nakonfigurovaly tabulky Identity
            base.OnModelCreating(modelBuilder);

            // Konfigurace pro naši spojovací tabulku ReservationItem
            modelBuilder.Entity<ReservationItem>()
                .HasKey(ri => new { ri.ReservationId, ri.HotelServiceId }); // Složený primární klíč

            // Konfigurace vazby z Reservation na ReservationItem
            modelBuilder.Entity<ReservationItem>()
                .HasOne(ri => ri.Reservation)
                .WithMany(r => r.ReservationItems)
                .HasForeignKey(ri => ri.ReservationId)
                .OnDelete(DeleteBehavior.Cascade); // Při smazání rezervace se smažou i její služby

            // Konfigurace vazby z HotelService na ReservationItem
            modelBuilder.Entity<ReservationItem>()
                .HasOne(ri => ri.HotelService)
                .WithMany(hs => hs.ReservationItems)
                .HasForeignKey(ri => ri.HotelServiceId);
        }
    }
}