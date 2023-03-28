using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.ResponseTypes
{
    public class GetActivePaymentsByPINResponse
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PaymentTypeName { get; set; }
        public decimal PaymentSize { get; set; }
    }
}
