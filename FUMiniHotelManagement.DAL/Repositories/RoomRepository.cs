using FUMiniHotelManagement.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUMiniHotelManagement.DAL.Repositories
{
    public class RoomRepository
    {
        private readonly FuminiHotelManagementContext _context;
        public RoomRepository()
        {
            _context = new FuminiHotelManagementContext();
        }
        public IEnumerable<RoomInformation> GetAllRooms()
        {
            return _context.RoomInformations.Include(r => r.RoomType).ToList();
        }
        public RoomInformation GetRoomById(int id)
        {
            return _context.RoomInformations.Find(id) ?? throw new KeyNotFoundException("Room not found with the provided ID.");
        }
        public void InsertRoom(RoomInformation room)
        {
            try
            {
                _context.RoomInformations.Add(room);
                Save();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while inserting the room.", ex);
            }
        }
        public void Save()
        {
            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while saving changes to the database.", ex);
            }
        }
        public void UpdateRoom(RoomInformation room)
        {
            try
            {
                _context.RoomInformations.Update(room);
                Save();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the room.", ex);
            }
        }
        public void DeleteRoom(int id)
        {
            var room = _context.RoomInformations.Find(id);
            if (room == null)
            {
                throw new KeyNotFoundException("Room not found with the provided ID.");
            }
            _context.RoomInformations.Remove(room);
            Save();
        }
    }
}
