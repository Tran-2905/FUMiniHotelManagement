using FUMiniHotelManagement.BLL.Services;
using FUMiniHotelManagement.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FUMiniHotelManagement
{
    /// <summary>
    /// Interaction logic for Booking.xaml
    /// </summary>
    public partial class Booking : Window
    {
        public BookingReservation EditedBooking { get; set; }
        private readonly BookingService _bookingService;
        private readonly CustomerService _customerService;
        private readonly RoomInformationService _roomService;

        private readonly List<bool> StatusList = new() { true, false };

        public Booking()
        {
            InitializeComponent();

            _bookingService = new BookingService();
            _customerService = new CustomerService();
            _roomService = new RoomInformationService();

            CustomerComboBox.ItemsSource = _customerService.GetAllCustomers();
            RoomsListBox.ItemsSource = _roomService.GetAllRooms().Where(r => r.RoomStatus == 1).ToList();
            StatusComboBox.ItemsSource = StatusList;

            BookingDatePicker.SelectedDate = DateTime.Today;
            StartDatePicker.SelectedDate = DateTime.Today;
            EndDatePicker.SelectedDate = DateTime.Today.AddDays(1);

            if (EditedBooking != null)
            {
                FillData(EditedBooking);
            }
            else
            {
                EditedBooking = new BookingReservation();
            }

            RoomsListBox.SelectionChanged += UpdateTotalPrice;
            StartDatePicker.SelectedDateChanged += UpdateTotalPrice;
            EndDatePicker.SelectedDateChanged += UpdateTotalPrice;
        }

        public void FillData(BookingReservation booking)
        {
            if (booking == null) return;

            CustomerComboBox.SelectedValue = booking.CustomerId;
            BookingDatePicker.SelectedDate = booking.BookingDate?.ToDateTime(TimeOnly.MinValue);
            StatusComboBox.SelectedIndex = booking.BookingStatus.HasValue ? booking.BookingStatus.Value : 0;
            TotalPriceBox.Text = booking.TotalPrice?.ToString("0.##") ?? "";

            if (booking.BookingDetails?.Any() == true)
            {
                var roomIds = booking.BookingDetails.Select(d => d.RoomId).Distinct().ToList();
                foreach (RoomInformation room in RoomsListBox.Items)
                {
                    if (roomIds.Contains(room.RoomId))
                        RoomsListBox.SelectedItems.Add(room);
                }
                var sampleDetail = booking.BookingDetails.First();
                StartDatePicker.SelectedDate = sampleDetail.StartDate.ToDateTime(TimeOnly.MinValue);
                EndDatePicker.SelectedDate = sampleDetail.EndDate.ToDateTime(TimeOnly.MinValue);
            }
        }

        private void UpdateTotalPrice(object sender, EventArgs e)
        {
            if (StartDatePicker.SelectedDate == null || EndDatePicker.SelectedDate == null)
            {
                TotalPriceBox.Text = "";
                return;
            }
            int days = Math.Max(1, (EndDatePicker.SelectedDate.Value - StartDatePicker.SelectedDate.Value).Days);
            decimal total = 0;
            foreach (RoomInformation room in RoomsListBox.SelectedItems)
                total += (room.RoomPricePerDay ?? 0) * days;
            TotalPriceBox.Text = total.ToString("0.##");
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Validate required fields
            if (CustomerComboBox.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn khách hàng.", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (RoomsListBox.SelectedItems.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất một phòng.", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (StartDatePicker.SelectedDate == null || EndDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Vui lòng chọn ngày bắt đầu và kết thúc.", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (StartDatePicker.SelectedDate > EndDatePicker.SelectedDate)
            {
                MessageBox.Show("Ngày bắt đầu không thể sau ngày kết thúc.", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (StatusComboBox.SelectedIndex < 0)
            {
                MessageBox.Show("Vui lòng chọn trạng thái đặt phòng.", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(TotalPriceBox.Text, out decimal totalPrice) || totalPrice < 0)
            {
                MessageBox.Show("Tổng tiền không hợp lệ.", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if(EditedBooking == null)
            { 
                EditedBooking = new BookingReservation();
            }
            EditedBooking.TotalPrice = totalPrice;
            EditedBooking.CustomerId = (int)CustomerComboBox.SelectedValue;
            EditedBooking.BookingDate = DateOnly.FromDateTime(BookingDatePicker.SelectedDate.Value);
            EditedBooking.BookingStatus = (byte)StatusComboBox.SelectedIndex;
            // Gán chi tiết phòng
            EditedBooking.BookingDetails = new List<BookingDetail>();
            foreach (RoomInformation room in RoomsListBox.SelectedItems)
            {
                EditedBooking.BookingDetails.Add(new BookingDetail
                {
                    RoomId = room.RoomId,
                    StartDate = DateOnly.FromDateTime(StartDatePicker.SelectedDate.Value),
                    EndDate = DateOnly.FromDateTime(EndDatePicker.SelectedDate.Value),
                    ActualPrice = room.RoomPricePerDay
                });
            }

            try
            {
                if (EditedBooking.BookingReservationId == 0 )
                {
                    // Update
                    _bookingService.AddBooking(EditedBooking);
                }
                else
                {
                    // Insert mới
                    _bookingService.UpdateBooking(EditedBooking);
                }

                MessageBox.Show("Lưu thông tin đặt phòng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu thông tin đặt phòng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}