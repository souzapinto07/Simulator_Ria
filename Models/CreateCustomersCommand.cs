using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator_Ria.Models
{
    public record CreateCustomersCommand
    {
        public List<Customer> Customers { get; private set; }

        public CreateCustomersCommand(List<Customer> customers)
        {
            Customers = customers;
        }
    }
}
