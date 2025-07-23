using FUMiniHotelManagement.DAL.Entities;
using FUMiniHotelManagement.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUMiniHotelManagement.BLL.Services
{
    public class RoomInformationService
    {
        private readonly RoomRepository _repo;
        public RoomInformationService()
        {
            _repo = new RoomRepository();
        }
        public List<RoomInformation> GetAllRooms()
        {
            return _repo.GetAllRooms().ToList();
        }
        public RoomInformation GetRoomById(int id)
        {
            return _repo.GetRoomById(id);
        }
        public void AddRoom(RoomInformation room)
        {
            if (room == null) throw new ArgumentNullException(nameof(room));
            _repo.InsertRoom(room);
        }
        public void UpdateRoom(RoomInformation room)
        {
            if (room == null) throw new ArgumentNullException(nameof(room));
            _repo.UpdateRoom(room);
        }
        public void DeleteRoom(int id)
        {
            var room = _repo.GetRoomById(id);
            if (room == null)
            {
                throw new KeyNotFoundException($"Room with ID {id} not found.");
            }
            _repo.DeleteRoom(id);
        }
        public List<RoomInformation> SearchRooms(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return GetAllRooms();
            }
            return _repo.GetAllRooms().Where(r => r.RoomDetailDescription.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
        }
    }
}
