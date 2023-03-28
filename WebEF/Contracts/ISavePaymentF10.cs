using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebEF.Contracts.RequestTypes;
using WebEF.Contracts.ResponseTypes;
using XRoadLib.Attributes;

namespace WebEF.Contracts
{
    public interface ISavePaymentF10
    {
        [XRoadService("SavePaymentF10")]
        [XRoadTitle("en", "SavePaymentF10 service")]
        [XRoadNotes("en", "SavePaymentF10")]
        SavePaymentF10Response SavePaymentF10(SavePaymentF10Request request);
    }
}
