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
    public class ApiAccessTests
    {
        private string _consumerKey = "qyprd7kWxwHpnq2YP9UkR4YuCb5ijz";
        private string _consumerSecret = "puF6IGzEAGSZ6fMwrz1z6ssmJnRoZmt9iEy3gAsE";
        private string _accessToken = "qyprdgz716hsYi7Z4SdnaixGqxXS6kniq08pch0uNM1ugJx5";
        private string _accessTokenSecret = "GFwtQy0tfqTNFktaQrccBGMX2rKUIi4blKi1VwJ3";
        private string _realmId = "404669156";
        private string _appUrl = "https://sandbox-quickbooks.api.intuit.com/";


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

        private QboOauthConnection GetOauthConnection()
        {
            return new QboOauthConnection(_appUrl, _accessToken, _accessTokenSecret, _consumerKey, _consumerSecret, _realmId);
        }
    }
}
