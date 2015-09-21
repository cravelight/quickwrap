using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intuit.Ipp.Data;
using Intuit.Ipp.DataService;
using quickwrap.connections;

namespace quickwrap.services
{
    public class EmployeeService : QboService<Employee>
    {
        public EmployeeService(IQboConnection connection) : base(connection)
        {
        }

        public override Employee Add(Employee item)
        {
            return Svc.Add(item);
        }

        public override IEnumerable<Employee> List(int startPosition = 1, int maxResults = 500)
        {
            return Svc.FindAll(new Employee(), startPosition, maxResults);
        }


    }
}
