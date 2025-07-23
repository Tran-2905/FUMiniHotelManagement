using FUMiniHotelManagement.DAL.Entities;
using FUMiniHotelManagement.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUMiniHotelManagement.BLL.Services
{
    public class BookingService
    {
        private readonly BookingRepository _repo;

        public BookingService()
        {
            _repo = new BookingRepository();
        }

        public List<BookingReservation> GetAllBookings()
            => _repo.GetAll();

        public BookingReservation? GetBookingById(int id)
            => _repo.GetById(id);

        public void AddBooking(BookingReservation booking)
        {
            // Validate cơ bản (ví dụ ngày, phòng, khách)

            if (booking == null || booking.BookingDetails == null || booking.BookingDetails.Count == 0)
                throw new System.Exception("Booking information is incomplete.");
            _repo.Add(booking);
        }

        public void UpdateBooking(BookingReservation booking)
        {
            if (booking == null || booking.BookingReservationId <= 0)
                throw new System.Exception("Booking to update not found.");
            _repo.Update(booking);
        }

        public void DeleteBooking(int id)
            => _repo.Delete(id);

        public List<BookingReservation> SearchByCustomer(string customerName)
            => _repo.SearchByCustomerName(customerName);

        public List<BookingReservation> GetBookingsByDate(System.DateOnly date)
            => _repo.GetBookingsByDate(date);
        private List<BookingReservation> SearchByCustomerName(string customerName)
        {
            if (string.IsNullOrWhiteSpace(customerName))
                throw new ArgumentException("Customer name cannot be null or empty.", nameof(customerName));
            return _repo.SearchByCustomerName(customerName);
        }
    }
}