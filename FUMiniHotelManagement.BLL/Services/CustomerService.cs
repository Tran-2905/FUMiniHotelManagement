using FUMiniHotelManagement.DAL;
using FUMiniHotelManagement.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUMiniHotelManagement.BLL.Services
{
    public class CustomerService

    {
        private readonly CustomerRepository _repo;
        public CustomerService()
        {
            _repo = new CustomerRepository();
        }

        public Customer Authenticate(string email, string password)
        {
            var customer = _repo.getByEmail(email);
            if (customer.Password != password)
            {
                throw new UnauthorizedAccessException("Invalid password.");
            }
            return customer;
        }
        public List<Customer> GetAllCustomers()
        {
            return _repo.GetAll().ToList();
        }
        public void AddCustomer(Customer customer)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer), "Customer cannot be null.");
            }
            _repo.Insert(customer);
        }

        public void UpdateCustomer(Customer customer)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer), "Customer cannot be null.");
            }
            _repo.Update(customer);
        }
        public void DeleteCustomer(int id)
        {
            var customer = _repo.GetById(id);
            if (customer == null)
            {
                throw new KeyNotFoundException($"Customer with ID {id} not found.");
            }
            _repo.Delete(id);
        }

        public List<Customer> SearchCustomers(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                throw new ArgumentException("Search term cannot be null or empty.", nameof(searchTerm));
            }
            return _repo.GetAll()
                        .Where(c => c.CustomerFullName != null && c.CustomerFullName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                        .ToList();
        }
    }
}
