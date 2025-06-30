using HotelMVCIs.Data;
using HotelMVCIs.DTOs;
using HotelMVCIs.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelMVCIs.Services
{
    public class HotelServiceService
    {
        private readonly HotelMVCIsDbContext _context;

        public HotelServiceService(HotelMVCIsDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<HotelServiceDTO>> GetAllAsync()
        {
            return await _context.HotelServices
                .Select(s => new HotelServiceDTO
                {
                    Id = s.Id,
                    Name = s.Name,
                    Price = s.Price,
                    Description = s.Description,
                    IsActive = s.IsActive
                })
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<HotelServiceDTO?> GetByIdAsync(int id)
        {
            var service = await _context.HotelServices.FindAsync(id);
            if (service == null) return null;

            return new HotelServiceDTO
            {
                Id = service.Id,
                Name = service.Name,
                Price = service.Price,
                Description = service.Description,
                IsActive = service.IsActive
            };
        }

        public async Task CreateAsync(HotelServiceDTO dto)
        {
            var service = new HotelService
            {
                Name = dto.Name,
                Price = dto.Price,
                Description = dto.Description,
                IsActive = dto.IsActive
            };
            _context.HotelServices.Add(service);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(HotelServiceDTO dto)
        {
            var service = await _context.HotelServices.FindAsync(dto.Id);
            if (service != null)
            {
                service.Name = dto.Name;
                service.Price = dto.Price;
                service.Description = dto.Description;
                service.IsActive = dto.IsActive;

                _context.Update(service);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var service = await _context.HotelServices.FindAsync(id);
            if (service != null)
            {
                _context.HotelServices.Remove(service);
                await _context.SaveChangesAsync();
            }
        }
    }
}