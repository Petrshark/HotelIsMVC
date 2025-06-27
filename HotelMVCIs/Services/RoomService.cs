using HotelMVCIs.Data;
using HotelMVCIs.DTOs;
using HotelMVCIs.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelMVCIs.Services
{
    public class RoomService
    {
        private readonly HotelMVCIsDbContext _context;

        public RoomService(HotelMVCIsDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Room>> GetAllAsync()
        {
            return await _context.Rooms
                .Include(r => r.RoomType)
                .OrderBy(r => r.RoomNumber)
                .ToListAsync();
        }

        public async Task<RoomDTO?> GetByIdAsync(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return null;

            return new RoomDTO
            {
                Id = room.Id,
                RoomNumber = room.RoomNumber,
                RoomTypeId = room.RoomTypeId,
                PricePerNight = room.PricePerNight,
                Description = room.Description
            };
        }

        public async Task<Room?> GetRoomForDeleteAsync(int id)
        {
            return await _context.Rooms
                .Include(p => p.RoomType)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task CreateAsync(RoomDTO dto)
        {
            var room = new Room
            {
                RoomNumber = dto.RoomNumber,
                RoomTypeId = dto.RoomTypeId,
                PricePerNight = dto.PricePerNight,
                Description = dto.Description
            };
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(RoomDTO dto)
        {
            var room = await _context.Rooms.FindAsync(dto.Id);
            if (room != null)
            {
                room.RoomNumber = dto.RoomNumber;
                room.RoomTypeId = dto.RoomTypeId;
                room.PricePerNight = dto.PricePerNight;
                room.Description = dto.Description;
                _context.Update(room);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room != null)
            {
                _context.Rooms.Remove(room);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Rooms.AnyAsync(e => e.Id == id);
        }
    }
}