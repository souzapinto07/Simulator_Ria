using Simulator_Ria.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator_Ria
{
    public class Data
    {
        private const int MIN_CUSTOMERS_PER_REQUEST = 2;
        private const int MAX_CUSTOMERS_PER_REQUEST = 5;
        private const int MIN_AGE = 10;
        private const int MAX_AGE = 90;
        private int _idIncremental = 0;
        private Random _random = new Random();


        private string[] FIRST_NAMES = new string[] { "Aarav", "Yuki", "Amina", "Nikolai", "Mei", "Jakub", "Leila", "Rafael", "Priya", "Elijah", "Sakura", "Mohammed", "Ananya", "Gabriel", "Zeynep" };
        private string[] LAST_NAMES = new string[] { "Wang", "Kim", "Nguyen", "Patel", "Silva", "Ivanov", "Kovács", "Müller", "Santos", "Cohen", "Saito", "Khan", "Nowak", "Dubois" };

        public List<Customer> GenerateRandomCustomers()
        {

            int count = _random.Next(MIN_CUSTOMERS_PER_REQUEST, MAX_CUSTOMERS_PER_REQUEST + 1);
            List<Customer> customers = new List<Customer>(count);

            for (int i = 0; i < count; i++)
            {
                string firstName = FIRST_NAMES[_random.Next(FIRST_NAMES.Length)];
                string lastName = LAST_NAMES[_random.Next(LAST_NAMES.Length)];
                int age = _random.Next(MIN_AGE, MAX_AGE + 1);
                _idIncremental++;

                customers.Add(new Customer(lastName, firstName, age, _idIncremental));
            }

            return customers;
        }
    }
}
