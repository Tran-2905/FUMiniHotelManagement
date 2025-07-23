using FUMiniHotelManagement.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUMiniHotelManagement.DAL.Repositories
{
    public class CustomerRepository 
    {
        private readonly FuminiHotelManagementContext _context;
        public CustomerRepository() { _context = new FuminiHotelManagementContext(); }

        public IEnumerable<Customer> GetAll() => _context.Customers.ToList();
        public Customer GetById(object id) => _context.Customers.Find(id);
        public void Insert(Customer entity)
        {
            try
            {
                _context.Customers.Add(entity);
                Save();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while inserting the customer.", ex);

            }
        }
        public void Update(Customer entity)
        {
            try
            {
                _context.Customers.Update(entity);
                Save();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the customer.", ex);
            }
        }

        public void Delete(object id)
        {
            var obj = _context.Customers.Find(id);
            if (obj != null) _context.Customers.Remove(obj);
            Save();
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


        public Customer GetById(Customer id)
        {
            throw new NotImplementedException();
        }

        public void Delete(Customer id)
        {
            throw new NotImplementedException();
        }
        public Customer getByEmail(string email)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.EmailAddress == email);
            return customer ?? throw new KeyNotFoundException("Customer not found with the provided email.");
        }
    }
}
