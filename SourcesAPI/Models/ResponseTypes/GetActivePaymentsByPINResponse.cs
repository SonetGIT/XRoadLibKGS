
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SourcesAPI.Models.ResponseTypes
{
    public class GetActivePaymentsByPINResponse
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PaymentTypeName { get; set; }
        public decimal PaymentSize { get; set; }
        public string OrganizationName { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public bool IsActive
        {
            get
            {
                return EndDate > DateTime.Today;
            }
        }
        public string[] Dependants { get; set; }
    }
}