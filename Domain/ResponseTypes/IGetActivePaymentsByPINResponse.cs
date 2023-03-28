using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ResponseTypes
{
    public interface IGetActivePaymentsByPINResponse
    {
        string OrganizationName { get; set; }
        DateTime StartDate { get; set; }
        DateTime EndDate { get; set; }
        string PaymentTypeName { get; set; }
        decimal PaymentSize { get; set; }
        /*string LastName { get; set; }
        string FirstName { get; set; }
        string MiddleName { get; set; }*/
        //bool IsActive { get; }
        string[] Dependants { get; set; }
    }
}
