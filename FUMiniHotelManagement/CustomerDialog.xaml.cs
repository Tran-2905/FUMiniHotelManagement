using FUMiniHotelManagement.BLL.Services;
using FUMiniHotelManagement.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for CustomerDialog.xaml
    /// </summary>
    public partial class CustomerDialog : Window
    {
        public Customer EditedCustomer { get; set; } = null;
        private readonly CustomerService _service;
        public CustomerDialog()
        {
            InitializeComponent();
            FillData(EditedCustomer);
            _service = new CustomerService();
        }
        public void FillData(Customer customer)
        {
            if (customer == null)
                return;

            // Gán dữ liệu vào các control
            FullNameBox.Text = customer.CustomerFullName;
            TelephoneBox.Text = customer.Telephone;
            EmailBox.Text = customer.EmailAddress;

            // Check if CustomerBirthday has a value before accessing it
            if (customer.CustomerBirthday.HasValue)
            {
                BirthdayPicker.SelectedDate = customer.CustomerBirthday.Value.ToDateTime(TimeOnly.MinValue);
            }
            else
            {
                BirthdayPicker.SelectedDate = null; // Handle null case
            }

            PasswordBox.Password = customer.Password; // lưu ý: hiếm khi hiển thị lại password
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FullNameBox.Text) ||
        string.IsNullOrWhiteSpace(TelephoneBox.Text) ||
        string.IsNullOrWhiteSpace(EmailBox.Text) ||
        BirthdayPicker.SelectedDate == null ||
        string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                MessageBox.Show("Please fill in all fields.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // HỌ VÀ TÊN TỪ 3-50 KÝ TỰ
            if (FullNameBox.Text.Length < 3 || FullNameBox.Text.Length > 50)
            {
                MessageBox.Show("Full Name must be between 3 and 50 characters.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // HỌ VÀ TÊN - mỗi từ bắt đầu hoa, không ký tự đặc biệt:
            if (!Regex.IsMatch(FullNameBox.Text, @"^([A-Z][a-z]+(?:\s)?)+$"))
            {
                MessageBox.Show("Each word in Full Name must start with a capital letter and contain no special character.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // TELEPHONE bắt đầu bằng số, độ dài 9-15 ký tự
            if (!Regex.IsMatch(TelephoneBox.Text, @"^\d{9,15}$"))
            {
                MessageBox.Show("Invalid Telephone number. It must be 9-15 digits.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // EMAIL format
            if (!Regex.IsMatch(EmailBox.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Invalid Email address.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // NGÀY SINH không thể lớn hơn ngày hiện tại
            if (BirthdayPicker.SelectedDate > DateTime.Today)
            {
                MessageBox.Show("Birthday cannot be in the future.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // PASSWORD: ít nhất 6 ký tự
            if (PasswordBox.Password.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // TẠO OBJECT GÁN DỮ LIỆU
            if (EditedCustomer == null)
            {
                EditedCustomer = new Customer();
            }
            // Lấy dữ liệu từ các control
            EditedCustomer.CustomerFullName = FullNameBox.Text;
            EditedCustomer.Telephone = TelephoneBox.Text;
            EditedCustomer.EmailAddress = EmailBox.Text;
            EditedCustomer.CustomerBirthday = BirthdayPicker.SelectedDate.HasValue ? DateOnly.FromDateTime(BirthdayPicker.SelectedDate.Value) : null;
            EditedCustomer.CustomerStatus = 1;
            EditedCustomer.Password = PasswordBox.Password;
            

            try
            {
                if (EditedCustomer == null)
                {
                    _service.AddCustomer(EditedCustomer);
                }
                else
                {
                    _service.UpdateCustomer(EditedCustomer);
                }
                this.DialogResult = true;
                MessageBox.Show("Customer saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving the customer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                
            }
                this.Close();
        }
    }
}
