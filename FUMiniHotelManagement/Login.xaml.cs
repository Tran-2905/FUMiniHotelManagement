using FUMiniHotelManagement.BLL.Services;
using FUMiniHotelManagement.DAL;
using Microsoft.Extensions.Configuration;
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
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private readonly IConfiguration _config;
        CustomerService customerService;
        public Login()
        {
            InitializeComponent();
            customerService = new CustomerService();
            _config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string? adminEmail = _config["AdminAccount:Email"];
            string? adminPassword = _config["AdminAccount:Password"];
            var testValue = _config["AdminAccount:Email"];
            if (string.IsNullOrEmpty(testValue))
            {
                MessageBox.Show("Could not load AdminAccount:Email from appsettings.json");
            }
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;
            try
            {
                if(email == adminEmail && password == adminPassword)
                {
                    MessageBox.Show("Admin login successful!");
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.UserName = "Admin";
                    mainWindow.Role = 1;
                    mainWindow.Show();
                    this.Close();
                    return;
                }
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Please enter both email and password.");
                    return;
                }
                FUMiniHotelManagement.DAL.Customer customer = customerService.Authenticate(email, password);
                if (customer != null)
                {
                    MessageBox.Show("Login successful!");
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.UserName = customer.CustomerFullName;
                    mainWindow.Role = 2;
                    mainWindow.Show();
                    this.Close();
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }
    }
}
