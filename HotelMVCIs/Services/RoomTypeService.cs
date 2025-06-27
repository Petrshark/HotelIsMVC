using HotelMVCIs.Data;
using HotelMVCIs.DTOs;
using HotelMVCIs.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelMVCIs.Services
{
    public class RoomTypeService
    {
        private readonly HotelMVCIsDbContext _context;

        public RoomTypeService(HotelMVCIsDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RoomTypeDTO>> GetAllAsync()
        {
            return await _context.RoomTypes
                .Select(t => new RoomTypeDTO
                {
                    Id = t.Id,
                    Name = t.Name,
                    Capacity = t.Capacity
                })
                .ToListAsync();
        }

        public async Task<RoomTypeDTO?> GetByIdAsync(int id)
        {
            var type = await _context.RoomTypes.FindAsync(id);
            if (type == null) return null;

            return new RoomTypeDTO
            {
                Id = type.Id,
                Name = type.Name,
                Capacity = type.Capacity
            };
        }

        public async Task CreateAsync(RoomTypeDTO dto)
        {
            var type = new RoomType
            {
                Name = dto.Name,
                Capacity = dto.Capacity
            };
            _context.RoomTypes.Add(type);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(RoomTypeDTO dto)
        {
            var type = await _context.RoomTypes.FindAsync(dto.Id);
            if (type != null)
            {
                type.Name = dto.Name;
                type.Capacity = dto.Capacity;
                _context.Update(type);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var type = await _context.RoomTypes.FindAsync(id);
            if (type != null)
            {
                _context.RoomTypes.Remove(type);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.RoomTypes.AnyAsync(e => e.Id == id);
        }
    }
}