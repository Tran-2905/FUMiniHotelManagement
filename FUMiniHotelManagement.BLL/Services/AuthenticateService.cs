using FUMiniHotelManagement.DAL;
using FUMiniHotelManagement.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUMiniHotelManagement.BLL.Services
{
    public class AuthenticateService
    {
        private readonly CustomerRepository _customerRepo;
        public AuthenticateService()
        {
            _customerRepo = new CustomerRepository();
        }
        public Customer Login(string email, string password)
        {
            var customer = _customerRepo.getByEmail(email);
            if (customer.Password != password)
            {
                throw new UnauthorizedAccessException("Invalid password.");
            }
            return customer;
        }
    }
}
