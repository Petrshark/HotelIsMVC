using HotelMVCIs.Data;
using HotelMVCIs.DTOs;
using HotelMVCIs.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelMVCIs.Services
{
    public class GuestService
    {
        private readonly HotelMVCIsDbContext _context;

        public GuestService(HotelMVCIsDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Guest>> GetAllAsync()
        {
            return await _context.Guests.OrderBy(g => g.LastName).ThenBy(g => g.FirstName).ToListAsync();
        }

        public async Task<GuestDTO> GetByIdAsync(int id)
        {
            var guest = await _context.Guests.FindAsync(id);
            if (guest == null) return null;

            return new GuestDTO
            {
                Id = guest.Id,
                FirstName = guest.FirstName,
                LastName = guest.LastName,
                DateOfBirth = guest.DateOfBirth,
                Email = guest.Email,
                PhoneNumber = guest.PhoneNumber,
                Address = guest.Address,
                City = guest.City,
                PostalCode = guest.PostalCode,
                Nationality = guest.Nationality
            };
        }

        public async Task CreateAsync(GuestDTO dto)
        {
            var guest = new Guest
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                DateOfBirth = dto.DateOfBirth,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                City = dto.City,
                PostalCode = dto.PostalCode,
                Nationality = dto.Nationality
            };
            _context.Guests.Add(guest);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(GuestDTO dto)
        {
            var guest = await _context.Guests.FindAsync(dto.Id);
            if (guest != null)
            {
                guest.FirstName = dto.FirstName;
                guest.LastName = dto.LastName;
                guest.DateOfBirth = dto.DateOfBirth;
                guest.Email = dto.Email;
                guest.PhoneNumber = dto.PhoneNumber;
                guest.Address = dto.Address;
                guest.City = dto.City;
                guest.PostalCode = dto.PostalCode;
                guest.Nationality = dto.Nationality;

                _context.Update(guest);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var guest = await _context.Guests.FindAsync(id);
            if (guest != null)
            {
                _context.Guests.Remove(guest);
                await _context.SaveChangesAsync();
            }
        }
    }
}