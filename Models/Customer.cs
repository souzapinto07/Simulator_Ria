using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator_Ria.Models
{
    public record Customer
    {
        public string LastName { get; private set; }
        public string FirstName { get; private set; }
        public int Age { get; private set; }
        public int Id { get; private set; }

        public Customer(string lastName, string firstName, int age, int id)
        {
            LastName = lastName;
            FirstName = firstName;
            Age = age;
            Id = id;
        }
    }
}
