using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.RequestTypes
{
    public interface ISaveNotPaymentF20Request
    {
        string OrderPaymentId { get; set; }
        DateTime RegDate { get; set; }
        string ReasonId { get; set; }
    }
}
