using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SourcesAPI.Models.RequestTypes
{
    public class SetPaymentInfoRequest
    {
        public string PIN { get; set; }
        public decimal Amount { get; set; }
        public DateTime PayDate { get; set; }
    }
}
