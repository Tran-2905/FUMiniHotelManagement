using FUMiniHotelManagement.BLL.Services;
using FUMiniHotelManagement.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FUMiniHotelManagement
{
    /// <summary>
    /// Interaction logic for RoomDialog.xaml
    /// </summary>
    public partial class RoomDialog : Window
    {
        public RoomInformation EditedRoom { get; set; } = null;
        private readonly RoomInformationService _roomService;

        public RoomDialog()
        {
            InitializeComponent();
            _roomService = new RoomInformationService();
            FillData(EditedRoom);
        }

        public void FillData(RoomInformation room)
        {
            if (room == null)
                return;

            RoomNumberBox.Text = room.RoomNumber;
            RoomDetailDescBox.Text = room.RoomDetailDescription;
            MaxCapacityBox.Text = room.RoomMaxCapacity?.ToString();
            RoomTypeIdBox.Text = room.RoomTypeId.ToString();
            StatusBox.Text = room.RoomStatus?.ToString();
            PricePerDayBox.Text = room.RoomPricePerDay?.ToString();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Kiểm tra không để trống
            if (string.IsNullOrWhiteSpace(RoomNumberBox.Text)
                || string.IsNullOrWhiteSpace(RoomTypeIdBox.Text))
            {
                MessageBox.Show("Room Number và Room Type ID không được để trống.", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Kiểm tra Room Number hợp lệ (vd: không có ký tự đặc biệt)
            if (!Regex.IsMatch(RoomNumberBox.Text, @"^[A-Za-z0-9\s\-]+$"))
            {
                MessageBox.Show("Room Number chỉ được phép gồm chữ, số, dấu cách, hoặc dấu gạch ngang.", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Kiểm tra sức chứa (nếu có nhập) là số nguyên dương
            if (!string.IsNullOrWhiteSpace(MaxCapacityBox.Text) &&
                (!int.TryParse(MaxCapacityBox.Text, out int maxCap) || maxCap <= 0))
            {
                MessageBox.Show("Sức chứa tối đa phải là số nguyên dương.", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Kiểm tra RoomTypeId là số nguyên dương
            if (!int.TryParse(RoomTypeIdBox.Text, out int roomTypeId) || roomTypeId <= 0)
            {
                MessageBox.Show("Room Type ID phải là số nguyên dương.", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Kiểm tra giá phòng (nếu có nhập) là số dương
            if (!string.IsNullOrWhiteSpace(PricePerDayBox.Text) &&
                (!decimal.TryParse(PricePerDayBox.Text, out decimal price) || price < 0))
            {
                MessageBox.Show("Giá phòng mỗi ngày phải là số dương (hoặc bằng 0).", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Kiểm tra trạng thái phòng (nếu có nhập)
            if (!string.IsNullOrWhiteSpace(StatusBox.Text) &&
                !byte.TryParse(StatusBox.Text, out byte status))
            {
                MessageBox.Show("Trạng thái phòng phải là số từ 0 đến 255.", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Gán dữ liệu vào đối tượng
            if (EditedRoom == null)
            {
                EditedRoom = new RoomInformation();
            }

            EditedRoom.RoomNumber = RoomNumberBox.Text.Trim();
            EditedRoom.RoomDetailDescription = RoomDetailDescBox.Text;
            EditedRoom.RoomMaxCapacity = string.IsNullOrWhiteSpace(MaxCapacityBox.Text) ? null : int.Parse(MaxCapacityBox.Text);
            EditedRoom.RoomTypeId = int.Parse(RoomTypeIdBox.Text);
            EditedRoom.RoomStatus = string.IsNullOrWhiteSpace(StatusBox.Text) ? null : byte.Parse(StatusBox.Text);
            EditedRoom.RoomPricePerDay = string.IsNullOrWhiteSpace(PricePerDayBox.Text) ? null : decimal.Parse(PricePerDayBox.Text);

            try
            {
                // Lưu xuống CSDL
                if (EditedRoom.RoomId == 0) // Add new
                {
                    _roomService.AddRoom(EditedRoom);
                }
                else // Update
                {
                    _roomService.UpdateRoom(EditedRoom);
                }

                MessageBox.Show("Thông tin phòng đã lưu thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu thông tin phòng: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            this.Close();
        }
    }
}