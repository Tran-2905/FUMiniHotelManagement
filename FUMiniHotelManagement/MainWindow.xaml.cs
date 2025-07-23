using FUMiniHotelManagement.BLL.Services;
using FUMiniHotelManagement.DAL;
using FUMiniHotelManagement.DAL.Entities;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FUMiniHotelManagement
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string? UserName { get; set; }
        public int? Role { get; set; }
        CustomerService _customer;
        RoomInformationService _roomInformationService;
        BookingService _bookingDetailService;
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += Window_Loaded;
            _customer = new CustomerService();
            _roomInformationService = new RoomInformationService();
            _bookingDetailService = new BookingService();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Initialize the main window with user information
            if (UserName != null)
            {
                lblUserName.Content = $"Welcome, {UserName}";
            }
            else
            {
                lblUserName.Content = "Welcome, Guest";
            }
            bool isAdmin = (Role == 1);

            addBtnCus.IsEnabled = isAdmin;
            editBtnCus.IsEnabled = isAdmin;
            deleteBtnCus.IsEnabled = isAdmin;

            addBtnRoom.IsEnabled = isAdmin;
            editBtnRoom.IsEnabled = isAdmin;
            deleteBtnRoom.IsEnabled = isAdmin;

            addBtnBooking.IsEnabled = isAdmin;
            editBtnBooking.IsEnabled = isAdmin;
            deleteBtnBooking.IsEnabled = isAdmin;

        }

        private void CustomerDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                CustomerDataGrid.ItemsSource = null; // Clear the previous items
                var customers = _customer.GetAllCustomers();
                CustomerDataGrid.ItemsSource = customers;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading customers: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RoomDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                RoomDataGrid.ItemsSource = null; // Clear the previous items
                var rooms = _roomInformationService.GetAllRooms();
                RoomDataGrid.ItemsSource = rooms;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading rooms: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void BookingDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var bookings = _bookingDetailService.GetAllBookings();
                BookingDataGrid.ItemsSource = bookings;
                BookingDetailDataGrid.ItemsSource = null; // Clear previous booking details
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading booking: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        

        private void BookingDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(BookingDataGrid.SelectedItem is BookingReservation selectedBooking)
            {
                // Hiển thị chi tiết đặt phòng tương ứng
                BookingDetailDataGrid.ItemsSource = selectedBooking.BookingDetails;
            }
            else
            {
                BookingDetailDataGrid.ItemsSource = null; // Clear previous booking details
            }
        }

        private void AddCustomer_Click(object sender, RoutedEventArgs e)
        {
            CustomerDialog customerDialog = new CustomerDialog();
            var result = customerDialog.ShowDialog();
            if (result == true)
            {
                CustomerDataGrid_Loaded(null, null); // Nạp lại danh sách cho data grid
            }
        }

        private void EditCustomer_Click(object sender, RoutedEventArgs e)
        {
            CustomerDialog customerDialog = new CustomerDialog();
            if (CustomerDataGrid.SelectedItem is Customer selectedCustomer)
            {
                customerDialog.EditedCustomer = selectedCustomer;
                customerDialog.FillData(selectedCustomer);
                var result = customerDialog.ShowDialog();
                if (result == true)
                {
                    CustomerDataGrid_Loaded(null, null); // Nạp lại danh sách
                }
            }
            else
            {
                
                MessageBox.Show("Please select a customer to edit.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        private void DeleteCustomer_Click(object sender, RoutedEventArgs e)
        {
            if(CustomerDataGrid.SelectedItem is Customer selectedCustomer)
            {
                var result = MessageBox.Show($"Are you sure you want to delete customer {selectedCustomer.CustomerFullName}?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    _customer.DeleteCustomer(selectedCustomer.CustomerId);
                    CustomerDataGrid_Loaded(null, null); // Nạp lại danh sách
                }
            }
            else
            {
                MessageBox.Show("Please select a customer to delete.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnSearchCustomer_Click(object sender, RoutedEventArgs e)
        {
            string searchText = txSearchCustomer.Text.Trim();
            if (string.IsNullOrEmpty(searchText))
            {
                MessageBox.Show("Please enter a name to search.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var customers = _customer.SearchCustomers(searchText);
            if (customers.Count == 0)
            {
                MessageBox.Show("No customers found with that name.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            CustomerDataGrid.ItemsSource = customers;
        }

        private void btnSearchRoom_Click(object sender, RoutedEventArgs e)
        {
            string searchText = txSearchRoom.Text.Trim();
            if (string.IsNullOrEmpty(searchText))
            {
                MessageBox.Show("Please enter a room number to search.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var rooms = _roomInformationService.SearchRooms(searchText);
            if (rooms.Count == 0)
            {
                MessageBox.Show("No rooms found with that number.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            RoomDataGrid.ItemsSource = rooms;
        }

        private void btnSearchBooking_Click(object sender, RoutedEventArgs e)
        {
            string searchText = txSearchBooking.Text.Trim();
            if (string.IsNullOrEmpty(searchText))
            {
                MessageBox.Show("Please enter a booking ID to search.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var bookings = _bookingDetailService.SearchByCustomer(searchText);
            if (bookings.Count == 0)
            {
                MessageBox.Show("No bookings found with that ID.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            BookingDataGrid.ItemsSource = bookings;
        }

        private void AddRoom_Click(object sender, RoutedEventArgs e)
        {
            RoomDialog roomDialog = new RoomDialog();
            var result = roomDialog.ShowDialog();
            if (result == true)
            {
                RoomDataGrid_Loaded(null, null); // Nạp lại danh sách phòng vào DataGrid
            }
        }



        private void EditRoom_Click(object sender, RoutedEventArgs e)
        {
            if (RoomDataGrid.SelectedItem is RoomInformation selectedRoom)
            {
                RoomDialog roomDialog = new RoomDialog();
                roomDialog.EditedRoom = selectedRoom;
                roomDialog.FillData(selectedRoom);

                var result = roomDialog.ShowDialog();
                if (result == true)
                {
                    RoomDataGrid_Loaded(null, null); // Nạp lại danh sách phòng
                }
            }
            else
            {
                MessageBox.Show("Please select a room to edit.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void DeleteRoom_Click(object sender, RoutedEventArgs e)
        {
            if (RoomDataGrid.SelectedItem is RoomInformation selectedRoom)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete room {selectedRoom.RoomNumber}?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

                if (result == MessageBoxResult.Yes)
                {
                    _roomInformationService.DeleteRoom(selectedRoom.RoomId);
                    RoomDataGrid_Loaded(null, null); // Nạp lại danh sách phòng
                }
            }
            else
            {
                MessageBox.Show("Please select a room to delete.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void AddBooking_Click(object sender, RoutedEventArgs e)
        {
            Booking bookingWindow = new Booking();
            var result = bookingWindow.ShowDialog();
            if (result == true)
            {
                BookingDataGrid_Loaded(null, null); // Nạp lại danh sách đặt phòng
            }
        }

        

        private void EditBooking_Click(object sender, RoutedEventArgs e)
        {
            if (BookingDataGrid.SelectedItem is BookingReservation selectedBooking)
            {
                Booking bookingWindow = new Booking();
                bookingWindow.EditedBooking = selectedBooking;
                bookingWindow.FillData(selectedBooking);
                var result = bookingWindow.ShowDialog();
                if (result == true)
                {
                    BookingDataGrid_Loaded(null, null); // Nạp lại danh sách đặt phòng
                }
            }
            else
            {
                MessageBox.Show("Please select a booking to edit.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void DeleteBooking_Click(object sender, RoutedEventArgs e)
        {
            if (BookingDataGrid.SelectedItem is BookingReservation selectedBooking)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete booking for customer {selectedBooking.Customer.CustomerFullName}?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );
                if (result == MessageBoxResult.Yes)
                {
                    _bookingDetailService.DeleteBooking(selectedBooking.BookingReservationId);
                    BookingDataGrid_Loaded(null, null); // Nạp lại danh sách đặt phòng
                }
            }
            else
            {
                MessageBox.Show("Please select a booking to delete.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("You have logged out successfully.", "Logout", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Role = null; // Clear the role
            Login loginWindow = new Login();
            loginWindow.Show();
            this.Close();
        }
    }

}