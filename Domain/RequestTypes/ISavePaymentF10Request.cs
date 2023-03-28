using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.RequestTypes
{
    public interface ISavePaymentF10Request
    {
        string OrderPaymentId { get; set; }
        decimal Amount { get; set; }
        DateTime PayDate { get; set; }
    }
}
