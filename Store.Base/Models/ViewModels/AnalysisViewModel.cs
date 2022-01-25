using System;
using System.Collections.Generic;
using System.Text;

namespace Store.Base.Models.ViewModels
{
    public class AnalysisViewModel
    {
        public IEnumerable<Vendor> Vendors { get; set; }
        public IEnumerable<Marketer> Marketers { get; set; }
        public IEnumerable<CheckoutOrder> CheckoutOrders { get; set; }

        public IList<UsersCountInCountry> UsersCountInCountries { get; set; }
        public IList<MonthlyRegistration> MonthlyRegistrations { get; set; }

    }
    public class UsersCountInCountry
    {
        public int CountryId { get; set; }
        public string Country { get; set; }
        public int VendorsCount { get; set; }
        public int MarketersCount { get; set; }
        public int AllUsersCount { get; set; }

    }
    public enum Months
    {
        January=1,
        February,
        March, 
        April,
        May,
        June,
        July,
        August,
        September,
        October,
        November,
        December
    };
    public class MonthlyRegistration
    {
        public Months Month { get; set; }
        public int Year { get; set; }
        public int RegistrationCount { get; set; }

    };

}
