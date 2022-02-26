using Music_Store.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Music_Store.Services
{
    public class UserService
    {
        private static UserService instance;
        private UserService() { }
        public static UserService Instance => instance ?? (instance = new UserService());
        public Employee Employee { get; private set; }
        public void SetEmployee(Employee employee)
        {
            if (employee != null)
            {
                if (Employee == null)
                {
                    Employee = employee;
                }
            }

        }
        public void Logout()
        {
            if (Employee != null)
            {
                Employee = null;
            }

        }
    }
}

