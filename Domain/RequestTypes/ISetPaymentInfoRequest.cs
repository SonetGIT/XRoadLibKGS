using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.RequestTypes
{
    public interface ISetPaymentInfoRequest
    {
        string PIN { get; set; }
        decimal Amount { get; set; }
        DateTime PayDate { get; set; }
    }
}
