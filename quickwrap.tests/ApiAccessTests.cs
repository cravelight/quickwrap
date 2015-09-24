using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intuit.Ipp.Data;
using quickwrap.connections;
using quickwrap.services;
using Xunit;

namespace quickwrap.tests
{
    //todo: refactor and get rid of this class
    // this was really just a starter sandbox
    // we should move all the access testing into the OauthTests
    // then the employee stuff can be moved into its own class
    public class ApiAccessTests : ConnectedBase
    {

        [Fact]
        public void GetAllEmployees()
        {
            var svc = new EmployeeService(GetOauthConnection());
            var employees = svc.List();
            Assert.NotEmpty(employees);
        }

        
        
        [Fact]
        public void CreateFakeEmployee()
        {
            var svc = new EmployeeService(GetOauthConnection());

            var newEmployee = svc.Add(GetFakeEmployee());

            var employees = svc.List().ToList();

            Assert.NotEmpty(employees);
            Assert.True(employees.Any(e => e.Id.Equals(newEmployee.Id)));
        }


        private Employee GetFakeEmployee()
        {
            var rand = new Random();
            var unique = rand.Next(99999).ToString();
            var email = new EmailAddress { Address = unique + "_smith@example.com" };
            var phone = new TelephoneNumber {FreeFormNumber = "650-555-1234" };
            return new Employee
            {
                GivenName = unique,
                FamilyName = "Smith",
                PrimaryEmailAddr = email,
                PrimaryPhone = phone
            };
        }




    }
}
