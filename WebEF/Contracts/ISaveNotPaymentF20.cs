using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebEF.Contracts.RequestTypes;
using WebEF.Contracts.ResponseTypes;
using XRoadLib.Attributes;

namespace WebEF.Contracts
{
    public interface ISaveNotPaymentF20
    {
        [XRoadService("SaveNotPaymentF20")]
        [XRoadTitle("en", "SaveNotPaymentF20 service")]
        [XRoadNotes("en", "SaveNotPaymentF20")]
        SaveNotPaymentF20Response SaveNotPaymentF20(SaveNotPaymentF20Request request);
    }
}
