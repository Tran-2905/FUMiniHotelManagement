using FUMiniHotelManagement.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUMiniHotelManagement.DAL.Repositories
{
    public class BookingRepository
    {
        private readonly FuminiHotelManagementContext _context;

        public BookingRepository()
        {
            _context = new FuminiHotelManagementContext();
        }
        public List<BookingReservation> GetAll()
        {
            // Lấy toàn bộ booking kèm theo BookingDetail, Customer và Room
            return _context.BookingReservations
                .Include(b => b.Customer)
                .Include(b => b.BookingDetails)
                    .ThenInclude(d => d.Room)
                    .ThenInclude(r => r.RoomType)
                .AsNoTracking()
                .ToList();
        }

        public BookingReservation? GetById(int bookingId)
        {
            return _context.BookingReservations
                .Include(b => b.Customer)
                .Include(b => b.BookingDetails)
                    .ThenInclude(d => d.Room)
                .FirstOrDefault(b => b.BookingReservationId == bookingId);
        }

        public void Add(BookingReservation booking)
        {
            int maxId = _context.BookingReservations.Any()
               ? _context.BookingReservations.Max(b => b.BookingReservationId)
               : 0;
            booking.BookingReservationId = maxId+1;
            if (booking == null) return;
            _context.BookingReservations.Add(booking);
            _context.SaveChanges();
        }

        public void Update(BookingReservation booking)
        {
            if (booking == null) return;
            // Update BookingReservation & BookingDetails
            var old = _context.BookingReservations
                .Include(b => b.BookingDetails)
                .FirstOrDefault(b => b.BookingReservationId == booking.BookingReservationId);
            if (old != null)
            {
                // Cập nhật trường gốc
                old.BookingDate = booking.BookingDate;
                old.TotalPrice = booking.TotalPrice;
                old.CustomerId = booking.CustomerId;
                old.BookingStatus = booking.BookingStatus;

                // Xóa hết BookingDetails cũ, thêm BookingDetails mới
                _context.BookingDetails.RemoveRange(old.BookingDetails);
                if (booking.BookingDetails != null)
                    foreach (var detail in booking.BookingDetails)
                    {
                        detail.BookingReservationId = old.BookingReservationId; // Đảm bảo đúng FK
                        _context.BookingDetails.Add(detail);
                    }
                _context.SaveChanges();
            }
        }

        public void Delete(int bookingId)
        {
            var booking = _context.BookingReservations
                .Include(b => b.BookingDetails)
                .FirstOrDefault(b => b.BookingReservationId == bookingId);
            if (booking != null)
            {
                _context.BookingDetails.RemoveRange(booking.BookingDetails);
                _context.BookingReservations.Remove(booking);
                _context.SaveChanges();
            }
        }

        public List<BookingReservation> SearchByCustomerName(string customerName)
        {
            return _context.BookingReservations
                .Include(b => b.Customer)
                .Include(b => b.BookingDetails)
                    .ThenInclude(d => d.Room)
                .Where(b => b.Customer.CustomerFullName.Contains(customerName))
                .AsNoTracking()
                .ToList();
        }

        public List<BookingReservation> GetBookingsByDate(DateOnly date)
        {
            return _context.BookingReservations
                .Include(b => b.Customer)
                .Include(b => b.BookingDetails)
                    .ThenInclude(d => d.Room)
                .Where(b => b.BookingDate == date)
                .AsNoTracking()
                .ToList();
        }
    }
}